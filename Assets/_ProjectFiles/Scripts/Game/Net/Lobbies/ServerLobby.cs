using System;
using System.Collections.Generic;
using System.Linq;
using Gasanov.Core;
using Gasanov.Extensions.Mono;
using Mirror;
using Sirenix.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Net
{
    
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
        public event Action<User> OnUserDisconnected = delegate(User user) {  };

        [OdinSerialize]
        public EventNetworkManager NetworkManager { get; set; }

        public bool HasStarted { get ; private set; }

        public bool IsInitialized { get; private set; }
        
        [OdinSerialize]
        /// <summary>
        /// Все текущие пользователи.
        /// </summary>
        public List<UserConnection> Connections { get; private set; }

        /// <summary>
        /// Текущая сессия сервера.
        /// </summary>
        public ServerSession Session { get; private set; }

        #region Setup
        protected override void Awake()
        {
            base.Awake();
            AlwaysExist = true;
            
            // NetworkManager = this.ValidateComponent(NetworkManager);
            // if (NetworkManager == null)
            //     throw new NullReferenceException();

            Connections = new List<UserConnection>();
        }

        public void Initialize()
        {
            if (IsInitialized == true) 
                return;
            
            NetworkManager.OnServerStarted += Started;
            NetworkManager.OnServerStopped += Stopped;
            NetworkManager.OnClientConnected += ClientConnected;
            NetworkManager.OnClientDisconnected += ClientDisconnected;

            Session = gameObject.AddComponent<ServerSession>();
            Session.NetworkManager = NetworkManager;
            Session.ServerLobby = this;
            IsInitialized = true;
        }
        #endregion
        
        #region Base work

        public void StartServer(string port)
        {
            var transportAdapter = NetworkManager.GetComponent<TransportAdapter>();
            transportAdapter.SetPort(port);

            NetworkServer.RegisterHandler<AddUserMessage>(RegisterUser);
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
            Debug.Log("Server lobby: someone connected!");
            Connections.Add(new UserConnection(clientConnection));
            OnClientConnected();
        }

        private void ClientDisconnected(NetworkConnection clientConnection)
        {
            Debug.Log("Server lobby: someone disconnected");
            var coincidence = Connections.FirstOrDefault(x
                => x.Connection == clientConnection);

            if (coincidence != null)
            {
                Connections.Remove(coincidence);
                if (coincidence.User != null)
                {
                    DisconnectUser(coincidence.User);
                }
            }

            OnClientDisconnected();
        }

        /// <summary>
        /// Связывает пользователя с подключением.
        /// </summary>
        private void RegisterUser(NetworkConnection conn, AddUserMessage message)
        {
            var coincidence = Connections.FirstOrDefault(x 
                => x.Connection == conn);
            
            coincidence.User = message.user;
            
            UpdateUserInfo(coincidence);
            GiveUserInfoAboutLobby(coincidence);
            
            NotifyClientsNewOne(coincidence.User);
            OnUserConnected(coincidence.User);
        }

        /// <summary>
        /// Удаляет пользователя и его подключение.
        /// Оповещает остальных.
        /// </summary>
        private void DisconnectUser(User disconnectedUser)
        {
            NotifyClientsDisconnectedOne(disconnectedUser);
            OnUserDisconnected(disconnectedUser);
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

            Debug.Log($"Sended lobby users : {lobbyMessage.AddUserMessages.Count}");
            
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