using System;
using Egsp.Core.Ui;
using UnityEngine;

namespace Game.Views
{
    public class GameStarterView : MonoBehaviour, IView
    {
        public event Action OnClientLaunch = delegate {  };
        public event Action OnServerLaucnh = delegate {  };
        
        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void LaunchAsClient()
        {
            OnClientLaunch();
        }

        public void LaunchAsServer()
        {
            OnServerLaucnh();
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}