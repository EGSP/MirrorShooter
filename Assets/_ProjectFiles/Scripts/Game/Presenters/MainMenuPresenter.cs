using System;
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
            View.OnServerMode += View.ShowServerWindow;

            View.OnConnect += Connect;
            View.OnConnectionQuit += () => Model.StopCheckingConnection();
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

        private void StartServer(string port)
        {
            Model.StartServer(port);
        }

        private void ShowLoginWindow()
        {
            View.ClearInfo();
            View.ShowLoginWindow();
        }

        private void ShowSelectModeWindow()
        {
            View.ShowModeSelectWindow();
        }

        private void ShowClientWindow()
        {
            View.ShowConnectWindow();
        }

        private void ShowServerWindow()
        {
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
            throw new NotImplementedException();
        }

        private void RequestClientLobby()
        {
            PresenterMediator.Request(this, "client_lobby", null);
            Model.StopCheckingConnection();
            View.Disable();
        }

        private void RequestServerLobby()
        {
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
                View.Enable();
                ShowSelectModeWindow();
                return true;
            }

            return false;
        }
    }
}