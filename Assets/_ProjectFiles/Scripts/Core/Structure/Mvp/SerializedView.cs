using System;
using Game.Views;
using Sirenix.OdinInspector;

namespace Gasanov.Core.Mvp
{
    public abstract class SerializedView: SerializedMonoBehaviour, IView
    {
        public bool Unloading { get; set; }
        
        public virtual void Enable()
        {
            gameObject.SetActive(true);   
        }

        public virtual void Disable()
        {
            gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            if (!Unloading)
                ViewFactory.UnloadFromView(this);
        }
    }
}