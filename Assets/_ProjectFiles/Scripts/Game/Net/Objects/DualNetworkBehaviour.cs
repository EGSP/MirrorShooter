using System;
using Game.Entities.Modules;
using Mirror;

namespace Game.Net.Objects
{
    
    public abstract class DualNetworkBehaviour : NetworkBehaviour, IDualObject, IUnityMethodsHook
    {
        private DualUpdateManager _cachedUpdateManager;
        
        private void Awake()
        {
            // Кешируем значение менеджера.
            _cachedUpdateManager = DualUpdateManager.Instance;
            _cachedUpdateManager.AwakeMe(this);

            OnAwakeEvent();
        }

        private void Start()
        {
            _cachedUpdateManager.StartMe(this);

            OnStartEvent();
        }

        private void Update()
        {
            _cachedUpdateManager.UpdateMe(this);

            OnUpdateEvent();
        }

        private void FixedUpdate()
        {
            _cachedUpdateManager.FixedUpdateMe(this);

            OnFixedUpdateEvent();
        }

        private void OnEnable()
        {
            _cachedUpdateManager.EnableMe(this);

            OnEnableEvent();
        }

        private void OnDisable()
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