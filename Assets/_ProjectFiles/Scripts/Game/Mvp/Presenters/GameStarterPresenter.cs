using System;
using Game.Views;
using Gasanov.Core.Mvp;
using UnityEngine;

namespace Game.Presenters
{
    public class GameStarterPresenter : SerializedPresenter<GameStarterView, GameStarter>
    {
        private void Awake()
        {   
            View.OnClientLaunch += OnClientLaunch;
            View.OnServerLaucnh += OnServerLaunch;
            OnAwake();
        }

        private void OnClientLaunch()
        {
            Model.StartAsClient();   
        }

        private void OnServerLaunch()
        {
            Model.StartAsServer();
        }

        public override string Key => "gamestarter";

        protected override void Dispose()
        {
            View.OnClientLaunch -= OnClientLaunch;
            View.OnServerLaucnh -= OnServerLaunch;
        }

        public override void OnAwake()
        {
            
        }

        protected override void OnResponse()
        {
            throw new System.NotImplementedException();
        }
    }
}