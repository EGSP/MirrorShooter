using System;
using Gasanov.Core.Mvp;

namespace Game.Views.Client.Session
{
    public class WeaponPickerMenu : SerializedView
    {
        public event Action<string> OnWeaponChoosed = delegate(string s) {  };
        
        public event Action OnClose = delegate {  };
        
        
    }
}