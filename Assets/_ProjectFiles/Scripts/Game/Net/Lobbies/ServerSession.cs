using Mirror;
using UnityEngine;

namespace Game.Net
{
    public class ServerSession
    {
        public readonly EventNetworkManager NetworkManager;
        public ServerSession(EventNetworkManager networkManager)
        {
            NetworkManager = networkManager;
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
        }
    }
}