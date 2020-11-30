using System;
using System.Collections.Generic;
using System.Linq;
using Game.Configuration;
using Game.Presenters.Server;
using Game.Sessions;
using Game.Views;
using Game.Views.Server;
using Gasanov.Core;
using Gasanov.Core.Mvp;
using Gasanov.Extensions.Mono;
using Mirror;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Game.Net
{
    [LazyInstance(false)]
    public class ServerLobby : SerializedSingleton<ServerLobby>
    {
        public event Action OnStarted = delegate {  };
        public event Action OnShutdown = delegate {  };
        
        public event Action OnClientConnected = delegate {  };
        public event Action OnClientDisconnected = delegate {  };
        
        /// <summary>
        /// Вызывается когда соединение связывает себя с пользователем.
        /// </summary>
        public event Action<User> OnUserConnected = delegate(User user) {  };
        /// <summary>
        /// Вызывается когда пользователь отключается от сервера.
        /// </summary>
        public event Action<UserConnection> OnUserDisconnected = delegate(UserConnection user) {  };
        /// <summary>
        /// Вызывается когда пользователь готов принимать данные.
        /// </summary>
        public event Action<UserConnection> OnUserReady = delegate(UserConnection user) {  };
        /// <summary>
        /// Вызывается когда пользователь сообщает о своей загрузке на сцену.
        /// </summary>
        public event Action<UserConnection> OnUserLoadedToScene = delegate(UserConnection connection) {  };

        public EventNetworkManager NetworkManager { get; set; }

        public bool HasStarted { get ; private set; }

        public bool IsInitialized { get; private set; }
        
        [OdinSerialize]
        /// <summary>
        /// Все текущие пользователи.
        /// </summary>
        public List<UserConnection> Connections { get; private set; }

        /// <summary>
        /// Все обработанные подключения подключения.
        /// </summary>
        public IEnumerable<UserConnection> RegisteredUsers =>
            Connections.Where(x => x.IsValidated);

        /// <summary>
        /// Все обработанные и готовые подключения.
        /// </summary>
        public IEnumerable<UserConnection> ReadyUsers =>
            Connections.Where(x => x.IsValidated && x.User.IsReady);

        
        public ServerScene Scene { get; private set; }

        /// <summary>
        /// Текущая сессия сервера.
        /// </summary>
        public ServerSession Session => ServerSession.Instance;

        #region Setup
        protected override void Awake()
        {
            base.Awake();
            AlwaysExist = true;
            Connections = new List<UserConnection>();
        }

        public void Initialize()
        {
            if (IsInitialized == true) 
                return;
         
            Debug.Log("Initialize");
            
            NetworkManager.OnServerStarted += Started;
            NetworkManager.OnServerStopped += Stopped;
            NetworkManager.OnServerConnectEvent += ClientConnected;
            NetworkManager.OnServerDisconnectEvent += ClientDisconnected;
            NetworkManager.OnServerReadyEvent += ClientReady;
            
            Scene = new ServerScene(this);
            
            CreateSession();
            
            IsInitialized = true;
            
            InitializeView();
        }
        
        private void InitializeView()
        {
            ViewFactory.LoadAndInstantiateView<ServerMenuView>("server_menu");
            ViewFactory.LoadAndInstantiateView<ServerInLobbyView>("server_lobby", false);
        }
        
        #endregion
        
        #region Base work

        public void StartServer(string port)
        {
            var transportAdapter = NetworkManager.GetComponent<TransportAdapter>();
            transportAdapter.SetPort(port);

            NetworkServer.RegisterHandler<AddUserMessage>(RegisterUser);
            NetworkServer.RegisterHandler<SceneLoadedMessage>(OnSceneLoadedMessage);
            NetworkServer.RegisterHandler<SessionStateMessage>(ShareServerSession);
            
            NetworkManager.StartServer();
        }

        public void Shutdown()
        {
            NetworkManager.StopServer();
        }

        private void Started()
        {
            OnStarted();
            HasStarted = true;
        }

        private void Stopped()
        {
            Connections.Clear();
            OnShutdown();
            HasStarted = false;
        }
        
        #endregion

        #region Client processing
        
        private void ClientConnected(NetworkConnection clientConnection)
        {
            Debug.Log($"Server lobby: someone connected! {clientConnection.connectionId}");
            Connections.Add(new UserConnection(clientConnection));
            OnClientConnected();
        }

        private void ClientDisconnected(NetworkConnection clientConnection)
        {
            Debug.Log("Server lobby: someone disconnected");
            var userConnection = Connections.FirstOrDefault(x
                => x.Connection == clientConnection);

            if (userConnection != null)
            {
                if (userConnection.User != null)
                {
                    UserDisconnected(userConnection);
                }
                Connections.Remove(userConnection);
            }

            OnClientDisconnected();
        }

        private void ClientReady(NetworkConnection clientConnection)
        {
            // Если готовый клиент - пользователь
            var uc = Val(clientConnection);

            if (uc.IsValidated)
            {
                uc.User.IsReady = true;
                OnUserReady(uc);
                return;
            }
            
        }
        
        #endregion

        #region User processing

        /// <summary>
        /// Связывает пользователя с подключением.
        /// </summary>
        private void RegisterUser(NetworkConnection conn, AddUserMessage message)
        {
            var coincidence = Connections.FirstOrDefault(x 
                => x.Connection == conn);
            
            coincidence.User = message.user;
            
            // Даем пользователю идентификатор. Это говорит о том, что пользователь полностью проверен.
            UpdateUserInfo(coincidence);
            
            GiveUserInfoAboutLobby(coincidence);
            
            NotifyClientsNewOne(coincidence.User);
            OnUserConnected(coincidence.User);
        }

        /// <summary>
        /// Удаляет пользователя и его подключение.
        /// Оповещает остальных.
        /// </summary>
        private void UserDisconnected(UserConnection userConnection)
        {
            NotifyClientsDisconnectedOne(userConnection.User);
            
            OnUserDisconnected(userConnection);
        }

        /// <summary>
        /// Оповещение клиентов о добавлении пользователя.
        /// </summary>
        /// <param name="newUser"></param>
        private void NotifyClientsNewOne(User newUser)
        {
            for (var i = 0; i < Connections.Count; i++)
            {
                var userConn = Connections[i];
                
                // Если соединение зарегестрировано полностью.
                if (userConn.User != null)
                {
                    // Если это не этот же пользователь.
                    if (userConn.User != newUser)
                    {
                        // Оповещаем остальных пользователей.
                        userConn.Connection.Send<AddUserMessage>(new AddUserMessage() {user = newUser});
                    }
                }
            }
        }

        /// <summary>
        /// Оповещение клиентов об отключении пользователя.
        /// </summary>
        private void NotifyClientsDisconnectedOne(User disconnectedUser)
        {
            for (var i = 0; i < Connections.Count; i++)
            {
                var userConn = Connections[i];
                
                // Если соединение зарегестрировано полностью.
                if (userConn.User != null)
                {
                    // Если это не этот же пользователь.
                    if (userConn.User != disconnectedUser)
                    {
                        // Оповещаем остальных пользователей.
                        userConn.Connection.Send<DisconnectUserMessage>(
                            new DisconnectUserMessage() {user = disconnectedUser});
                    }
                }
            }
        }

        /// <summary>
        /// Передача информации о пользователе. 
        /// </summary>
        private void GiveUserInfoAboutLobby(UserConnection userConnection)
        {
            // Если это не первое подключение.
            if (Connections.Count == 1)
                return;
            
            var lobbyMessage = new LobbyUsersMessage();
            for (var i = 0; i < Connections.Count; i++)
            {
                var anotherUserConnection = Connections[i];
                
                // Если соединение зарегестрировано полностью.
                if (anotherUserConnection.User != null)
                {
                    // Если это не этот же пользователь.
                    if (anotherUserConnection.User != userConnection.User)
                    {
                        // userConnection.Connection.Send<AddUserMessage>(
                        //     new AddUserMessage(){Name = anotherUserConnection.User.Name});

                        lobbyMessage.AddUserMessages.Add(
                            new AddUserMessage() {user = anotherUserConnection.User});
                    }
                }
            }

            // Debug.Log($"Sended lobby users : {lobbyMessage.AddUserMessages.Count}");
            
            userConnection.Connection.Send<LobbyUsersMessage>(lobbyMessage);
        }

        private void UpdateUserInfo(UserConnection userConnection)
        {
            int exc = 0;
            bool uniqueId = false;

            while (uniqueId != true)
            {
                exc++;
                if(exc > 1_00)
                    throw new ArgumentOutOfRangeException();
                
                var id = Random.Range(1, int.MaxValue);
                
                // Если хоть один номер совпал.
                if(Connections.Exists(x=>x.User.id == id))
                    continue;

                // Создан уникальный идентификатор.
                uniqueId = true;
                userConnection.User.id = id;
            }

            var message = new UpdateUserMessage();
            message.updatedUser = userConnection.User;

            userConnection.Connection.Send<UpdateUserMessage>(message);
        }
        
        private void OnSceneLoadedMessage(NetworkConnection connection, SceneLoadedMessage msg)
        {
            var uc = Val(connection);
            if (uc != null)
            {
                OnUserLoadedToScene(uc);
            }
        }

        #endregion

        #region Communication

        /// <summary>
        /// Отправка сообщения всем пользователям.
        /// </summary>
        public void SendToAll<TMessage>(TMessage message)
            where TMessage: IMessageBase
        {
            for (var i = 0; i < Connections.Count; i++)
            {
                var userConnection = Connections[i];
                
                // Если соединение зарегестрировано полностью.
                if (userConnection.User != null)
                {
                    // Отправка пользователю сообщения.
                    userConnection.Connection.Send<TMessage>(message);
                }
            }
        }

        public void SendToAll<TMessage>(TMessage message, Predicate<User> excludePredicate)
            where TMessage: IMessageBase
        {
            for (var i = 0; i < Connections.Count; i++)
            {
                var userConnection = Connections[i];
                
                // Если соединение зарегестрировано полностью.
                if (userConnection.User != null)
                {
                    // Отправка пользователю сообщения.
                    if(excludePredicate(userConnection.User) == false)
                        userConnection.Connection.Send<TMessage>(message);
                }
            }
        }
        
        #endregion

        /// <summary>
        /// Обновляет информацию о сессии у пользователя.
        /// </summary>
        private void ShareServerSession(NetworkConnection clientConnection, SessionStateMessage msg)
        {
            if (Session == null)
                return;

            clientConnection.Send<SessionStateMessage>(Session.StateMessage);
        }

        /// <summary>
        /// Передает всем клиентам информацию о сессии.
        /// </summary>
        public void ShareServerSessionForConnections(SessionStateMessage msg)
        {
            foreach (var userConnection in Connections)
            {
                userConnection.Connection.Send<SessionStateMessage>(msg);
            }
        }

        private void CreateSession()
        {
            ServerSession.CreateInstance();
        }

        public void StartSession()
        {
            throw new Exception();
        }

        public void ChangeServerScene(string sceneName)
        {
            Scene.ChangeServerScene(sceneName);
        }
        
        /// <summary>
        /// Ищет пользовательское соединение связанное с этим NetworkConnection.
        /// При отсутствии возвращает null.
        /// </summary>
        public UserConnection Val(NetworkConnection conn)
        {
            var coincidence = Connections.FirstOrDefault(x 
                => x.Connection == conn);

            return coincidence;
        }
    }
}