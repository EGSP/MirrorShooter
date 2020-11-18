using System;
using System.Collections.Generic;
using System.Linq;
using Game.Entities;
using Game.Entities.Controllers;
using Game.Net;
using Game.Net.Objects;
using Game.Sessions.Observers;
using Game.World;
using Gasanov.Core;
using Gasanov.Extensions.Linq;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Sessions
{
    [LazyInstance(false)]
    public class ServerSession : SerializedSingleton<ServerSession>
    {   
        [NonSerialized] public EventNetworkManager NetworkManager;
        [NonSerialized] public ServerLobby ServerLobby;
        
        /// <summary>
        /// Началась ли сессия.
        /// </summary>
        public bool IsStarted { get; private set; }
        
        /// <summary>
        /// Нужно ли спавнить контроллер для сервера.
        /// </summary>
        public bool SpawnServerController { get; set; }

        /// <summary>
        /// Префаб сущности игрока.
        /// </summary>
        private PlayerEntity _playerEntityPrefab;
        
        /// <summary>
        /// Префаб контроллера игрока.
        /// </summary>
        private PlayerController _playerController;
        
        /// <summary>
        /// Создает сообщение о сессии на основе текущего состояния.
        /// </summary>
        public SessionStateMessage StateMessage
        {
            get
            {
                var msg = new SessionStateMessage(){IsStarted = this.IsStarted};
                return msg;
            }
        }
        

        /// <summary>
        /// Список пользователей находящихся на сцене. ПЕРЕНЕСТИ В ЛОГИКУ СЦЕНЫ
        /// </summary>
        private List<UserHandler> _userConnectionsInScene;

        /// <summary>
        /// Все текущие сущности игроков.
        /// </summary>
        public List<PlayerEntity> PlayerEntities { get; private set; }
        
        /// <summary>
        /// Вызывается при добавлении новой сущности игрока.
        /// </summary>
        public event Action<PlayerEntity> OnPlayerEntityAdd = delegate(PlayerEntity entity) {  };
        /// <summary>
        /// Вызывается при удалении сущности игрока.
        /// </summary>
        public event Action<PlayerEntity> OnPlayerEntityRemove = delegate(PlayerEntity entity) {  };

        public List<SessionObserver> SessionObservers { get; private set; }

        protected override void OnInstanceCreated()
        {
            base.OnInstanceCreated();
            
            NetworkManager = Mirror.NetworkManager.singleton as EventNetworkManager;
            ServerLobby = ServerLobby.Instance;
            
            NetworkManager.OnServerSceneChangedEvent += StartSession;
            ServerLobby.OnUserLoadedToScene += ProcessLoadedUser;
            
            AlwaysExist = true;
        }

        private void Start()
        {
            _playerEntityPrefab = Resources.Load<PlayerEntity>("Prefabs/Player");
            if(_playerEntityPrefab == null)
                throw new NullReferenceException();

            _playerController = Resources.Load<PlayerController>("Prefabs/PC");
            if (_playerController == null)
                throw new NullReferenceException();
            
            _userConnectionsInScene = new List<UserHandler>();
            PlayerEntities = new List<PlayerEntity>();
            SessionObservers = new List<SessionObserver>();
            
            SessionObservers.Add(new PlayerTransformObserver(this));
        }

        private void Update()
        {
            UpdateObservers();
        }
        

        public void StartSession()
        {
            ServerLobby.OnUserReady += ProcessReadyUser;
            ServerLobby.OnUserDisconnected += ProcessDisconnectedUser;
            
            IsStarted = true;
            ServerLobby.ShareServerSessionForConnections(StateMessage);

            // Спавн серверного контроллера
            if (SpawnServerController)
            {
                var playerEntityServerPrefab = Resources.Load<PlayerEntity>("Prefabs/Player_Server");
                if(playerEntityServerPrefab == null)
                    throw new NullReferenceException();

                 var playerControllerServer = Resources.Load<PlayerController>("Prefabs/PC_Server");
                if (playerControllerServer == null)
                    throw new NullReferenceException();
                
                // Спавн персонажа
                var playerEntity = Instantiate(playerEntityServerPrefab);
                playerEntity.gameObject.transform.position = SpawnPoint.SpawnPoints.Random().transform.position;
                
                // Спавн контроллера
                var playerController = Instantiate(playerControllerServer);
                playerController.gameObject.name = $"PC [SERVER]";
                
                playerController.SetPlayerEntity(playerEntity);
                playerController.SetPlayerEntityCamera();
            }
        }

        public void StopSession()
        {
            ServerLobby.OnUserReady -= ProcessReadyUser;
            ServerLobby.OnUserDisconnected -= ProcessDisconnectedUser;
            IsStarted = false;
            ServerLobby.ShareServerSessionForConnections(StateMessage);
        }
        
        private void UpdateObservers()
        {
            for (var i = 0; i < SessionObservers.Count; i++)
            {
                SessionObservers[i].Update(Time.deltaTime);
            }   
        }

        /// <summary>
        /// Обработка готового пользователя.
        /// </summary>
        private void ProcessReadyUser(UserConnection userConnection)
        {
            Debug.Log($"PROCESS_USER_READY: {userConnection.User.id} / {userConnection.User.name}");

            // Если пользователь уже на сцене, то ничего не делаем.
            if (userConnection.SceneState != UserConnection.UserSceneState.NotLoaded)
                return;
            
            Debug.Log("CHANGE SCENE FOR USER");
            
            // Добавить в серверную сцену автоматический редирект. !!!!!!!!!!!!!!!!
            ServerLobby.Scene.ChangeUserScene(userConnection);
        }
        
        /// <summary>
        /// Обработка загрузившегося пользователя.
        /// </summary>
        private void ProcessLoadedUser(UserConnection userConnection)
        {
            Debug.Log($"PROCESS_USER_LOADED: {userConnection.User.id} / {userConnection.User.name}");

            var userHandler = new UserHandler(userConnection);
            
            // Заносим в пользователей сцены.
            _userConnectionsInScene.Add(userHandler);
            
            // Спавним игрока
            var playerEntity = Instantiate(_playerEntityPrefab);
            playerEntity.gameObject.transform.position = SpawnPoint.SpawnPoints.Random().transform.position;
            playerEntity.owner = userConnection.User;
            NetworkFactory.SpawnForAll(playerEntity.gameObject, userConnection);
            
            // Спавн контроллера
            var playerController = Instantiate(_playerController);
            playerController.gameObject.name = $"PC [{playerEntity.owner.id}]";
            NetworkFactory.SpawnForConnection(playerController.gameObject, userConnection);
            playerController.SetPlayerEntity(playerEntity);
            playerController.playerEntityId = playerEntity.netId;

            userHandler.RelatedPlayerEntity = playerEntity;
            userHandler.AddGameObject(playerEntity.gameObject);
            userHandler.AddGameObject(playerController.gameObject);
            
            AddPlayerEntity(playerEntity);
        }

        /// <summary>
        /// Обработка отключившегося пользователя.
        /// </summary>
        private void ProcessDisconnectedUser(UserConnection uc)
        {
            Debug.Log("PROCESS_USER_DISCONNECTED");
            
            var userHandler = _userConnectionsInScene.FirstOrDefault(x => x.UserConnection == uc);
            
            if (userHandler != null)
            {
                if (userHandler.RelatedPlayerEntity != null)
                {
                    RemovePlayerEntity(userHandler.RelatedPlayerEntity);
                }
                
                userHandler.DisposeAsServer();
                _userConnectionsInScene.Remove(userHandler);
            }
        }
        
        public void AddPlayerEntity(PlayerEntity playerEntity)
        {
            var coincidence = PlayerEntities.FirstOrDefault(x=> x== playerEntity);
            
            // Он не был зарегестрирован.
            if (coincidence == null)
            {
                PlayerEntities.Add(playerEntity);
                OnPlayerEntityAdd(playerEntity);
            }
        }

        public void RemovePlayerEntity(PlayerEntity playerEntity)
        {
            var coincidence = PlayerEntities.FirstOrDefault(x=> x== playerEntity);

            // Он был зарегестрирован.
            if (coincidence != null)
            {
                PlayerEntities.Remove(playerEntity);
                OnPlayerEntityRemove(playerEntity);
            }
        }

        private void OnDestroy()
        {
            NetworkManager.OnServerSceneChangedEvent -= StartSession;
            ServerLobby.OnUserLoadedToScene -= ProcessLoadedUser;
        }
    }
}