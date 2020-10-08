using System;
using System.Collections.Generic;
using System.Linq;
using Gasanov.Core;
using Gasanov.Extensions.Mono;
using Mirror;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Net
{
    
    public class ServerLobby : SerializedSingleton<ServerLobby>
    {
        [OdinSerialize]
        public EventNetworkManager NetworkManager { get; private set; }

        public bool HasStarted { get ; private set; }
        
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

        public List<UserConnection> Connections { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            AlwaysExist = true;
            
            NetworkManager = this.ValidateComponent(NetworkManager);
            if (NetworkManager == null)
                throw new NullReferenceException();

            Connections = new List<UserConnection>();
        }

        private void Start()
        {
            NetworkManager.OnServerStarted += Started;
            NetworkManager.OnServerStopped += Stopped;
            NetworkManager.OnClientConnected += ClientConnected;
            NetworkManager.OnClientDisconnected += ClientDisconnected;
            
            
        }

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
            
            coincidence.User = new User(message.Name);
            
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
                        userConn.Connection.Send<AddUserMessage>(new AddUserMessage() {Name = newUser.Name});
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
                            new DisconnectUserMessage() {Name = disconnectedUser.Name});
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
                            new AddUserMessage() {Name = anotherUserConnection.User.Name});
                    }
                }
            }

            Debug.Log($"Sended lobby users : {lobbyMessage.AddUserMessages.Count}");
            
            userConnection.Connection.Send<LobbyUsersMessage>(lobbyMessage);
        }

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
    }
}