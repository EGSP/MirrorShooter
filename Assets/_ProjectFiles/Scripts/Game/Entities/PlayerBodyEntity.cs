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
        

        public override void AwakeOnServer()
        {
            if(BodyTransform == null)
                throw new NullReferenceException();

            _cachedBodyRotation = BodyTransform.rotation;
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