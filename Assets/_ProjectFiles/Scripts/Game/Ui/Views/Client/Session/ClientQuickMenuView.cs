using System;
using Gasanov.Core.Mvp;
using Gasanov.Core.Pooling;

namespace Game.Views.Client.Session
{
    public class ClientQuickMenuView : SerializedView
    {
        public event Action OnResume = delegate {  };
        public event Action OnExit = delegate {  };
        
        
        public void Resume()
        {
            OnResume();
        }

        public void Exit()
        {
            OnExit();
        }
    }
    
    
}