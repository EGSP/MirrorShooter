using System;
using Game.Net.Objects;
using Mirror;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities
{
    public class PlayerBodyEntity : DualNetworkBehaviour
    {
        /// <summary>
        /// Корневое тело игрока.
        /// </summary>
        [OdinSerialize] 
        public Transform BodyTransform { get; private set; }

        private Quaternion _cachedBodyRotation;
        
        protected override void Awake()
        {
            base.Awake();
            
            if(BodyTransform == null)
                throw new NullReferenceException();

            _cachedBodyRotation = BodyTransform.rotation;
        }

        protected override void OnClient()
        {
            // throw new System.NotImplementedException();
        }

        protected override void OnServer()
        {
            // throw new System.NotImplementedException();
        }

        [Command(ignoreAuthority = true)]
        public void CmdRotateY(float deltaRotationY)
        {
            Debug.Log("CMD_BODY");
            // Новое вращение тела
            _cachedBodyRotation *= Quaternion.Euler(0, deltaRotationY, 0);
            
            // Поворот тела
            BodyTransform.rotation = _cachedBodyRotation;
        }
    }
}