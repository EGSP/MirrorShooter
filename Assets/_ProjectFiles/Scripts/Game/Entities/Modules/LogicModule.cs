using System;
using System.Collections.Generic;
using Game.Entities.States;
using Game.Net.Objects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entities.Modules
{
    public abstract class LogicModule : IDualObject, IDisposable
    {
        public event Action<string,string> OnStateChanged = delegate(string s, string s1) {  };
        
        
        protected DualUpdateManager _cachedUpdateManager;

        private IUnityMethodsHook _hook;

        [SerializeField][ReadOnly] protected string currentStateName;

        /// <summary>
        /// Возвращает идентификатор модуля.
        /// </summary>
        public virtual string ID => this.GetType().Name;
        public string CachedId { get; protected set; }

        public virtual void Initialize(IUnityMethodsHook hook)
        {
            CachedId = ID;
            
            _cachedUpdateManager = DualUpdateManager.Instance;
            _hook = hook;

            AcceptHook(hook);
        }

        protected void StateChanged(string module, string state)
        {
            OnStateChanged(module, state);
        }
        
        protected abstract void DefineStates();

        public abstract void RecognizeState(string key);

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


        #region Unity methods

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

        private void LateUpdate()
        {
            _cachedUpdateManager.LateUpdateMe(this);
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

        public virtual void LateUpdateOnClient()
        {
        }

        public virtual void LateUpdateOnServer()
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
        
        #endregion

        public virtual void Dispose()
        {
            DenyHook();
        }
    }
    
    
    public abstract class LogicModule<TState, TModule> : LogicModule
        where TState: LogicState<TState,TModule>
        where TModule: LogicModule<TState,TModule>
    {
        protected Dictionary<string, Func<TState>> DefinedStates;

        protected TState CurrentState;

        public override void Initialize(IUnityMethodsHook hook)
        {
            base.Initialize(hook);
            DefinedStates = new Dictionary<string, Func<TState>>();
            DefineStates();
        }

        protected void UpdateStateOnServer()
        {
            if (CurrentState != null)
            {
                currentStateName = CurrentState.GetType().Name;
                
                var newState = CurrentState.UpdateOnServer(Time.deltaTime);

                CheckState(newState, CurrentState);

                CurrentState = newState;
            }
        }

        protected void FixedUpdateStateOnServer()
        {
            if (CurrentState != null)
            {
                currentStateName = CurrentState.CachedId;
                
                var newState = CurrentState.FixedUpdateOnServer(Time.fixedDeltaTime);
                
                CheckState(newState, CurrentState);

                CurrentState = newState;
            }
        }

        protected void UpdateStateOnClient()
        {
            if (CurrentState != null)
            {
                currentStateName = CurrentState.CachedId;

                CurrentState = CurrentState.UpdateOnClient(Time.deltaTime);
            }
        }
        
        protected void FixedUpdateStateOnClient()
        {
            if (CurrentState != null)
            {
                currentStateName = CurrentState.CachedId;

                CurrentState = CurrentState.FixedUpdateOnClient(Time.deltaTime);
            }
        }

        private void CheckState(TState newState, TState oldState)
        {
            if (newState == null)
            {
                StateChanged(CachedId, "null");
                return;
            }
            
            if (oldState == null)
            {
                StateChanged(CachedId, newState.CachedId);
                return;
            }
            
            if (newState != oldState)
            {
                // Debug.Log($"NewState = {newState.CachedId}, CurrentState = {oldState.CachedId}");
                StateChanged(CachedId, newState.CachedId);
            }
        }
        
        public virtual void SetState(TState state)
        {
            Debug.Log($"NEW STATE ON {GetType().Name}");
            if (state == null)
                return;

            CurrentState = state;

            if (DualUpdateManager.IsServer)
                CheckState(CurrentState, null);
        }
        
        protected void DefineState(string key, Func<TState> creationMethod)
        {
            DefinedStates.Add(key, creationMethod);
        }

        public override void RecognizeState(string key)
        {
            if (DefinedStates.ContainsKey(key))
            {
                SetState(DefinedStates[key].Invoke());
            }
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