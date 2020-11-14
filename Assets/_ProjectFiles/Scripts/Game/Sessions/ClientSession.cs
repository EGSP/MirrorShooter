using System;
using System.Collections.Generic;
using Game.Entities;
using Game.Entities.Controllers;
using Game.Net;
using Game.Net.Resources;
using Gasanov.Core;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Sessions
{
    [LazyInstance(false)]
    public class ClientSession : SerializedMonoBehaviour
    {
        [NonSerialized] public EventNetworkManager NetworkManager;
        [NonSerialized] public ClientLobby ClientLobby;

        private string offlineScene;
        
        public void StartSession()
        {
            ClientLobby.OnDisconnect += StopSession;
        }

        public void StopSession()
        {
            ClientLobby.OnDisconnect -= StopSession;
            ClientLobby.ChangeScene(Preloader.Instance.OfflineScene);
        }

        public List<GameObject> LoadPrefabs()
        {
            throw new NotImplementedException();
        }
    }
    
    public class ClientSessionLoader : IResourceLoader
    {
        public List<GameObject> LoadPrefabs()
        {
            var list = new List<GameObject>();
            
            var playerEntityPrefab = Resources.Load<PlayerEntity>("Prefabs/Player");
            if(playerEntityPrefab == null)
                throw new NullReferenceException();

            var playerController = Resources.Load<PlayerController>("Prefabs/PC");
            if (playerController == null)
                throw new NullReferenceException();
            
            list.Add(playerEntityPrefab.gameObject);
            list.Add(playerController.gameObject);
            
            return list;
        }
    }
}