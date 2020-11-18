using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Net
{
    public class EventNetworkManager : NetworkManager
    {
        // CLIENT
        
        /// <summary>
        /// Вызывается при успешном подключении к серверу.
        /// </summary>
        public event Action<NetworkConnection> OnConnectedToServer = delegate(NetworkConnection connection) {  };
        public event Action<NetworkConnection> OnDisconnectedFromServer = delegate(NetworkConnection connection) {  };
        /// <summary>
        /// Вызывается перед изменением сцены.
        /// </summary>
        public event Action OnClientChangeSceneEvent = delegate {  };
        /// <summary>
        /// Вызывается после смены сцены.
        /// </summary>
        public event Action OnClientSceneChangedEvent = delegate {  };
        
        // SERVER

        /// <summary>
        /// Вызывается при подключении нового клиента к серверу.
        /// </summary>
        public event Action<NetworkConnection> OnServerConnectEvent = delegate(NetworkConnection connection) {  };
        /// <summary>
        /// Вызывается при потере связи с клиентом.
        /// </summary>
        public event Action<NetworkConnection> OnServerDisconnectEvent = delegate(NetworkConnection connection) {  };
        /// <summary>
        /// Вызывается при готовности клиента продолжать связь.
        /// </summary>
        public event Action<NetworkConnection> OnServerReadyEvent = delegate(NetworkConnection connection) {  };
        /// <summary>
        /// Вызывается при старте сервера на текущем пк.
        /// </summary>
        public event Action OnServerStarted = delegate {  };
        /// <summary>
        /// Вызывается при остановке сервера.
        /// </summary>
        public event Action OnServerStopped = delegate {  };
        /// <summary>
        /// Вызывается после смены сцены на сервере.
        /// </summary>
        public event Action OnServerSceneChangedEvent = delegate {  };
        
        // CLIENT
        
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            OnConnectedToServer(conn);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            OnDisconnectedFromServer(conn);
        }

        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation,
            bool customHandling)
        {
            OnClientChangeSceneEvent();
            base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);
            OnClientSceneChangedEvent();
        }

        // SERVER

        public override void OnStartServer()
        {
            base.OnStartServer();
            OnServerStarted();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            OnServerStopped();
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            OnServerConnectEvent(conn);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            OnServerDisconnectEvent(conn);
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            OnServerReadyEvent(conn);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
            OnServerSceneChangedEvent();
        }
        
        
        
        
        
        
        
        
        
        
        
        
        /// <summary>
        /// Меняет сцену сервера и только заданным пользователям.
        /// </summary>
        public virtual void ServerChangeSceneWith(string newSceneName, IEnumerable<NetworkConnection> connections)
        {
            if (string.IsNullOrEmpty(newSceneName))
            {
                logger.LogError("ServerChangeSceneWith empty scene name");
                return;
            }
            
            if (Application.CanStreamedLevelBeLoaded(newSceneName) == false)
            {
                Debug.Log($"Scene \"{newSceneName}\" not exist in the current build!" +
                          $" But you are trying to access it!");
                return;
            }

            if (logger.logEnabled) logger.Log("ServerChangeScene " + newSceneName);

            foreach (var networkConnection in connections)
            {
                NetworkServer.SetClientNotReady(networkConnection);
            }
            networkSceneName = newSceneName;

            // Let server prepare for scene change
            OnServerChangeScene(newSceneName);

            // Suspend the server's transport while changing scenes
            // It will be re-enabled in FinishScene.
            Transport.activeTransport.enabled = false;

            loadingSceneAsync = SceneManager.LoadSceneAsync(newSceneName);

            // ServerChangeScene can be called when stopping the server
            // when this happens the server is not active so does not need to tell clients about the change
            if (NetworkServer.active)
            {
                foreach (var networkConnection in connections)
                {
                    networkConnection.Send(new SceneMessage {sceneName = newSceneName});
                }
            }

            startPositionIndex = 0;
            startPositions.Clear();
        }

        /// <summary>
        /// Меняет сцену у пользователя на текщую сцену сервера.
        /// </summary>
        public virtual void ServerChangeSceneFor(NetworkConnection connection)
        {
            NetworkServer.SetClientNotReady(connection);
            
            connection.Send(new SceneMessage {sceneName = networkSceneName});
        }
    }
}