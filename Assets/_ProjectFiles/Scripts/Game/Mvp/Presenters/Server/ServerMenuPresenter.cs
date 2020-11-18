using Game.Net;
using Game.Views.Server;
using Gasanov.Core.Mvp;
using UnityEngine;

namespace Game.Presenters.Server
{
    public class ServerMenuPresenter : SerializedPresenter<ServerMenuView, ServerLobby>
    {
        public override string Key => "server_menu";

        private void StartServer()
        {
            var port = View.portInput.text;
            
            Model.StartServer(port);
            
            Dispose();
            
            // Debug.Log("REQUEST");
            PresenterMediator.Request(this,"server_lobby");
        }

        private void OpenSetup()
        {
            View.CloseMenuWindow();
            View.ShowStartSetupWindow();
        }

        private void OpenMenu()
        {
            View.CloseStartSetupWindow();
            View.ShowMenuWindow();
        }
        
        private void Exit()
        {
            GameQuither.Exit();
        }

        public override void OnAwake()
        {
            Model = ServerLobby.Instance;

            View.Enable();
            View.OnExitButton += Exit;
            View.OnStartButton += StartServer;
            View.OnBackToMenuButton += OpenMenu;
            View.OnStartSetupButton += OpenSetup;
        }

        protected override void OnResponse()
        {
            OpenMenu();
        }

        private void Dispose()
        {
            View.Disable();
            View.OnExitButton -= Exit;
            View.OnStartButton -= StartServer;
            View.OnBackToMenuButton -= OpenMenu;
            View.OnStartSetupButton -= OpenSetup;
        }
    }
}