using System;
using System.Collections;
using System.Threading.Tasks;
using Game.Net;
using Gasanov.Core;
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

        public void StartClient(string address, string port)
        {
            var lobby = ClientLobby.Instance;

            lobby.OnConnected += OnSuccessfulConnection;
            lobby.TryConnectToServer(address,port);

            if(_checkRoutine != null)
                StopCoroutine(_checkRoutine);
            _checkRoutine = CheckConnection(lobby);
            StartCoroutine(_checkRoutine);
        }

        private void OnSuccessfulConnection()
        {
            OnClientLobby();
        }

        public void StartServer(string port)
        {
            var lobby = ServerLobby.Instance;

            lobby.OnStarted += OnServerStarted;
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

        private IEnumerator CheckConnection(ClientLobby lobby)
        {
            yield return new WaitForSeconds(5f);
            Debug.Log($"Delayed: lobby status {lobby.isConnected}");
            if (!lobby.isConnected)
            {
                OnMainLobby();
                OnFailedConnection();
            }

            _checkRoutine = null;
        }
    }
}