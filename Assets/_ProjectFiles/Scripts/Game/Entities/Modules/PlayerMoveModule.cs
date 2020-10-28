using System;
using Game.Entities.Controllers;
using Game.Entities.Modules;
using Game.Processors;
using Gasanov.Eppd.Proceeders;
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
            protected MoveModuleState(PlayerMoveModule moveModule)
            {
                MoveModule = moveModule;
            }
            
            public abstract MoveModuleState FixedUpdateOnServer(float deltaTime);

            public abstract void Move(float horizontal, float vertical);
        }

        public class MoveModuleWalk : MoveModuleState
        {
            public MoveModuleWalk(PlayerMoveModule moveModule) : base(moveModule)
            {
                
            }

            public override MoveModuleState FixedUpdateOnServer(float deltaTime)
            {
                MoveModule.MoveData.FixedDeltaTime = deltaTime;
                MoveModule.RigMoveProcessor.Tick();

                var input = MoveModule.PlayerInputManager;

                if (input.GetHold(KeyCode.LeftShift))
                {
                    return new MoveModuleRun(MoveModule);
                }
                
                return this;
            }

            public override void Move(float horizontal, float vertical)
            {
                var forwardBack = vertical * MoveModule.Rigidbody.transform.forward;
                var rightLeft = horizontal * MoveModule.Rigidbody.transform.right;
                
                MoveModule.RigMoveProcessor.DirectionProceeder.Add(forwardBack+rightLeft);
            }
        }
        
        public class MoveModuleRun : MoveModuleState
        {
            public MoveModuleRun(PlayerMoveModule moveModule) : base(moveModule)
            {
                MoveModule.RigMoveProcessor.AddMoveDataModifier(new RunDataModifier(MoveModule.RunSpeedModifier));
            }

            public override MoveModuleState FixedUpdateOnServer(float deltaTime)
            {
                MoveModule.MoveData.FixedDeltaTime = deltaTime;
                MoveModule.RigMoveProcessor.Tick();

                var input = MoveModule.PlayerInputManager;

                // Если нет удержания.
                if (!input.GetHold(KeyCode.LeftShift))
                {
                    MoveModule.RigMoveProcessor.RemoveMoveDataModifier<RunDataModifier>();
                    return new MoveModuleWalk(MoveModule);
                }

                return this;
            }

            public override void Move(float horizontal, float vertical)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        [OdinSerialize] public float MoveSpeed { get; private set; } = 5f;
        [OdinSerialize] public float RunSpeedModifier { get; private set; } = 2f;
        
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