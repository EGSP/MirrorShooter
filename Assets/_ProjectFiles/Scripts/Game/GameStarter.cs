using System;
using System.Collections;
using System.Threading.Tasks;
using Game.Net;
using Gasanov.Core;
using Mirror;
using UnityEngine;

namespace Game
{
    public class GameStarter : SerializedSingleton<GameStarter>
    {
        public event Action OnClientLobby = delegate {  };
        public event Action OnServerLobby = delegate {  };
        
        public event Action OnMainLobby = delegate {  };
        public event Action OnFailedConnection = delegate {  };

        private IEnumerator _checkRoutine;

        private Action _clearLastLobby;

        public void CleanUpLobbies()
        {
            _clearLastLobby?.Invoke();
        }
        
        public void CreateClientLobby()
        {
            var lobby = ClientLobby.Instance;
            var networkManager = NetworkManager.singleton as EventNetworkManager;
            if (networkManager == null)
                throw new InvalidCastException();

            lobby.NetworkManager = networkManager;
            lobby.Initialize();
            
            lobby.OnConnected += OnSuccessfulConnection;
            _clearLastLobby = () =>
            {
                Destroy(lobby.gameObject);
            };
        }

        public void StartClient(string address, string port)
        {
            var lobby = ClientLobby.Instance;
            lobby.ConnectToServer(address,port);

            if(_checkRoutine != null)
                StopCoroutine(_checkRoutine);
            _checkRoutine = CheckClientConnection(lobby);
            StartCoroutine(_checkRoutine);
        }

        private void OnSuccessfulConnection()
        {
            OnClientLobby();
        }

        public void CreateServerLobby()
        {
            var lobby = ServerLobby.Instance;
            var networkManager = NetworkManager.singleton as EventNetworkManager;
            if (networkManager == null)
                throw new InvalidCastException();

            lobby.NetworkManager = networkManager;
            lobby.Initialize();

            lobby.OnStarted += OnServerStarted;
            _clearLastLobby = () =>
            {
                Destroy(lobby.gameObject);
            };
        }

        public void StartServer(string port)
        {
            var lobby = ServerLobby.Instance;
            lobby.StartServer(port);
        }

        private void OnServerStarted()
        {
            OnServerLobby();
        }
        
        public void Exit()
        {
            Application.Quit();
        }

        public void StopCheckingConnection()
        {
            if (_checkRoutine != null)
                StopCoroutine(_checkRoutine);
        }
        
        public void StopClientConnection()
        {
            ClientLobby.Instance.Disconnect();
        }

        private IEnumerator CheckClientConnection(ClientLobby lobby)
        {
            yield return new WaitForSeconds(5f);
            Debug.Log($"Delayed: lobby status {lobby.isConnected}");
            if (!lobby.isConnected)
            {
                lobby.Disconnect();
                OnMainLobby();
                OnFailedConnection();
            }

            _checkRoutine = null;
        }
    }
}