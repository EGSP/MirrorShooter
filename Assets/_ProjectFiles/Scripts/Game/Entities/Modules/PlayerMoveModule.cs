using System;
using Game.Entities.Controllers;
using Game.Entities.Modules;
using Game.Entities.States.Player;
using Gasanov.Eppd.Proceeders;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities.Modules
{
    [Serializable]
    public class PlayerMoveModule : LogicModule
    {
        public PlayerEntity PlayerEntity { get; set; }
        public PlayerInputManager PlayerInputManager { get; set; }

        [BoxGroup("Characteristics")]
        [OdinSerialize] public float MoveSpeed { get; private set; } = 5f;
        [BoxGroup("Characteristics")]
        [OdinSerialize] public float RunSpeedModifier { get; private set; } = 2f;

        [BoxGroup("Characteristics/Jump")]
        [OdinSerialize] public float JumpForce { get; private set; } = 2f;

        [BoxGroup("Characteristics/Jump")]
        [OdinSerialize][PropertyRange(0,1)]
        public float ForwardModifier { get; private set; } = 0.1f;

        [BoxGroup("Characteristics/Jump")]
        [OdinSerialize][PropertyRange(0,1)]
        public float BackwardModifier { get; private set; } = 0.06f;

        [BoxGroup("Characteristics/Jump")]
        [OdinSerialize][PropertyRange(0,1)]
        public float SidewardsModifier { get; private set; } = 0.03f;
        
        [BoxGroup("Characteristics/Jump")]
        [OdinSerialize] public float BaseJumpInterval { get; private set; }

        [BoxGroup("Global settings")]
        [OdinSerialize] public LayerMask GroundLayer { get; private set; }
        [BoxGroup("Global settings")] [InfoBox("Промежуток времени после прыжка, прежде чем проверять землю")]
        [OdinSerialize] public float GroundCheckInterval { get; private set; }
        [BoxGroup("Global settings")]
        [OdinSerialize] public float WallCheckDistance { get; private set; }
        
        
        [OdinSerialize][ReadOnly] public bool IsGrounded { get; private set; }

        [SerializeField][ReadOnly] private string currentStateName;

        /// <summary>
        /// Можно прыгать, когда пройден интервал времени с момента прошлого прыжка.
        /// </summary>
        public bool JumpIntervaled => _baseJumpInterval >= BaseJumpInterval;
        
        /// <summary>
        /// Физическое тело для перемещения.
        /// </summary>
        public Rigidbody Rigidbody { get; private set; }

        /// <summary>
        /// Текущее состояние модуля передвижения.
        /// </summary>
        private MoveModuleState _moveModuleState;

        private float _baseJumpInterval;
        private float _groundCheckInterval;
        

        
        public void Setup(PlayerEntity playerEntity, Rigidbody rig)
        {
            PlayerEntity = playerEntity;
            Rigidbody = rig;

            _baseJumpInterval = _baseJumpInterval;
            _groundCheckInterval = GroundCheckInterval;
            _moveModuleState = new MoveModuleWalk(this);
        }

        public override void UpdateOnServer()
        {
            CheckJumpInterval();
        }

        public override void FixedUpdateOnServer()
        {
            if (PlayerInputManager == null)
            {    
                return;
            }

            if (_moveModuleState != null)
            {
                currentStateName = _moveModuleState.GetType().Name;
                
                _moveModuleState = _moveModuleState.FixedUpdateOnServer(Time.fixedDeltaTime);
            }

            CheckIsGrounded();
        }

        /// <summary>
        /// Проверка земли под персонажем.
        /// </summary>
        private void CheckIsGrounded()
        {
            if (_groundCheckInterval < GroundCheckInterval)
            {
                _groundCheckInterval += Time.fixedDeltaTime;
                return;
            }
            
            // Если под ногами есть земля. Пивот должен быть в ногах.
            if (Physics.OverlapSphere(Rigidbody.position,0.01f,GroundLayer).Length != 0)
            {
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }

        public void JumpInitiated()
        {
            _baseJumpInterval = 0;
            _groundCheckInterval = 0;
            IsGrounded = false;
        }
        
        public void CheckJumpInterval()
        {
            if (_baseJumpInterval < BaseJumpInterval)
            {
                _baseJumpInterval += Time.deltaTime;
            }          
        }

        public float ExcpectedJumpHeight(Rigidbody rigidBody)
        {
            float g = Physics.gravity.magnitude;
            float v0 = JumpForce / rigidBody.mass; // converts the jumpForce to an initial velocity
            float jumpHeight = (v0 * v0)/(2*g);

            return jumpHeight;
        }
    }
}