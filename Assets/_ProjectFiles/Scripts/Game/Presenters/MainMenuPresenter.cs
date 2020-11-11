using System;
using Game.Configuration;
using Game.Net;
using Game.Views;
using Gasanov.Core;
using Gasanov.Core.Mvp;
using Gasanov.Extensions.Mono;
using Mirror;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Presenters
{
    public class MainMenuPresenter : SerializedSingleton<MainMenuPresenter>, IPresenter<MainMenuView, GameStarter>
    {
        [OdinSerialize]
        public MainMenuView View { get; set; }
        [OdinSerialize]
        public GameStarter Model { get; set; }
        
        public event Action OnDispose;

        protected override void Awake()
        {
            base.Awake();
            Model = this.ValidateComponent(Model);
            
            if(View == null)
                throw new NullReferenceException();
            
            Share();
        }

        private void Start()
        {
            View.Enable();

            View.OnLogin += Login;
            View.OnRelogin += ShowLoginWindow;
            View.OnClientMode += ShowClientWindow;
            View.OnServerMode += ShowServerWindow;

            View.OnConnect += Connect;
            View.OnConnectionQuit += () =>
            {
                Model.StopCheckingConnection();
                Model.StopClientConnection();
            };
            View.OnStartServer += StartServer;
            View.OnExit += ExitFromGame;

            Model.OnClientLobby += RequestClientLobby;
            Model.OnServerLobby += RequestServerLobby;
            Model.OnMainLobby += ShowSelectModeWindow;
            Model.OnFailedConnection += NotifyFailedConnection;
            
            ShowLoginWindow();
        }

        private void Login(string name)
        { 
            LaunchInfo.User = new User(name);
            
            View.ShowModeSelectWindow();
            View.ShowInfo($"Logged as \"{name}\"");
        }

        private void Connect(string address, string port)
        {
            Model.StartClient(address,port);
            View.SetStatus("Connecting");
        }

        private void ConnectionQuit()
        {
            Model.StopCheckingConnection();
            Model.StopClientConnection();
        }

        private void StartServer(string port)
        {
            Model.StartServer(port);
        }

        private void ShowLoginWindow()
        {
            Model.CleanUpLobbies();
            
            View.ClearInfo();
            View.ShowLoginWindow();
        }

        private void ShowSelectModeWindow()
        {
            View.ShowModeSelectWindow();
        }

        private void ShowClientWindow()
        {
            Model.CreateClientLobby();
            View.ShowConnectWindow();
        }

        private void ShowServerWindow()
        {
            Model.CreateServerLobby();
            View.ShowServerWindow();
        }

        private void NotifyFailedConnection()
        {
            View.SetStatus("Connection failed");
        }

        private void ExitFromGame()
        {
            Model.Exit();
        }
        
        public void Dispose()
        {
            View.OnLogin -= Login;
            View.OnRelogin -= ShowLoginWindow;
            View.OnClientMode -= ShowClientWindow;
            View.OnServerMode -= ShowServerWindow;

            View.OnConnect -= Connect;
            View.OnConnectionQuit -= ConnectionQuit;
            View.OnStartServer -= StartServer;
            View.OnExit -= ExitFromGame;

            Model.OnClientLobby -= RequestClientLobby;
            Model.OnServerLobby -= RequestServerLobby;
            Model.OnMainLobby -= ShowSelectModeWindow;
            Model.OnFailedConnection -= NotifyFailedConnection;
            
            PresenterMediator.Unregister(this);
        }

        private void RequestClientLobby()
        {
            Model.StopCheckingConnection();
            Dispose();
            PresenterMediator.Request(this, "client_lobby", null);
            View.Disable();
        }

        private void RequestServerLobby()
        {
            Dispose();
            PresenterMediator.Request(this, "server_lobby", null);
            View.Disable();
        }

        public void Share()
        {
            PresenterMediator.Register(this);
        }

        public bool Response(string key, object arg)
        {
            if (key == "main_menu")
            {
                OnResponse();
                return true;
            }

            return false;
        }

        private void OnResponse()
        {
            View.Enable();

            View.OnLogin += Login;
            View.OnRelogin += ShowLoginWindow;
            View.OnClientMode += ShowClientWindow;
            View.OnServerMode += ShowServerWindow;

            View.OnConnect += Connect;
            View.OnConnectionQuit += ConnectionQuit;
            View.OnStartServer += StartServer;
            View.OnExit += ExitFromGame;

            Model.OnClientLobby += RequestClientLobby;
            Model.OnServerLobby += RequestServerLobby;
            Model.OnMainLobby += ShowSelectModeWindow;
            Model.OnFailedConnection += NotifyFailedConnection;
            
            ShowSelectModeWindow();
        }
    }
}