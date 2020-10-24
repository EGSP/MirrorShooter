using System;
using Mirror;

namespace Game.Net.Objects
{
    
    public abstract class DualNetworkBehaviour : NetworkBehaviour, IDualObject
    {
        private DualUpdateManager _cachedUpdateManager;
        
        private void Awake()
        {
            // Кешируем значение менеджера.
            _cachedUpdateManager = DualUpdateManager.Instance;
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

        private void OnEnable()
        {
            _cachedUpdateManager.EnableMe(this);
        }

        private void OnDisable()
        {    
            _cachedUpdateManager.DisableMe(this);
        }

        #region Dual event methods

        public virtual void UpdateOnClient()
        {
        }

        public virtual void UpdateOnServer()
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

        // /// <summary>
        // /// Обновления состояния на стороне клиента.
        // /// </summary>
        // protected virtual void OnClientUpdate()
        // {
        // }
        //
        // /// <summary>
        // /// Обновление состояние на стороне сервера.
        // /// </summary>
        // protected virtual void OnServerUpdate()
        // {
        // }
        // protected virtual void OnClientAwake()
        // {
        // }
        //
        // protected virtual void OnServerAwake()
        // {
        // }
        //
        // protected virtual void OnClientStart()
        // {
        // }
        //
        // protected virtual void OnServerStart()
        // {
        // }
        //
        // protected virtual void OnClientEnable()
        // {
        // }
        //
        // protected virtual void OnServerEnable()
        // {
        // }
        //
        // protected virtual void OnClientDisable()
        // {
        // }
        //
        // protected virtual void OnServerDisable()
        // {
        // }
    }
}