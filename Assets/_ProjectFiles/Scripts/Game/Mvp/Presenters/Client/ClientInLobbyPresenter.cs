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
    public class ClientInLobbyPresenter : SerializedPresenter<ClientInLobbyView, ClientLobby>
    {
        public override string Key => "client_lobby";


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
            
            PresenterMediator.Request(this,"client_menu");
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
            // Debug.Log("Session presenter");
            View.ShowSession(msg);
        }

        private void Ready()
        {
            Model.ReadyForChangeScene();
        }

        protected override void Dispose()
        {
            View.OnDisconnect -= DisconnectInput;

            if (Model != null)
            {
                Model.OnAddUser -= AddUser;
                Model.OnDisconnectUser -= RemoveUser;
                Model.OnDisconnect -= Disconnect;
                Model.OnServerSession -= OnSessionChanged;
            }
        }

        public override void OnAwake()
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
        }

        protected override void OnResponse()
        {
            Model.GetServerSessionState();
            
            View.Enable();
            View.SetStatus("Connected: Online!");
        }

    }
}