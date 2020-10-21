using System;
using System.Collections.Generic;
using System.Linq;
using Game.Sessions;
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
        /// Вызывается при получении информации о сессии с сервера.
        /// </summary>
        public event Action<SessionStateMessage> OnServerSession = delegate(SessionStateMessage message) {  };
        
        /// <summary>
        /// Подключен ли клиент к лобби.
        /// </summary>
        [NonSerialized] public bool isConnected = NetworkClient.isConnected;
        
        /// <summary>
        /// Принят ли пользователь сервером на данный момент.
        /// </summary>
        public bool IsAcceptedUser { get; private set; }
        
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
        }

        public void Initialize()
        {
            if (IsInitialized == true)
                return;
            
            NetworkManager.OnConnectedToServer += Connected;
            NetworkManager.OnDisconnectedFromServer += DisconnectedFromServer;
            NetworkManager.OnClientChangeSceneEvent += CreateSession;
            NetworkManager.OnClientSceneChangedEvent += StartSession;
            NetworkManager.OnClientSceneChangedEvent += SceneChanged;

            NetworkIdentity.ApplicationMode = NetworkIdentity.ApplicationModeType.Client;

            IsInitialized = true;
        }

        /// <summary>
        /// Попытка присоединения к серверу.
        /// </summary>
        public void ConnectToServer(string address, string port)
        {
            NetworkManager.networkAddress = address;
            var transportAdapter = NetworkManager.GetComponent<TransportAdapter>();
            transportAdapter.SetPort(port);
            
            NetworkClient.RegisterHandler<LobbyUsersMessage>(OnLobbyUsersMessage);
            NetworkClient.RegisterHandler<AddUserMessage>(AddUser);
            NetworkClient.RegisterHandler<DisconnectUserMessage>(DisconnectUser);
            NetworkClient.RegisterHandler<UpdateUserMessage>(UpdateUserInfo);
            
            NetworkClient.RegisterHandler<SessionStateMessage>(OnServerSessionChanged);

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
            ClearData();
            OnDisconnect();
        }

        /// <summary>
        /// Вызывается после отключения от сервера.
        /// </summary>
        /// <param name="conn"></param>
        private void DisconnectedFromServer(NetworkConnection conn)
        {
            ClearData();
            OnDisconnect();
        }

        private void SceneChanged()
        {
            NetworkClient.Send<SceneLoadedMessage>(new SceneLoadedMessage());
            Ready();
        }

        #region User processing

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
        /// При первом подключении сервер дает пользователю идентификатор.
        /// </summary>
        public void UpdateUserInfo(UpdateUserMessage message)
        {
            MainUser = message.updatedUser;

            // Пользователь принят сервером.
            if (MainUser.id != -1)
                IsAcceptedUser = true;
        }
        
        #endregion

        /// <summary>
        /// Устанавливает флаг NetworkScene.isReady = true.
        /// </summary>
        public void Ready()
        {
            if (!IsAcceptedUser)
                return;

            ClientScene.Ready(NetworkClient.connection);
        }

        /// <summary>
        /// Очищение данных лобби.
        /// </summary>
        private void ClearData()
        {
            LobbyUsers.Clear();
        }

        /// <summary>
        /// Уничтожает лобби и сессию.
        /// </summary>
        public void DestroyLobby()
        {
            NetworkManager.OnConnectedToServer -= Connected;
            NetworkManager.OnDisconnectedFromServer -= DisconnectedFromServer;
            NetworkManager.OnClientChangeSceneEvent -= CreateSession;
            NetworkManager.OnClientSceneChangedEvent -= StartSession;
            NetworkManager.OnClientSceneChangedEvent -= SceneChanged;
            
            DestroySession();
            Destroy(gameObject);
        }

        #region Session setup

        public void GetServerSession()
        {
            NetworkClient.Send<SessionStateMessage>(new SessionStateMessage());
        }
        
        private void OnServerSessionChanged(SessionStateMessage msg)
        {
            OnServerSession(msg);
        }
        
        private void CreateSession()
        {
            Session = gameObject.AddComponent<ClientSession>();
            Session.NetworkManager = NetworkManager;
        }

        private void DestroySession()
        {
            Destroy(Session.gameObject);
        }

        private void StartSession()
        {
            if (Session == null)
            {
                throw new NullReferenceException();
            }

            Session.StartSession();
        }

        #endregion
    }
}