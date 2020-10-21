using System;
using System.Collections.Generic;
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
            protected AbstractUpdateMode(List<IDualObject> dualObjects)
            {
                DualObjects = dualObjects;
            }

            protected readonly List<IDualObject> DualObjects;
            
            /// <summary>
            /// Вызывается после обновления.
            /// </summary>
            public event Action AfterUpdate = delegate {  };

            public abstract void Update();

            protected void OnAfterUpdate()
            {
                AfterUpdate();
            }
        }

        private class ServerUpdateMode : AbstractUpdateMode
        {
            public ServerUpdateMode(List<IDualObject> dualObjects) : base(dualObjects)
            {
            }

            public override void Update()
            {
                // Вызываем обновление у всех объектов.
                for (var i = DualObjects.Count - 1; i > -1; i--)
                {
                    if (DualObjects != null)
                    {
                        DualObjects[i].UpdateOnServer();
                    }
                    else
                    {
                        DualObjects.RemoveAt(i);
                    }          
                }
                
                OnAfterUpdate();
            }
        }

        private class ClientUpdateMode : AbstractUpdateMode
        {
            public ClientUpdateMode(List<IDualObject> dualObjects) : base(dualObjects)
            {
            }

            public override void Update()
            {
                // Вызываем обновление у всех объектов.
                for (var i = DualObjects.Count - 1; i > -1; i--)
                {
                    if (DualObjects != null)
                    {
                        DualObjects[i].UpdateOnClient();
                    }
                    else
                    {
                        DualObjects.RemoveAt(i);
                    }          
                }
                
                OnAfterUpdate();
            }
        }    
        #endregion
        
        /// <summary>
        /// Объекты, которые должны обновляться.
        /// </summary>
        private List<IDualObject> _dualObjects;

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

            _dualObjects = new List<IDualObject>();
            
            if (NetworkServer.active)
            {
                SetMode(UpdateModeType.Server);
            }
            else
            {
                SetMode(UpdateModeType.Client);
            }
        }

        private void Update()
        {
            _abstractUpdateMode.Update();
        }

        /// <summary>
        /// Добавляет новый объект в очередь обновления.
        /// Обновление произойдет только на следующей итерации.
        /// </summary>
        public void Add(IDualObject dualObject)
        {
            _dualObjects.Add(dualObject);
        }

        private void SetMode(UpdateModeType newMode)
        {
            if (newMode == UpdateModeType.Server)
            {
                _abstractUpdateMode = new ServerUpdateMode(_dualObjects);
            }
            else
            {
                _abstractUpdateMode = new ClientUpdateMode(_dualObjects);
            }
        }

        /// <summary>
        /// Меняет текущий режим обновления на новый, если они отличаются. 
        /// </summary>
        public void ChangeUpdateMode(UpdateModeType newMode)
        {
            if (newMode == UpdateMode)
                return;

            _abstractUpdateMode.AfterUpdate += () => SetMode(newMode);
        }
    }
}