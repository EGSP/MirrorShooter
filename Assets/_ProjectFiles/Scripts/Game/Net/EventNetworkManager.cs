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
        /// Вызывается при старте сервера на текущем пк.
        /// </summary>
        public event Action OnServerStarted = delegate {  };
        /// <summary>
        /// Вызывается при остановке сервера.
        /// </summary>
        public event Action OnServerStopped = delegate {  };
        
        
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            OnConnectedToServer(conn);
        }

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
    }
}