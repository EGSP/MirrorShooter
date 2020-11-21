using System;
using Game.Entities.Controllers;
using Game.Entities.Modules;
using Game.Entities.States.Player;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities.Modules
{
    [Serializable]
    public class PlayerMoveModule : LogicModule<MoveModuleState, PlayerMoveModule>
    {
        public PlayerEntity PlayerEntity { get; set; }
        public PlayerInputManager PlayerInputManager { get; set; }

        [BoxGroup("Characteristics")]
        [OdinSerialize] public float MoveSpeed { get; private set; } = 5f;
        [BoxGroup("Characteristics")]
        [OdinSerialize] public float RunSpeedModifier { get; private set; } = 2f;
        [BoxGroup("Characteristics")]
        [OdinSerialize] public float CrouchSpeedModifier { get; private set; }
        
        [BoxGroup("Characteristics/Sprint")]
        [OdinSerialize] public float SprintSpeedModifier { get; private set; }
        
        [BoxGroup("Characteristics/Sprint")]
        [OdinSerialize] public float SprintTime { get; private set; }
        
        [BoxGroup("Characteristics/Sprint")] [PropertyTooltip("Время для возможности активации спринта.")]
        [OdinSerialize] public float SprintInterval { get; private set; }

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
        [BoxGroup("Characteristics/Jump")]
        [OdinSerialize] public float LongJumpInterval { get; private set; }

        [BoxGroup("Global settings")]
        [OdinSerialize] public LayerMask GroundLayer { get; private set; }
        [BoxGroup("Global settings")] [InfoBox("Промежуток времени после прыжка, прежде чем проверять землю.")]
        [OdinSerialize] public float GroundCheckDelay { get; private set; }
        [BoxGroup("Global settings")]
        [OdinSerialize] public float WallCheckDistance { get; private set; }
        
        
        [OdinSerialize][ReadOnly] public bool IsGrounded { get; private set; }
        [OdinSerialize][ReadOnly] public bool IsHeadUnderObstacle { get; private set; }

        /// <summary>
        /// Можно прыгать, когда пройден интервал времени с момента прошлого прыжка.
        /// </summary>
        public bool JumpIntervaled => _baseJumpInterval >= BaseJumpInterval;
        
        /// <summary>
        /// Физическое тело для перемещения.
        /// </summary>
        public Rigidbody Rigidbody { get; private set; }

        private float _baseJumpInterval;
        private float _groundCheckDelay;
        

        
        public void Setup(PlayerEntity playerEntity, Rigidbody rig)
        {
            PlayerEntity = playerEntity;
            Rigidbody = rig;

            _baseJumpInterval = _baseJumpInterval;
            _groundCheckDelay = GroundCheckDelay;
            CurrentState = new MoveModuleWalk(this);
        }

        protected override void DefineStates()
        {
            DefineState(typeof(MoveModuleWalk).Name, () =>
            {
                return new MoveModuleWalk(this);
            });
            
            DefineState(typeof(MoveModuleRun).Name, () =>
            {
                return new MoveModuleRun(this);
            });
            
            DefineState(typeof(MoveModuleJump).Name, () =>
            {
                return new MoveModuleJump(this, 0, 0);
            });
            
            DefineState(typeof(MoveModuleLongJump).Name, () => new MoveModuleLongJump(this,0));
            DefineState(typeof(MoveModuleDodge).Name, () => new MoveModuleDodge(this,0,0));
            DefineState(typeof(MoveModuleCrouch).Name, () => new MoveModuleCrouch(this));
            DefineState(typeof(MoveModuleSprint).Name, () => new MoveModuleSprint(this));
        }

        public override void UpdateOnServer()
        {
            if (PlayerInputManager == null)
            {    
                return;
            }

            UpdateStateOnClient();
            UpdateStateOnServer();
            
            CheckJumpInterval();
            
            CheckIsGrounded();
            CheckIsHeadUnderObstacle();
        }

        public override void FixedUpdateOnServer()
        {
            if (PlayerInputManager == null)
            {    
                return;
            }

            FixedUpdateStateOnClient();
            FixedUpdateStateOnServer();
        }

        /// <summary>
        /// Проверка земли под персонажем.
        /// </summary>
        private void CheckIsGrounded()
        {
            
            // Debug.Log("CheckGroundStart");
            if (_groundCheckDelay < GroundCheckDelay)
            {
                // Debug.Log("CheckDelay");
                _groundCheckDelay += Time.fixedDeltaTime;
                return;
            }
            
            // Если под ногами есть земля. Пивот должен быть в ногах.
            if (Physics.OverlapSphere(Rigidbody.position,0.1f,GroundLayer).Length != 0)
            {
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }

        private void CheckIsHeadUnderObstacle()
        {
            if (Physics.OverlapSphere(
                Rigidbody.position + Vector3.up * PlayerEntity.BodyModule.InOutCrouch.Get(0), 
                0.1f, GroundLayer).Length != 0)
            {
                IsHeadUnderObstacle = true;
            }
            else
            {
                IsHeadUnderObstacle = false;
            }
        }

        public void JumpInitiated()
        {
            _baseJumpInterval = 0;
            _groundCheckDelay = 0;
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