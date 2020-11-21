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
        
        [OdinSerialize]
        public Transform CameraTarget { get; private set; }
        
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
            CommonAwake();
        }

        public override void AwakeOnServer()
        {
            CommonAwake();
        }

        private void CommonAwake()
        {
            if(BodyTransform == null)
                throw new NullReferenceException();
            
            if(Collider == null)
                throw new NullReferenceException();

            _cachedBodyRotation = BodyTransform.rotation;
        }

        protected override void DefineStates()
        {
            DefineState(typeof(BodyModuleInOutCrouch).Name+"_In", () =>
            {
                return new BodyModuleInOutCrouch(this,InOut.In);
            });
            
            DefineState(typeof(BodyModuleInOutCrouch).Name+"_Out", () =>
            {
                return new BodyModuleInOutCrouch(this,InOut.Out);
            });
        }

        public override void UpdateOnServer()
        {
            if (PlayerInputManager == null)
            {    
                return;
            }
            
            UpdateState();
            MoveCameraTarget();
        }

        private void MoveCameraTarget()
        {
            CameraTarget.localPosition = Vector3.zero + new Vector3(0, 
                InOutCrouch.Get(CurrentCrouchOpacity),0);
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