using System;
using Game.Entities;
using Game.World;
using Gasanov.Extensions.Linq;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Net
{
    public class ServerSession : SerializedMonoBehaviour
    {
        [NonSerialized] public EventNetworkManager NetworkManager;
        [NonSerialized] public ServerLobby ServerLobby;
        
        /// <summary>
        /// Началась ли сессия.
        /// </summary>
        public bool IsStarted { get; private set; }

        private PlayerEntity _playerEntityPrefab;

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

        private void Start()
        {
            _playerEntityPrefab = Resources.Load<PlayerEntity>("Prefabs/Player");
            
            if(_playerEntityPrefab == null)
                throw new NullReferenceException();
            
           
        }

        public void StartSession()
        {
            ServerLobby.OnUserReady += ProcessReadyUser;
            IsStarted = true;
            ServerLobby.ShareServerSessionForConnections(StateMessage);
        }

        public void StopSession()
        {
            ServerLobby.OnUserReady -= ProcessReadyUser;
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
            
            var inst = Instantiate(_playerEntityPrefab);
            inst.gameObject.transform.position = SpawnPoint.SpawnPoints.Random().transform.position;
            inst.userName = uc.User.name;
            NetworkServer.SpawnFor(inst.gameObject, uc.Connection);
        }
    }
}