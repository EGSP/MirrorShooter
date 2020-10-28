using System;
using System.Collections.Generic;
using Game.Configuration;
using Gasanov.Core;
using Mirror;

namespace Game.Net.Objects
{
    public class DualUpdateManager : Singleton<DualUpdateManager>
    {
        #region Objects

        public enum UpdateModeType
        {
            None,
            Client,
            Server
        }

        private abstract class AbstractUpdateMode
        {
            public abstract void Awake(IDualObject dualObject);

            public abstract void Start(IDualObject dualObject);

            public abstract void Update(IDualObject dualObject);

            public abstract void FixedUpdate(IDualObject dualObject);

            public abstract void LateUpdate(IDualObject dualObject);

            public abstract void Enable(IDualObject dualObject);

            public abstract void Disable(IDualObject dualObject);
        }

        private class ServerUpdateMode : AbstractUpdateMode
        {

            public override void Awake(IDualObject dualObject)
            {
                dualObject.AwakeOnServer();
            }

            public override void Start(IDualObject dualObject)
            {
                dualObject.StartOnServer();
            }

            public override void Update(IDualObject dualObject)
            {
                dualObject.UpdateOnServer();
            }

            public override void FixedUpdate(IDualObject dualObject)
            {
                dualObject.FixedUpdateOnServer();
            }

            public override void LateUpdate(IDualObject dualObject)
            {
                dualObject.LateUpdateOnServer();
            }

            public override void Enable(IDualObject dualObject)
            {
                dualObject.EnableOnServer();
            }

            public override void Disable(IDualObject dualObject)
            {
                dualObject.DisableOnServer();
            }
        }

        private class ClientUpdateMode : AbstractUpdateMode
        {

            public override void Awake(IDualObject dualObject)
            {
                dualObject.AwakeOnClient();
            }

            public override void Start(IDualObject dualObject)
            {
                dualObject.StartOnClient();
            }

            public override void Update(IDualObject dualObject)
            {
                dualObject.UpdateOnClient();
            }

            public override void FixedUpdate(IDualObject dualObject)
            {
                dualObject.FixedUpdateOnClient();
            }

            public override void LateUpdate(IDualObject dualObject)
            {
                dualObject.LateUpdateOnClient();
            }

            public override void Enable(IDualObject dualObject)
            {
                dualObject.EnableOnClient();
            }

            public override void Disable(IDualObject dualObject)
            {
                dualObject.DisableOnClient();
            }
        }    
        #endregion

        /// <summary>
        /// Текущий режим обновления.
        /// </summary>
        public UpdateModeType UpdateMode { get; private set; }

        /// <summary>
        /// Текущий объект режима обновления.
        /// </summary>
        private AbstractUpdateMode _abstractUpdateMode;
        
        protected override void Awake()
        {
            base.Awake();
            AlwaysExist = true;
            
            if (LaunchInfo.LaunchMode == LaunchModeType.Server)
            {
                SetMode(UpdateModeType.Server);
            }
            else
            {
                SetMode(UpdateModeType.Client);
            }
        }

        public void AwakeMe(IDualObject dualObject)
        {
            _abstractUpdateMode.Awake(dualObject);
        }

        public void StartMe(IDualObject dualObject)
        {
            _abstractUpdateMode.Start(dualObject);
        }

        public void UpdateMe(IDualObject dualObject)
        {
            _abstractUpdateMode.Update(dualObject);
        }

        public void FixedUpdateMe(IDualObject dualObject)
        {
            _abstractUpdateMode.FixedUpdate(dualObject);
        }

        public void LateUpdateMe(IDualObject dualObject)
        {
            _abstractUpdateMode.LateUpdate(dualObject);
        }

        public void EnableMe(IDualObject dualObject)
        {
            _abstractUpdateMode.Enable(dualObject);
        }

        public void DisableMe(IDualObject dualObject)
        {
            _abstractUpdateMode.Disable(dualObject);
        }

        /// <summary>
        /// Меняет текущий режим обновления на новый, если они отличаются. 
        /// </summary>
        public void ChangeUpdateMode(UpdateModeType newMode)
        {
            if (newMode == UpdateMode)
                return;
        }
        
        private void SetMode(UpdateModeType newMode)
        {
            if (newMode == UpdateModeType.Server)
            {
                _abstractUpdateMode = new ServerUpdateMode();
            }
            else
            {
                _abstractUpdateMode = new ClientUpdateMode();
            }
        }
    }
}