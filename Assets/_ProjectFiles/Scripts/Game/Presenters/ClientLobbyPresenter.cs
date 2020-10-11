using System;
using Game.Net;
using Gasanov.Core;
using Gasanov.Core.Mvp;
using Gasanov.Extensions.Mono;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Presenters
{
    public class ClientLobbyPresenter : SerializedSingleton<ClientLobbyPresenter>, IPresenter<ClientLobbyView,ClientLobby>
    {
        [OdinSerialize]
        public ClientLobbyView View { get; private set; }
        [OdinSerialize]
        public ClientLobby Model { get; private set; }
        
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
            View.OnDisconnect += DisconnectInput;
            
            Model.OnAddUser += (x) => View.AddUser(x);
            Model.OnDisconnectUser += (x) => View.RemoveUser(x);
            Model.OnDisconnect += Disconnect;
        }

        /// <summary>
        /// Пользователь запросил отключение.
        /// </summary>
        public void DisconnectInput()
        {
            Model.Disconnect();
        }

        /// <summary>
        /// Отключение произошло по техниеским причинам.
        /// </summary>
        private void Disconnect()
        {
            GoToMenu();
        }

        private void GoToMenu()
        {
            View.ClearUsers();
            View.Disable();
            
            PresenterMediator.Request(this, "main_menu", null);
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
            if (key == "client_lobby")
            {
                View.Enable();
                View.SetStatus("Connected: Online!");
                return true;
            }

            return false;
        }

    }
}