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
            
            Dispose();
            PresenterMediator.Request(this, "main_menu", null);
        }

        private void AddUser(User user)
        {
            View.AddUser(user);
        }

        private void RemoveUser(User user)
        {
            View.RemoveUser(user);
        }

        private void OnSessionChanged(SessionStateMessage msg)
        {
            Debug.Log("Session presenter");
            View.ShowSession(msg);
        }

        private void Ready()
        {
            Model.Ready();
        }

        public void Dispose()
        {
            View.OnDisconnect -= DisconnectInput;

            if (Model != null)
            {
                Model.OnAddUser -= AddUser;
                Model.OnDisconnectUser -= RemoveUser;
                Model.OnDisconnect -= Disconnect;
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Share()
        {
            PresenterMediator.Register(this);
        }

        public bool Response(string key, object arg)
        {
            if (key == "client_lobby")
            {
                OnResponse();
                return true;
            }

            return false;
        }

        private void OnResponse()
        {
            Model = ClientLobby.Instance;
            if(Model == null)
                throw new NullReferenceException();
            
            if(View == null)
                throw new NullReferenceException();
            
            View.OnDisconnect += DisconnectInput;
            View.OnReady += Ready;
            
            Model.OnAddUser += AddUser;
            Model.OnDisconnectUser += RemoveUser;
            Model.OnDisconnect += Disconnect;
            Model.OnServerSession += OnSessionChanged;
            
            // Запрашиваем информацию о сессии.
            Model.GetServerSession();
            
            View.Enable();
            View.SetStatus("Connected: Online!");
        }

    }
}