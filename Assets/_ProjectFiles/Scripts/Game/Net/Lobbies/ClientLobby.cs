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
        public event Action OnConnected = delegate {  };
        public event Action OnDisconnect = delegate {  };
        
        public event Action<User> OnAddUser = delegate(User user) {  };
        public event Action<User> OnDisconnectUser = delegate(User user) {  };
        
        /// <summary>
        /// Подключен ли клиент к лобби.
        /// </summary>
        [NonSerialized] public bool isConnected = NetworkClient.isConnected;
        
        /// <summary>
        /// Инициализировано ли лобби.
        /// </summary>
        public bool IsInitialized { get; private set; }
        
        public ClientSession Session { get; private set; }

        [OdinSerialize]
        public EventNetworkManager NetworkManager { get; set; }
        
        /// <summary>
        /// Пользователь процесса игры.
        /// </summary>
        public User MainUser
        {
            get => LaunchInfo.User;
            set => LaunchInfo.User = value;
        }

        [OdinSerialize]
        public List<User> LobbyUsers { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            AlwaysExist = true;
            LobbyUsers = new List<User>();
            
            // NetworkManager = this.ValidateComponent(NetworkManager);
            // if (NetworkManager == null)
            //     throw new NullReferenceException();
        }

        public void Initialize()
        {
            if (IsInitialized == true)
                return;
            
            NetworkManager.OnConnectedToServer += Connected;
            NetworkManager.OnDisconnectedFromServer += DisconnectedFromServer;
            NetworkManager.OnClientChangeSceneEvent += () =>
            {
                Session = gameObject.AddComponent<ClientSession>();
                Session.NetworkManager = NetworkManager;
            };
            NetworkManager.OnClientSceneChangedEvent += () => Session.StartSession();
            

            IsInitialized = true;
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
            NetworkClient.RegisterHandler<UpdateUserMessage>(UpdateUserInfo);

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
            var message = new AddUserMessage(){user = LaunchInfo.User};
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
        private void DisconnectedFromServer(NetworkConnection conn)
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
            if (!LobbyUsers.Exists(x => x.id == message.user.id))
            {
                
                LobbyUsers.Add(message.user);
                
                
                Debug.Log($"New User {message.user.name} : {LobbyUsers.Count}");
                OnAddUser(message.user);
                return;
            }
            

            Debug.Log($"Пользователь уже был занесен в список {message.user.name}");
        }

        /// <summary>
        /// Обработка отключения пользователя.
        /// </summary>
        public void DisconnectUser(DisconnectUserMessage message)
        {
            var coincidence = LobbyUsers.FirstOrDefault(x => x.id == message.user.id);

            if (coincidence != null)
            {
                LobbyUsers.Remove(coincidence);
                OnDisconnectUser(coincidence);
            }
        }

        /// <summary>
        /// Обновление информации о текущем пользователе.
        /// </summary>
        public void UpdateUserInfo(UpdateUserMessage message)
        {
            MainUser = message.updatedUser;
        }

        /// <summary>
        /// Очищение данных лобби.
        /// </summary>
        private void ClearLobby()
        {
            LobbyUsers.Clear();
        }
    }
}