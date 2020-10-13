using System;
using Game.Entities;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Net
{
    public class ClientSession : SerializedMonoBehaviour
    {
        [NonSerialized] public EventNetworkManager NetworkManager;
        [NonSerialized] public ClientLobby ClientLobby;

        private PlayerEntity _playerEntityPrefab;

        private void Start()
        {
            _playerEntityPrefab = Resources.Load<PlayerEntity>("Prefabs/Player");
            if(_playerEntityPrefab == null)
                throw new NullReferenceException();
            
            ClientScene.RegisterPrefab(_playerEntityPrefab.gameObject);
        }

        public void StartSession()
        {
            
        }

        public void StopSession()
        {
            
        }
    }
}