using System;
using System.Collections;
using Egsp.Core.Ui;
using Game.Net;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Presenters
{
    public class ServerInLobbyPresenter : SerializedPresenter<ServerInLobbyView, ServerLobby>
    {
        public override string Key => "server_lobby";

        private void ClientConnected()
        {
            // View.SetStatus("Someone connected!");
        }

        private void LoadScene(string sceneName)
        {
            Model.Scene.ChangeServerScene(sceneName);
        }

        private void ToggleSpawnServerController(bool toggleState)
        {
            Model.Session.SpawnServerController = toggleState;
        }

        private void Shutdown()
        {
            Model.Shutdown();
            
            View.ClearUsers();
            View.Disable();
            
            View.OnShutdown -= Shutdown;
            View.OnLoadScene -= LoadScene;

            Model.OnClientConnected -= ClientConnected;
            Model.OnUserConnected -= AddUser;
            Model.OnUserDisconnected -= RemoveUser;
            
            PresenterMediator.Request(this,"server_menu");
        }

        private void AddUser(User user)
        {
            View.AddUser(user);
        }

        private void RemoveUser(UserConnection userConnection)
        {
            View.RemoveUser(userConnection.User);
        }

        protected override void Dispose()
        {
            View.OnShutdown -= Shutdown;
            View.OnLoadScene -= LoadScene;
            View.spawnControllerToggle.onValueChanged.RemoveListener(ToggleSpawnServerController);

            if (Model != null)
            {
                Model.OnClientConnected -= ClientConnected;
                Model.OnUserConnected -= AddUser;
                Model.OnUserDisconnected -= RemoveUser;
            }
        }

        public override void OnAwake()
        {
            Model = ServerLobby.Instance;
            if(Model == null)
                throw new NullReferenceException();
            
            if(View == null)
                throw new NullReferenceException();
            
            
            View.OnShutdown += Shutdown;
            View.OnLoadScene += LoadScene;
            View.spawnControllerToggle.onValueChanged.AddListener(ToggleSpawnServerController);

            var isOnToggle = View.spawnControllerToggle.isOn;
            if (isOnToggle)
            {
                ToggleSpawnServerController(true);
            }

            Model.OnClientConnected += ClientConnected;
            Model.OnUserConnected += AddUser;
            Model.OnUserDisconnected += RemoveUser;
        }

        protected override void OnResponse()
        {
            // Debug.Log("RESPONSE");

            View.Enable();
            View.SetStatus("Server has started..");
        }
    }
}