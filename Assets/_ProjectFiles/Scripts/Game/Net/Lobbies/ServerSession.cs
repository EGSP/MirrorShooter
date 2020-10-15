using System;
using Game.Entities;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Net
{
    public class ServerSession : SerializedMonoBehaviour
    {
        [NonSerialized] public EventNetworkManager NetworkManager;
        [NonSerialized] public ServerLobby ServerLobby;

        private PlayerEntity _playerEntityPrefab;

        private void Start()
        {
            _playerEntityPrefab = Resources.Load<PlayerEntity>("Prefabs/Player");
            
            if(_playerEntityPrefab == null)
                throw new NullReferenceException();
        }

        public void StartSession()
        {
            
        }

        public void StopSession()
        {
            
        }
        
        public void ChangeScene(string sceneName)
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName) == false)
            {
                Debug.Log($"Scene \"{sceneName}\" not exist in the current build!" +
                          $" But you are trying to access it!");
                return;
            }

            NetworkManager.ServerChangeScene(sceneName);
            NetworkManager.OnClientReady += SpawnUser;
        }

        public void SpawnUser(NetworkConnection conn)
        {
            var uc = ServerLobby.Val(conn);

            if (uc != null)
            {
                var inst = Instantiate(_playerEntityPrefab);
                inst.userName = uc.User.name;
                NetworkServer.SpawnFor(inst.gameObject, conn);
            }
            else
            {
                Debug.Log("Поптыка создать игрока пользователем без регистрации!");
            }
        }
    }
}