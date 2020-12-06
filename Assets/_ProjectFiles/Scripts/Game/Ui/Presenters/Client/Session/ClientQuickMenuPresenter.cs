using System;
using Game.Entities.Controllers;
using Game.Net;
using Game.Sessions;
using Game.Views.Client.Session;
using Gasanov.Core.Mvp;

namespace Game.Presenters.Session
{
    public class ClientQuickMenuPresenter : SerializedPresenter<ClientQuickMenuView, ClientLobby>
    {
        public override string Key => "quick_menu";
        public override void OnAwake()
        {
            Model = ClientLobby.Instance;
            if(Model == null)
                throw new NullReferenceException();
            
            View.Disable();
            View.OnResume += Resume;
            View.OnExit += Exit;
        }
        
        // Активно ли сейчас меню.
        public bool IsActive { get; private set; }

        protected override void OnResponse()
        {
            View.Enable();
        }

        protected override void Dispose()
        {
            View.Disable();
            View.OnResume -= Resume;
            View.OnExit -= Exit;
        }

        private void Resume()
        {
            View.Disable();
            GlobalInput.SetCharacterMode();
        }

        private void Exit()
        {
            View.Disable();
            Model.Disconnect();
        }

        public void Activate()
        {
            View.Enable();
            IsActive = true;
        }

        public void Deactivate()
        {
            View.Disable();
            IsActive = false;
        }
    }
}