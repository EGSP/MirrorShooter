using System;
using Game.Entities;
using Game.Entities.Controllers;
using Game.Net;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Sessions
{
    public class ClientSession : SerializedMonoBehaviour
    {
        [NonSerialized] public EventNetworkManager NetworkManager;
        [NonSerialized] public ClientLobby ClientLobby;

        /// <summary>
        /// Префаб сущности игрока.
        /// </summary>
        private PlayerEntity _playerEntityPrefab;
        
        /// <summary>
        /// Префаб контроллера игрока.
        /// </summary>
        private PlayerController _playerController;
        
        private void Start()
        {
            _playerEntityPrefab = Resources.Load<PlayerEntity>("Prefabs/Player");
            if(_playerEntityPrefab == null)
                throw new NullReferenceException();

            _playerController = Resources.Load<PlayerController>("Prefabs/PC");
            if (_playerController == null)
                throw new NullReferenceException();
            
            ClientScene.RegisterPrefab(_playerEntityPrefab.gameObject);
            ClientScene.RegisterPrefab(_playerController.gameObject);
        }

        public void StartSession()
        {
            ClientLobby.OnDisconnect += StopSession;
        }

        public void StopSession()
        {
            ClientLobby.OnDisconnect -= StopSession;
            ClientLobby.ChangeScene(0);
        }
    }
}