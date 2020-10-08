using System;
using Game.Net;
using Gasanov.Core;
using Gasanov.Core.Mvp;
using Gasanov.Extensions.Mono;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Presenters
{
    public class ServerLobbyPresenter : SerializedSingleton<ServerLobbyPresenter>, IPresenter<ServerLobbyView, ServerLobby>
    {
        [OdinSerialize]
        public ServerLobbyView View { get; private set; }
        [OdinSerialize]
        public ServerLobby Model { get; private set; }
        
        public event Action OnDispose;
        
        protected override void Awake()
        {
            base.Awake();

            Share();

            Model = this.ValidateComponent(Model);
            if(Model == null)
                throw new NullReferenceException();
            
            if(View == null)
                throw new NullReferenceException();
        }

        private void Start()
        {
            View.OnShutdown += Shutdown;

            Model.OnClientConnected += ClientConnected;
            Model.OnUserConnected += AddUser;
            Model.OnUserDisconnected += RemoveUser;
        }

        private void ClientConnected()
        {
            // View.SetStatus("Someone connected!");
        }

        private void Shutdown()
        {
            Model.Shutdown();
            
            View.ClearUsers();
            View.Disable();
            
            PresenterMediator.Request(this,"main_menu",null);
        }

        private void AddUser(User user)
        {
            View.AddUser(user);
        }

        private void RemoveUser(User user)
        {
            View.RemoveUser(user);
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Share()
        {
            PresenterMediator.Register(this);
        }

        public bool Response(string key, object arg)
        {
            if (key == "server_lobby")
            {
                View.Enable();
                View.SetStatus("Server has started..");
                return true;
            }

            return false;
        }
    }
}