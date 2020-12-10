using System;
using Game.Entities.Modules;
using Mirror;
using UnityEngine;

namespace Game.Net.Objects
{
    /// <summary>
    /// Объект типа NetworkBehaviour с разделенной логикой (клиент-сервер).
    /// Для написания общей логики для клиента или сервера нужно переопределить базовые методы Unity.
    /// При переопределении обязательно нужно вызывать базовое определение методов.
    /// </summary>
    public abstract class DualNetworkBehaviour : NetworkBehaviour, IDualObject, IUnityMethodsHook
    {
        private DualUpdateManager _cachedUpdateManager;
        
        protected virtual void Awake()
        {
            // Кешируем значение менеджера.
            _cachedUpdateManager = DualUpdateManager.Instance;
            _cachedUpdateManager.AwakeMe(this);

            // Debug.Log("AWAKE DUAL");
            OnAwakeEvent();
        }

        protected virtual void Start()
        {
            _cachedUpdateManager.StartMe(this);

            OnStartEvent();
        }

        protected virtual void Update()
        {
            _cachedUpdateManager.UpdateMe(this);

            OnUpdateEvent();
        }

        protected virtual void FixedUpdate()
        {
            _cachedUpdateManager.FixedUpdateMe(this);

            OnFixedUpdateEvent();
        }
        
        protected virtual void LateUpdate()
        {
            _cachedUpdateManager.LateUpdateMe(this);
        }

        protected virtual void OnEnable()
        {
            _cachedUpdateManager.EnableMe(this);

            OnEnableEvent();
        }

        protected virtual  void OnDisable()
        {    
            _cachedUpdateManager.DisableMe(this);

            OnDisableEvent();
        }

        #region Dual event methods

        public virtual void UpdateOnClient()
        {
        }

        public virtual void UpdateOnServer()
        {
        }

        public virtual void FixedUpdateOnClient()
        {
        }

        public virtual void FixedUpdateOnServer()
        {
        }

        public virtual void LateUpdateOnClient()
        {
        }

        public virtual void LateUpdateOnServer()
        {
        }

        public virtual  void AwakeOnClient()
        {
        }

        public virtual  void AwakeOnServer()
        {
        }

        public virtual  void StartOnClient()
        {
        }

        public  virtual void StartOnServer()
        {
        }

        public virtual  void EnableOnClient()
        {
        }

        public  virtual void EnableOnServer()
        {
        }

        public virtual void DisableOnClient()
        {
        }

        public virtual void DisableOnServer()
        {
        }

        #endregion

        #region IUnityMethodsHook

        public event Action OnAwakeEvent = delegate {  };
        public event Action OnStartEvent = delegate {  };
        public event Action OnEnableEvent = delegate {  };
        public event Action OnDisableEvent = delegate {  };
        public event Action OnUpdateEvent = delegate {  };
        public event Action OnFixedUpdateEvent = delegate {  };

        #endregion
    }
}