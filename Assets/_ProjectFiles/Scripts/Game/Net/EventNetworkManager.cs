using System;
using Mirror;

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
        public event Action<NetworkConnection> OnClientConnected = delegate(NetworkConnection connection) {  };
        /// <summary>
        /// Вызывается при потере связи с клиентом.
        /// </summary>
        public event Action<NetworkConnection> OnClientDisconnected = delegate(NetworkConnection connection) {  };
        /// <summary>
        /// Вызывается при готовности клиента продолжать связь.
        /// </summary>
        public event Action<NetworkConnection> OnClientReady = delegate(NetworkConnection connection) {  };
        /// <summary>
        /// Вызывается при старте сервера на текущем пк.
        /// </summary>
        public event Action OnServerStarted = delegate {  };
        /// <summary>
        /// Вызывается при остановке сервера.
        /// </summary>
        public event Action OnServerStopped = delegate {  };
        
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
            OnClientConnected(conn);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            OnClientDisconnected(conn);
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            OnClientReady(conn);
        }
    }
}