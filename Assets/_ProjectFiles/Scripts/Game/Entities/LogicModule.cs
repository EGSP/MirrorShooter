using System;
using Game.Net.Objects;

namespace Game.Entities.Modules
{
    public class LogicModule : IDualObject, IDisposable
    {
        private DualUpdateManager _cachedUpdateManager;

        private IUnityMethodsHook _hook;
        
        public virtual void Initialize(IUnityMethodsHook hook)
        {
            _cachedUpdateManager = DualUpdateManager.Instance;
            _hook = hook;

            AcceptHook(hook);
        }

        public virtual void AcceptHook(IUnityMethodsHook hook)
        {
            hook.OnAwakeEvent += Awake;
            hook.OnStartEvent += Start;
            hook.OnUpdateEvent += Update;
            hook.OnFixedUpdateEvent += FixedUpdate;
            hook.OnEnableEvent += Enable;
            hook.OnDisableEvent += Disable;
        }

        /// <summary>
        /// Отписывается от всех событий хука, однако сам хук остается.
        /// </summary>
        public virtual void DenyHook()
        {
            if (_hook == null)
                return;
            
            _hook.OnAwakeEvent -= Awake;
            _hook.OnStartEvent -= Start;
            _hook.OnUpdateEvent -= Update;
            _hook.OnFixedUpdateEvent -= FixedUpdate;
            _hook.OnEnableEvent -= Enable;
            _hook.OnDisableEvent -= Disable;
        }

        private void Awake()
        {
            _cachedUpdateManager.AwakeMe(this);
        }

        private void Start()
        {
            _cachedUpdateManager.StartMe(this);
        }

        private void Update()
        {
            _cachedUpdateManager.UpdateMe(this);
        }

        private void FixedUpdate()
        {
            _cachedUpdateManager.FixedUpdateMe(this);
        }

        private void Enable()
        {
            AcceptHook(_hook);
            _cachedUpdateManager.EnableMe(this);
        }

        private void Disable()
        {
            DenyHook();
            _cachedUpdateManager.DisableMe(this);
        }

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

        public virtual void AwakeOnClient()
        {
        }

        public virtual void AwakeOnServer()
        {
        }

        public virtual void StartOnClient()
        {
        }

        public virtual void StartOnServer()
        {
        }

        public virtual void EnableOnClient()
        {
        }

        public virtual void EnableOnServer()
        {
        }

        public virtual void DisableOnClient()
        {
        }

        public virtual void DisableOnServer()
        {
        }

        public virtual void Dispose()
        {
            DenyHook();
        }
    }

    public interface IUnityMethodsHook
    {
        event Action OnAwakeEvent;
        event Action OnStartEvent;
        event Action OnEnableEvent;
        event Action OnDisableEvent;
        event Action OnUpdateEvent;
        event Action OnFixedUpdateEvent;
    }
    
}