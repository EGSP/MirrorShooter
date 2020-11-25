using System;
using System.Collections.Generic;
using System.Linq;
using Game.Entities;
using Game.Entities.Controllers;
using Game.Net;
using Game.Net.Objects;
using Game.Sessions.Actors;
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
        /// <summary>
        /// Вызывается при добавлении новой сущности игрока.
        /// </summary>
        public event Action<PlayerEntity> OnPlayerEntityAdd = delegate(PlayerEntity entity) {  };
        /// <summary>
        /// Вызывается при удалении сущности игрока.
        /// </summary>
        public event Action<PlayerEntity> OnPlayerEntityRemove = delegate(PlayerEntity entity) {  };

        public event Action<UserHandler> OnUserLoaded = delegate(UserHandler handler) {  };
        public event Action<UserHandler> OnUserDisconnected = delegate(UserHandler handler) {  };
        
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
        public PlayerEntity PlayerEntityPrefab { get; private set; }

        /// <summary>
        /// Префаб контроллера игрока.
        /// </summary>
        public PlayerController PlayerController { get; private set; }

        /// <summary>
        /// Список пользователей находящихся на сцене.
        /// </summary>
        public List<UserHandler> UserHandlers { get; private set; } = new List<UserHandler>();
        /// <summary>
        /// Все текущие сущности игроков.
        /// </summary>
        public List<PlayerEntity> PlayerEntities { get; private set; } = new List<PlayerEntity>();
        public List<SessionObserver> Observers { get; private set; } = new List<SessionObserver>();
        public List<Actor> Actors { get; private set; } = new List<Actor>();

        protected override void OnInstanceCreated()
        {
            base.OnInstanceCreated();
            AlwaysExist = true;
            
            NetworkManager = Mirror.NetworkManager.singleton as EventNetworkManager;
            ServerLobby = ServerLobby.Instance;
            
            NetworkManager.OnServerSceneChangedEvent += StartSession;
            ServerLobby.OnUserLoadedToScene += ProcessLoadedUser;
            
            CreateObservers(Observers);
            CreateActors(Actors);
            
            LoadPrefabs();
        }

        private void LoadPrefabs()
        {
            PlayerEntityPrefab = Resources.Load<PlayerEntity>("Prefabs/Player");
            if(PlayerEntityPrefab == null)
                throw new NullReferenceException();

            PlayerController = Resources.Load<PlayerController>("Prefabs/PC");
            if (PlayerController == null)
                throw new NullReferenceException();
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
            for (var i = 0; i < Observers.Count; i++)
            {
                Observers[i].Update(Time.deltaTime);
            }   
        }

        private void CreateObservers(List<SessionObserver> observers)
        {
            observers.Add(new PlayerTransformObserver(this));
        }

        private void CreateActors(List<Actor> actors)
        {
            var playerCharactersActor = new PlayerCharactersActor(this);
            var weaponsActor = new WeaponsActor(this, playerCharactersActor);
            
            actors.Add(playerCharactersActor);
            actors.Add(weaponsActor);
        }

        /// <summary>
        /// Обработка готового пользователя.
        /// </summary>
        private void ProcessReadyUser(UserConnection userConnection)
        {
            // Debug.Log($"PROCESS_USER_READY: {userConnection.User.id} / {userConnection.User.name}");

            // Если пользователь уже на сцене, то ничего не делаем.
            if (userConnection.SceneState != UserConnection.UserSceneState.NotLoaded)
                return;
            
            // Debug.Log("CHANGE SCENE FOR USER");
            
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
            UserHandlers.Add(userHandler);

            OnUserLoaded(userHandler);
        }

        /// <summary>
        /// Обработка отключившегося пользователя.
        /// </summary>
        private void ProcessDisconnectedUser(UserConnection uc)
        {
            // Debug.Log("PROCESS_USER_DISCONNECTED");
            
            var userHandler = UserHandlers.FirstOrDefault(x => x.UserConnection == uc);
            
            if (userHandler != null)
            {
                if (userHandler.RelatedPlayerEntity != null)
                {
                    RemovePlayerEntity(userHandler.RelatedPlayerEntity);
                }
                
                OnUserDisconnected(userHandler);
                userHandler.DisposeAsServer();
                UserHandlers.Remove(userHandler);
            }
        }
        
        // Эти методы не особо нужны. Они создавались только для поддержки серверной сущности.
        
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

        public bool GetUserHandler(PlayerEntity playerEntity, out UserHandler userHandler)
        {
            userHandler = null;
            var coincidence = UserHandlers.FirstOrDefault(x =>
                x.Id == playerEntity.owner.id);

            if (coincidence == null)
                return false;

            userHandler = coincidence;
            return true;
        }



        private void OnDestroy()
        {
            // ДОБАВИТЬ РАСШИРЕНИЯ ДЛЯ КОЛЛЕКЦИЙ DISPOSEALL !!!!!!!!!!!
            
            NetworkManager.OnServerSceneChangedEvent -= StartSession;
            ServerLobby.OnUserLoadedToScene -= ProcessLoadedUser;
        }
    }
}