using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace Game.Net.Mirrors
{
    /// <summary>
    /// Используется на стороне сервера.
    /// </summary>
    public class UserSceneStateVisibility : NetworkVisibility
    {
        public ServerLobby Lobby => ServerLobby.Instance;

        private void Awake()
        {
            Lobby.OnUserLoadedToScene += AddObserverByLoadEvent;
        }

        private void OnDestroy()
        {
            Lobby.OnUserLoadedToScene -= AddObserverByLoadEvent;
        }

        // Метод будет вызван сервером при спавне,
        // соответственно пользователь к этому времени может быть не загружен на сцену.
        public override bool OnCheckObserver(NetworkConnection conn)
        {
            var checkResult = CheckConnection(conn);
            // Debug.Log($"Check observer with connection ID {conn.connectionId} result : {checkResult}");
            return checkResult;
        }

        private void AddObserverByLoadEvent(UserConnection userConnection)
        {
            var checkResult = CheckConnection(userConnection);
            if(checkResult == true)
                netIdentity.AddObserver(userConnection.Connection);
        }

        private bool CheckConnection(NetworkConnection conn)
        {
            var uc = Lobby.Val(conn);

            if (uc != null)
            {
                if (uc.SceneState == UserConnection.UserSceneState.Loaded)
                    return true;
            }

            return false;
        }
        
        private bool CheckConnection(UserConnection userConnection)
        {
            if (userConnection != null)
            {
                if (userConnection.SceneState == UserConnection.UserSceneState.Loaded)
                    return true;
            }

            return false;
        }

        public override void OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
        {
            var loadedUsers = Lobby.ReadyUsers
                .Where(x => x.SceneState == UserConnection.UserSceneState.Loaded);

            foreach (var userConnection in loadedUsers)
            {
                observers.Add(userConnection.Connection);
            }
        }
        
       
    }
}