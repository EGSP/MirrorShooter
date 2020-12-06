using System;
using Egsp.Core;
using Game.Configuration;
using Game.Net;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameStarter : SerializedSingleton<GameStarter>
    {
        protected override void Awake()
        {
            base.Awake();
            AlwaysExist = true;
        }

        public void StartAsClient()
        {
            LaunchInfo.LaunchMode = LaunchModeType.Client;

            SceneManager.sceneLoaded += InitializeClient;
            SceneManager.LoadScene("main_menu");
        }

        public void StartAsServer()
        {
            LaunchInfo.LaunchMode = LaunchModeType.Server;
            
            SceneManager.sceneLoaded += InitializeServer;
            SceneManager.LoadScene("main_menu");
        }

        private void InitializeClient(Scene scene, LoadSceneMode loadSceneMode)
        {
            ClientLobby.CreateInstance();
            var lobby = ClientLobby.Instance;
            
            var networkManager = NetworkManager.singleton as EventNetworkManager;
            if (networkManager == null)
                throw new InvalidCastException();
            
            lobby.NetworkManager = networkManager;
            lobby.Initialize();

            SceneManager.sceneLoaded -= InitializeClient;
            DestroyIfExist(true);
        }

        private void InitializeServer(Scene scene, LoadSceneMode loadSceneMode)
        {
            ServerLobby.CreateInstance();
            var lobby = ServerLobby.Instance;
            
            var networkManager = NetworkManager.singleton as EventNetworkManager;
            if (networkManager == null)
                throw new InvalidCastException();
            
            lobby.NetworkManager = networkManager;
            lobby.Initialize();
            
            SceneManager.sceneLoaded -= InitializeServer;
            DestroyIfExist(true);
        }
        
        // public event Action OnMainLobby = delegate {  };
        // public event Action OnFailedConnection = delegate {  };

        // private IEnumerator _checkRoutine;
        //
        // private Action _clearLastLobby;
        //
        // public void CleanUpLobbies()
        // {
        //     _clearLastLobby?.Invoke();
        // }
        //
        // public void CreateClientLobby()
        // {
        //     var lobby = ClientLobby.Instance;
        //     var networkManager = NetworkManager.singleton as EventNetworkManager;
        //     if (networkManager == null)
        //         throw new InvalidCastException();
        //
        //     lobby.NetworkManager = networkManager;
        //     lobby.Initialize();
        //     
        //     lobby.OnConnected += OnSuccessfulConnection;
        //     _clearLastLobby = () =>
        //     {
        //         Destroy(lobby.gameObject);
        //     };
        // }
        //
        // public void StartClient(string address, string port)
        // {
        //     var lobby = ClientLobby.Instance;
        //     lobby.ConnectToServer(address,port);
        //
        //     if(_checkRoutine != null)
        //         StopCoroutine(_checkRoutine);
        //     _checkRoutine = CheckClientConnection(lobby);
        //     StartCoroutine(_checkRoutine);
        // }
        //
        // private void OnSuccessfulConnection()
        // {
        //     OnClientLobby();
        // }
        //
        // public void CreateServerLobby()
        // {
        //     var lobby = ServerLobby.Instance;
        //     var networkManager = NetworkManager.singleton as EventNetworkManager;
        //     if (networkManager == null)
        //         throw new InvalidCastException();
        //
        //     lobby.NetworkManager = networkManager;
        //     lobby.Initialize();
        //
        //     lobby.OnStarted += OnServerStarted;
        //     _clearLastLobby = () =>
        //     {
        //         Destroy(lobby.gameObject);
        //     };
        // }

        // public void StartServer(string port)
        // {
        //     var lobby = ServerLobby.Instance;
        //     lobby.StartServer(port);
        // }
        //
        // private void OnServerStarted()
        // {
        //     OnServerLobby();
        // }
        //
        public void Exit()
        {
            Application.Quit();
        }
        //
        // public void StopCheckingConnection()
        // {
        //     if (_checkRoutine != null)
        //         StopCoroutine(_checkRoutine);
        // }
        //
        // public void StopClientConnection()
        // {
        //     ClientLobby.Instance.Disconnect();
        // }

        // private IEnumerator CheckClientConnection(ClientLobby lobby)
        // {
        //     yield return new WaitForSeconds(5f);
        //     Debug.Log($"Delayed: lobby status {lobby.isConnected}");
        //     if (!lobby.isConnected)
        //     {
        //         lobby.Disconnect();
        //         OnMainLobby();
        //         OnFailedConnection();
        //     }
        //
        //     _checkRoutine = null;
        // }
    }
}