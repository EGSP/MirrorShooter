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
    
    public class ClientLobby : SerializedSingleton<ClientLobby>
    {
        /// <summary>
        /// Подключен ли клиент к лобби.
        /// </summary>
        [NonSerialized] public bool isConnected = NetworkClient.isConnected;
        
        [OdinSerialize]
        public EventNetworkManager NetworkManager { get; private set; }
        
        public event Action OnConnected = delegate {  };
        public event Action OnDisconnect = delegate {  };
        
        public event Action<User> OnAddUser = delegate(User user) {  };
        public event Action<User> OnDisconnectUser = delegate(User user) {  };

        

        /// <summary>
        /// Пользователь процесса игры.
        /// </summary>
        public User MainUser => LaunchInfo.User;
        
        private List<User> _lobbyUsers;
        
        protected override void Awake()
        {
            base.Awake();
            AlwaysExist = true;
            _lobbyUsers = new List<User>();
            
            NetworkManager = this.ValidateComponent(NetworkManager);
            if (NetworkManager == null)
                throw new NullReferenceException();
        }

        private void Start()
        {
            NetworkManager.OnConnectedToServer += Connected;
            NetworkManager.OnDisconnectedFromServer += Disconnected;
        }

        /// <summary>
        /// Попытка присоединения к серверу.
        /// </summary>
        public void TryConnectToServer(string address, string port)
        {
            NetworkManager.networkAddress = address;
            var transportAdapter = NetworkManager.GetComponent<TransportAdapter>();
            transportAdapter.SetPort(port);
            
            NetworkClient.RegisterHandler<LobbyUsersMessage>(OnLobbyUsersMessage);
            NetworkClient.RegisterHandler<AddUserMessage>(AddUser);
            NetworkClient.RegisterHandler<DisconnectUserMessage>(DisconnectUser);

            NetworkManager.StartClient();
        }

        /// <summary>
        /// Обработка присоединения к серверу.
        /// </summary>
        /// <param name="conn"></param>
        private void Connected(NetworkConnection conn)
        {
            Debug.Log($"Client lobby connected to {NetworkClient.serverIp}");
            OnConnected();
            
            ConnectAsUser();
        }

        private void ConnectAsUser()
        {
            var message = new AddUserMessage(){Name = LaunchInfo.User.Name};
            NetworkClient.Send<AddUserMessage>(message);

            // Локальное добавления самого себя.
            AddUser(message);
        }

        /// <summary>
        /// Отключение от сервера.
        /// </summary>
        public void Disconnect()
        {
            NetworkManager.StopClient();
            ClearLobby();
            OnDisconnect();
        }

        /// <summary>
        /// Вызывается после отключения от сервера.
        /// </summary>
        /// <param name="conn"></param>
        private void Disconnected(NetworkConnection conn)
        {
            ClearLobby();
            OnDisconnect();
        }

        /// <summary>
        /// Обработка информации о лобби. 
        /// </summary>
        public void OnLobbyUsersMessage(LobbyUsersMessage message)
        {
            foreach (var addUserMessage in message.AddUserMessages)
            {
                AddUser(addUserMessage);
            }
        }

        /// <summary>
        /// Добавление нового пользователя в лобби.
        /// </summary>
        public void AddUser(AddUserMessage message)
        {
            // Если пользователь новый
            if (!_lobbyUsers.Exists(x => x.Name == message.Name))
            {
                var user = new User(message.Name);
                _lobbyUsers.Add(user);
                
                
                Debug.Log($"New User {message.Name} : {_lobbyUsers.Count}");
                OnAddUser(user);
                return;
            }
            

            Debug.Log($"Пользователь уже был занесен в список {message.Name}");
        }

        /// <summary>
        /// Обработка отключения пользователя.
        /// </summary>
        public void DisconnectUser(DisconnectUserMessage message)
        {
            var coincidence = _lobbyUsers.FirstOrDefault(x => x.Name == message.Name);

            if (coincidence != null)
            {
                _lobbyUsers.Remove(coincidence);
                OnDisconnectUser(coincidence);
            }
        }

        /// <summary>
        /// Очищение данных лобби.
        /// </summary>
        private void ClearLobby()
        {
            _lobbyUsers.Clear();
        }
    }
}