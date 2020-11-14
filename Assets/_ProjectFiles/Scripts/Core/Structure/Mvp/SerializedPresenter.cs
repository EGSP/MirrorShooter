using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Gasanov.Core.Mvp
{
    public abstract class SerializedPresenter<TView, TModel> : SerializedMonoBehaviour,
        IPresenter<TView, TModel> where TView : IView
    {
        [OdinSerialize] public bool GetViewFromObject { get; protected set; } = true;
        
        [OdinSerialize] public TView View { get; protected set; }
        [OdinSerialize] public TModel Model { get; protected set; }

        public abstract string Key { get; }

        protected virtual void Awake()
        {
            if (GetViewFromObject)
            {
                View = GetComponent<TView>();
            }
        }

        public void Share()
        {
            PresenterMediator.Register(this);
        }

        public bool Response(string key, object arg)
        {
            if (key == Key)
            {
                OnResponse();
                return true;
            }
            
            return false;
        }
        
        protected abstract void OnResponse();

        protected virtual void OnDestroy()
        {
            PresenterMediator.Unregister(this);
        }
        
    }
}