using System;
using System.Collections.Generic;
using Egsp.Core;
using Game.Entities;
using Game.Entities.Controllers;
using Game.Net;
using Game.Net.Objects;
using Game.Net.Resources;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Sessions
{
    [LazyInstance(false)]
    public class ClientSession : SerializedSingleton<ClientSession>
    {
        [NonSerialized] public EventNetworkManager NetworkManager;
        [NonSerialized] public ClientLobby ClientLobby;

        private string offlineScene;

        protected override void OnInstanceCreated()
        {
            // base.OnInstanceCreated();
            
            NetworkManager = Mirror.NetworkManager.singleton as EventNetworkManager;
            ClientLobby = ClientLobby.Instance;
            
            NetworkResources.LoadAndRegister(new ClientSessionLoader());
            AlwaysExist = true;
        }

        public void StartSession()
        {
            ClientLobby.OnDisconnect += StopSession;
        }

        public void StopSession()
        {
            ClientLobby.OnDisconnect -= StopSession;
            ClientLobby.LoadMenuScene();
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
            
            var weaponPicker = Resources.Load<WeaponShop>("Prefabs/Weapons/weapon_picker");
            
            list.Add(playerEntityPrefab.gameObject);
            list.Add(playerController.gameObject);
            list.Add(weaponPicker.gameObject);
            
            return list;
        }
    }
}