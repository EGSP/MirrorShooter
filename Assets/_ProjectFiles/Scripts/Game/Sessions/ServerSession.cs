﻿using System;
using System.Collections.Generic;
using System.Linq;
using Game.Entities;
using Game.Entities.Controllers;
using Game.Net;
using Game.World;
using Gasanov.Extensions.Linq;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Sessions
{
    public class ServerSession : SerializedMonoBehaviour
    {
        [NonSerialized] public EventNetworkManager NetworkManager;
        [NonSerialized] public ServerLobby ServerLobby;
        
        /// <summary>
        /// Началась ли сессия.
        /// </summary>
        public bool IsStarted { get; private set; }

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
        /// Список пользователей находящихся на сцене.
        /// </summary>
        private List<UserHandler> _userConnectionsInScene;

        private void Start()
        {
            _playerEntityPrefab = Resources.Load<PlayerEntity>("Prefabs/Player");
            if(_playerEntityPrefab == null)
                throw new NullReferenceException();

            _playerController = Resources.Load<PlayerController>("Prefabs/PC");
            if (_playerController == null)
                throw new NullReferenceException();
            
            _userConnectionsInScene = new List<UserHandler>();
        }

        public void StartSession()
        {
            ServerLobby.OnUserReady += ProcessReadyUser;
            ServerLobby.OnUserDisconnected += ProcessDisconnectedUser;
            IsStarted = true;
            ServerLobby.ShareServerSessionForConnections(StateMessage);
        }

        public void StopSession()
        {
            ServerLobby.OnUserReady -= ProcessReadyUser;
            ServerLobby.OnUserDisconnected -= ProcessDisconnectedUser;
            IsStarted = false;
            ServerLobby.ShareServerSessionForConnections(StateMessage);
        }

       
        
        /// <summary>
        /// Смена сцены сервера.
        /// </summary>
        /// <param name="sceneName"></param>
        public void ChangeScene(string sceneName)
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName) == false)
            {
                Debug.Log($"Scene \"{sceneName}\" not exist in the current build!" +
                          $" But you are trying to access it!");
                return;
            }
            
            NetworkServer.RegisterHandler<SceneLoadedMessage>(ProcessLoadedUser);

            NetworkManager.OnServerSceneChangedEvent += StartSession;
            NetworkManager.ServerChangeSceneUsers(sceneName, ServerLobby.FullValReadyConnections);
        }

        /// <summary>
        /// Обработка готового пользователя.
        /// </summary>
        private void ProcessReadyUser(UserConnection uc)
        {
            Debug.Log("PROCESS_USER_READY");

            // Если пользователь уже на сцене, то ничего не делаем.
            if (_userConnectionsInScene.Exists(x=>x.UserConnection == uc))
                return;
            
            ServerLobby.ChangeUserScene(uc);
        }
        
        /// <summary>
        /// Обработка загрузившегося пользователя.
        /// </summary>
        private void ProcessLoadedUser(NetworkConnection conn, SceneLoadedMessage msg)
        {
            Debug.Log("PROCESS_USER_LOADED");

            var uc = ServerLobby.Val(conn);
            if (uc == null)
                return;

            if (_userConnectionsInScene.Exists(x=>x.UserConnection == uc))
                return;

            var userHandler = new UserHandler(uc);
            // Заносим в пользователей сцены.
            _userConnectionsInScene.Add(userHandler);
            
            // Спавним игрока
            var playerEntity = Instantiate(_playerEntityPrefab);
            playerEntity.gameObject.transform.position = SpawnPoint.SpawnPoints.Random().transform.position;
            playerEntity.owner = uc.User;
            NetworkServer.Spawn(playerEntity.gameObject);

            // Спавн контроллера
            var playerController = Instantiate(_playerController);
            playerController.gameObject.name = $"PC [{playerEntity.owner.id}]";
            NetworkServer.SpawnFor(playerController.gameObject, uc.Connection);
            playerController.SetPlayerEntity(playerEntity);
            playerController.playerEntityId = playerEntity.netId;
            
            NetworkIdentity.RebuildObserversForAll();
            
            userHandler.AddGameObject(playerEntity.gameObject);
            userHandler.AddGameObject(playerController.gameObject);
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
                userHandler.DisposeAsServer();
                _userConnectionsInScene.Remove(userHandler);
            }
        }
    }
}