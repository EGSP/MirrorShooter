using System;
using Game.Entities.Controllers;
using Game.Entities.States.Player;
using Game.Entities.States.Player.Body;
using Gasanov.Extensions.Curves;
using Mirror;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities.Modules
{
    [Serializable]
    public class PlayerBodyModule : LogicModule<BodyModuleState, PlayerBodyModule>
    {
        public PlayerEntity PlayerEntity { get; set; }
        public PlayerInputManager PlayerInputManager { get; set; }
        
        /// <summary>
        /// Корневое тело игрока.
        /// </summary>
        [OdinSerialize] 
        public Transform BodyTransform { get; private set; }
        
        /// <summary>
        /// Физический коллайдер игрока.
        /// </summary>
        [OdinSerialize]
        public CapsuleCollider Collider { get; private set; }
        
        [BoxGroup("Height")]
        [OdinSerialize] public CurveHandler InOutCrouch { get; private set; }
        [BoxGroup("Height")]
        [OdinSerialize] public float InOutCrouchTime { get; private set; }
        [BoxGroup("Height")]
        [OdinSerialize] public Vector2 CrouchY { get; private set; }
        
        [BoxGroup("Height")]
        [OdinSerialize] public CurveHandler InSprint { get; private set; }
        [BoxGroup("Height")]
        [OdinSerialize] public float InSprintTime { get; private set; }

        
        private Quaternion _cachedBodyRotation;
        
        /// <summary>
        /// Текущая позиция присядания на кривой.
        /// Нужна в случае быстрой смены положения, чтобы не проигрывать анимацию с краев.
        /// </summary>
        public float CurrentCrouchOpacity { get; set; }

        /// <summary>
        /// Присел ли персонаж.
        /// </summary>
        public bool HasCrouched => CurrentCrouchOpacity == 1;
        
        public override void AwakeOnClient()
        {
            if(BodyTransform == null)
                throw new NullReferenceException();
            
            if(Collider == null)
                throw new NullReferenceException();

            _cachedBodyRotation = BodyTransform.rotation;
        }

        public override void AwakeOnServer()
        {
            if(BodyTransform == null)
                throw new NullReferenceException();
            
            if(Collider == null)
                throw new NullReferenceException();

            _cachedBodyRotation = BodyTransform.rotation;
        }

        public override void UpdateOnServer()
        {
            if (PlayerInputManager == null)
            {    
                return;
            }
            
            UpdateState();
        }

        /// <summary>
        /// Вращение тела у клиента.
        /// </summary>
        public void RotateY(float deltaRotationY)
        {
            // Новое вращение тела
            _cachedBodyRotation *= Quaternion.Euler(0, deltaRotationY, 0);
            
            // Поворот тела
            BodyTransform.rotation = _cachedBodyRotation;
        }

    }
}