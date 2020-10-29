using System;
using Game.Entities.Controllers;
using Game.Entities.Modules;
using Game.Processors;
using Gasanov.Eppd.Proceeders;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Entities.Modules
{
    [Serializable]
    public class PlayerMoveModule : LogicModule
    {
        public PlayerInputManager PlayerInputManager { get; set; }

        #region States

        public abstract class MoveModuleState
        {
            protected readonly PlayerMoveModule MoveModule;
            /// <summary>
            /// Следующее состояние. Может быть пустым, тогда состояние само определяет следующее.
            /// </summary>
            protected MoveModuleState NextState { get; private set; }
            
            protected MoveModuleState(PlayerMoveModule moveModule)
            {
                MoveModule = moveModule;
            }
            
            public abstract MoveModuleState FixedUpdateOnServer(float deltaTime);

            public virtual void Move(float horizontal, float vertical)
            {
                var forwardBack = vertical * MoveModule.Rigidbody.transform.forward;
                var rightLeft = horizontal * MoveModule.Rigidbody.transform.right;
                
                MoveModule.RigMoveProcessor.DirectionProceeder.Add(forwardBack+rightLeft);
            }

            public void SetNext(MoveModuleState nextState)
            {
                NextState = nextState;
            }
        }

        public class MoveModuleWalk : MoveModuleState
        {
            public MoveModuleWalk(PlayerMoveModule moveModule) : base(moveModule)
            {
                Debug.Log("NEW WALK MODULE");
            }

            public override MoveModuleState FixedUpdateOnServer(float deltaTime)
            {
                MoveModule.MoveData.FixedDeltaTime = deltaTime;
                MoveModule.RigMoveProcessor.Tick();

                var input = MoveModule.PlayerInputManager;
                
                if (input.GetDown(KeyCode.Space))
                {
                    return new MoveModuleJump(MoveModule, MoveModule.MoveSpeed);
                }

                if (input.GetHold(KeyCode.LeftShift))
                {
                    return new MoveModuleRun(MoveModule);
                }
                
                return this;
            }
        }
        
        public class MoveModuleRun : MoveModuleState
        {
            public MoveModuleRun(PlayerMoveModule moveModule) : base(moveModule)
            {
                Debug.Log("NEW RUN MODULE");
                MoveModule.RigMoveProcessor.AddMoveDataModifier(new RunDataModifier(MoveModule.RunSpeedModifier));
            }

            public override MoveModuleState FixedUpdateOnServer(float deltaTime)
            {
                MoveModule.MoveData.FixedDeltaTime = deltaTime;
                MoveModule.RigMoveProcessor.Tick();

                var input = MoveModule.PlayerInputManager;

                if (input.GetDown(KeyCode.Space))
                {
                    MoveModule.RigMoveProcessor.RemoveMoveDataModifier<RunDataModifier>();
                    return new MoveModuleJump(MoveModule,
                        MoveModule.RunSpeedModifier * MoveModule.MoveSpeed);
                    //
                    // js.SetNext(new );
                }
                
                // Если нет удержания.
                if (!input.GetHold(KeyCode.LeftShift))
                {
                    // Debug.Log("NOT RUN");
                    MoveModule.RigMoveProcessor.RemoveMoveDataModifier<RunDataModifier>();
                    return new MoveModuleWalk(MoveModule);
                }

                return this;
            }
        }
        
        public class MoveModuleJump : MoveModuleState
        {
            private readonly JumpDataModifier _jumpDataModifier;
            public MoveModuleJump(PlayerMoveModule moveModule, float flySpeed) : base(moveModule)
            {
                Debug.Log("NEW JUMP MODULE");
                _jumpDataModifier = new JumpDataModifier(flySpeed);
                MoveModule.RigMoveProcessor.AddMoveDataModifier(_jumpDataModifier);
                
                MoveModule.Rigidbody.AddForce(MoveModule.Rigidbody.transform.up * MoveModule.JumpForce,
                    ForceMode.Impulse);
            }

            public override MoveModuleState FixedUpdateOnServer(float deltaTime)
            {
                MoveModule.MoveData.FixedDeltaTime = deltaTime;
                MoveModule.RigMoveProcessor.Tick();

                // Если на земле.
                if (MoveModule.IsGrounded)
                {
                    MoveModule.RigMoveProcessor.RemoveMoveDataModifier<JumpDataModifier>();

                    if (NextState != null)
                    {
                        return NextState;
                    }
                    else
                    {
                        return new MoveModuleWalk(MoveModule);
                    }
                }
                
                // Если в прыжке.
                return this;
            }
        }
        

        #endregion
        

        [BoxGroup("Characteristics")]
        [OdinSerialize] public float MoveSpeed { get; private set; } = 5f;
        [BoxGroup("Characteristics")]
        [OdinSerialize] public float RunSpeedModifier { get; private set; } = 2f;

        [BoxGroup("Characteristics")]
        [OdinSerialize] public float JumpForce { get; private set; } = 2f;

        [BoxGroup("Global settings")]
        [OdinSerialize] public LayerMask GroundLayer { get; private set; }
        
        /// <summary>
        /// Данные передвижения.
        /// </summary>
        public MoveData MoveData { get; private set; }
        
        /// <summary>
        /// Физическое тело для перемещения.
        /// </summary>
        public Rigidbody Rigidbody { get; private set; }
        
        /// <summary>
        /// Процессор для передвижения.
        /// </summary>
        public RigMoveProcessor RigMoveProcessor { get; private set; }
        
        /// <summary>
        /// Находится ли персонаж сейчас на земле.
        /// </summary>
        [OdinSerialize] public bool IsGrounded { get; private set; }

        /// <summary>
        /// Текущее состояние модуля передвижения.
        /// </summary>
        private MoveModuleState _moveModuleState;
        
        
        public void Setup(Rigidbody rig)
        {
            Rigidbody = rig;
            
            MoveData = new MoveData(){Speed = MoveSpeed};
            RigMoveProcessor = new RigMoveProcessor(new RigidBodyData(Rigidbody), MoveData);

            _moveModuleState = new MoveModuleWalk(this);
        }

        public override void FixedUpdateOnServer()
        {
            if (PlayerInputManager == null)
            {    
                Debug.LogWarning("PLAYER_INPUT_NULL");
                return;
            }

            if (_moveModuleState != null)
                _moveModuleState = _moveModuleState.FixedUpdateOnServer(Time.fixedDeltaTime);
            
            CheckIsGrounded();
        }

        /// <summary>
        /// Проверка земли под персонажем.
        /// </summary>
        private void CheckIsGrounded()
        {
            // Если под нами есть земля
            if (Physics.Raycast(Rigidbody.position, Rigidbody.transform.up * -1,
                0.3f, GroundLayer))
            {
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }

        /// <summary>
        /// Передвижение объекта с помощью двух направлений.
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        public void Move(float horizontal, float vertical)
        {
            _moveModuleState.Move(horizontal, vertical);
        }
    }
}