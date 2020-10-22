using System;
using Mirror;

namespace Game.Net.Objects
{
    
    public abstract class DualNetworkBehaviour : NetworkBehaviour, IDualObject
    {
        /// <summary>
        /// Активен ли объект для обновления состояния.
        /// </summary>
        protected bool IsActive { get; private set; }
        
        protected virtual void Awake()
        {
            DualUpdateManager.Instance.Add(this);       
        }

        protected void OnEnable()
        {
            IsActive = true;
        }

        protected void OnDisable()
        {
            IsActive = false;
        }

        public virtual void UpdateOnClient()
        {
            if (!IsActive)
                return;
            
            OnClient();
        }

        public virtual void UpdateOnServer()
        {
            if (!IsActive)
                return;
            
            OnServer();
        }

        /// <summary>
        /// Обновления состояния на стороне клиента.
        /// </summary>
        protected abstract void OnClient();

        /// <summary>
        /// Обновление состояние на стороне сервера.
        /// </summary>
        protected abstract void OnServer();
    }
}