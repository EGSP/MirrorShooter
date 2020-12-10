using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Egsp.Core;
using Egsp.Core.Ui;
using Game.Configuration;
using Game.Sessions;
using Game.Views;
using Game.Views.Client;
using Mirror;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace Game.Net
{
    [LazyInstance(false)]
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

        public ClientLobbyScene Scene { get; private set; }


        #region Setup

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
            // NetworkManager.OnClientChangeSceneEvent += CreateSession;
            NetworkManager.OnClientSceneChangedEvent += StartSession;
            NetworkManager.OnClientSceneChangedEvent += SceneChanged;

            Scene = new ClientLobbyScene(this);
            
            IsInitialized = true;
            
            InitializeView();
        }

        private void InitializeView()
        {
            var view = ViewFactory.LoadAndInstantiateView<ClientMenuView>("client_menu");
            ViewFactory.LoadAndInstantiateView<ClientInLobbyView>("client_lobby", false);
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
            
            NetworkClient.RegisterHandler<SessionStateMessage>(OnServerSessionStateChanged);

            NetworkManager.StartClient();
        }

        #endregion

        #region Client processing

        /// <summary>
        /// Обработка присоединения к серверу.
        /// </summary>
        /// <param name="conn"></param>
        private void Connected(NetworkConnection conn)
        {
            Debug.Log($"Client lobby: connected to {NetworkClient.serverIp}");
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

        #endregion

        #region User processing

        public void LoginAs(User user)
        {
            LaunchInfo.User = user;
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
                
                
                // Debug.Log($"New User {message.user.name} : {LobbyUsers.Count}");
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

        #region Scene processing

        /// <summary>
        /// Меняет сцену и отключается от сервера, если было подключение.
        /// </summary>
        public void ChangeScene(string sceneName)
        {
            if(isConnected)
                Disconnect();
            
            if (Application.CanStreamedLevelBeLoaded(sceneName) == false)
            {
                Debug.Log($"Scene \"{sceneName}\" not exist in the current build!" +
                          $" But you are trying to access it!");
                return;
            }
            
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Меняет сцену и отключается от сервера, если было подключение.
        /// </summary>
        public void ChangeScene(int sceneId)
        {
            if(isConnected)
                Disconnect();
            
            if (Application.CanStreamedLevelBeLoaded(sceneId) == false)
            {
                Debug.Log($"Scene \"{sceneId}\" not exist in the current build!" +
                          $" But you are trying to access it!");
                return;
            }
            
            SceneManager.LoadScene(sceneId);
        }
        
        private void SceneChanged()
        {
            // Debug.Log($"SCENE CHANGED TO {SceneManager.GetActiveScene().name}");
            NetworkClient.Send<SceneLoadedMessage>(new SceneLoadedMessage());
        }
        
        /// <summary>
        /// Устанавливает флаг NetworkScene.isReady = true.
        /// </summary>
        public void ReadyForChangeScene()
        {
            Debug.Log("READY FOR CHANGE SCENE");
            if (!IsAcceptedUser)
                return;

            CreateSession();
            SetReady();
        }
        
        private void SetReady()
        {
            ClientScene.Ready(NetworkClient.connection);
        }

        #endregion

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
            // NetworkManager.OnClientChangeSceneEvent -= CreateSession;
            NetworkManager.OnClientSceneChangedEvent -= StartSession;
            NetworkManager.OnClientSceneChangedEvent -= SceneChanged;
            
            Destroy(gameObject);
        }

        #region Session setup

        public void GetServerSessionState()
        {
            NetworkClient.Send<SessionStateMessage>(new SessionStateMessage());
        }
        
        private void OnServerSessionStateChanged(SessionStateMessage msg)
        {
            OnServerSession(msg);
        }
        
        private void CreateSession()
        {
            ClientSession.CreateInstance();
        }

        private void StartSession()
        {
            Debug.Log("StartSession");
            ClientSession.Instance.StartSession();
        }

        public void LoadMenuScene()
        {
            Scene.LoadMenuScene().With(InitializeView);
        }

        #endregion
    }

    public class ClientLobbyScene
    {
        private readonly ClientLobby lobby;
        
        public ClientLobbyScene(ClientLobby lobby)
        {
            this.lobby = lobby;
        }
        
        // public string Name { get; private set; }

        private IEnumerator sceneLoadRoutine;
        
        public CallBack LoadMenuScene()
        {
            var cb = new CallBack();

            if (sceneLoadRoutine != null)
                return cb;

            sceneLoadRoutine = LoadSceneRoutine(Preloader.Instance.OfflineScene, cb);
            lobby.StartCoroutine(sceneLoadRoutine);
            return cb;
        }

        private IEnumerator LoadSceneRoutine(string sceneName, CallBack callBack)
        {
            var sceneAo = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            while (!sceneAo.isDone)
            {
                yield return null;
            }

            // Ожидание в одну секунду.
            yield return new WaitForSeconds(0.2f);
            sceneLoadRoutine = null;
            
            callBack.On();
        }
        
    }
}