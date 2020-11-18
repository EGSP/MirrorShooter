using System;
using Game.Net;
using Game.Views.Client;
using Gasanov.Core.Mvp;

namespace Game.Presenters
{
    public class ClientMenuPresenter : SerializedPresenter<ClientMenuView, ClientLobby>
    {
        public override string Key => "client_menu";

        private void ConnectToServer()
        {
            var address = View.addressInput.text;
            var port = View.portInput.text;

            var nickname = View.nicknameInput.text;
            
            Model.LoginAs(new User(nickname));

            Model.ConnectToServer(address, port);
        }
        
        private void OpenSetup()
        {
            View.CloseMenuWindow();
            View.ShowConnectionSetupWindow();
        }

        private void OpenMenu()
        {
            View.CloseConnectionSetupWindow();
            View.ShowMenuWindow();
        }

        private void Exit()
        {
            GameQuither.Exit();
        }

        private void OnConnectedToServer()
        {
            View.Disable();
            Dispose();
            PresenterMediator.Request(this,"client_lobby");
        }

        public override void OnAwake()
        {
            Model = ClientLobby.Instance;

            Model.OnConnected += OnConnectedToServer;
            
            View.OnConnectButton += ConnectToServer;
            View.OnBackToMenuButton += OpenMenu;
            View.OnExitButton += Exit;
            View.OnConnectSetupButton += OpenSetup;
        }

        protected override void OnResponse()
        {
            OpenMenu();
        }

        private void Dispose()
        {
            Model.OnConnected -= OnConnectedToServer;
            
            View.OnConnectButton -= ConnectToServer;
            View.OnBackToMenuButton -= OpenMenu;
            View.OnExitButton -= Exit;
            View.OnConnectSetupButton -= OpenSetup;
        }
    }
}