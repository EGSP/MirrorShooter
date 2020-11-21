using Game.Entities.Modules;
using Game.Entities.States.Player.Body;
using UnityEngine;

namespace Game.Entities.States.Player
{
    public class MoveModuleSprint : MoveModuleState
    {
        private float _sprintTime;
        
        public MoveModuleSprint(PlayerMoveModule moveModule) : base(moveModule)
        {
            _sprintTime = 0;
            CallDualConstructor();
        }

        protected override void ConstructorServer()
        {
            base.ConstructorServer();
            
            Module.PlayerEntity.BodyModule
                .SetState(new BodyModuleInOutCrouch(Module.PlayerEntity.BodyModule, InOut.Out));
        }

        public override MoveModuleState UpdateOnServer(float deltaTime)
        {
            // Если игрок на земле и над ним препятствие.
            if (Module.IsGrounded && Module.IsHeadUnderObstacle)
            {
                return new MoveModuleCrouch(Module);       
            }
            
            var direction = ExtractRawInputDirection();
            
            var isShifting = InputManager.GetHold(KeyCode.LeftShift);
            var isSpacing = InputManager.GetDown(KeyCode.Space);

            int isWalking = CheckIsWalking(direction);

            // Стоим на месте.
            if (isWalking == 0)
            {
                return new MoveModuleWalk(Module);
            }

            // Пятимся назад.
            if (CheckIsBacking(direction))
            {
                // Debug.Log("IsBacking");
                if (isShifting)
                {
                    return new MoveModuleRun(Module);
                }
                else
                {
                    return new MoveModuleWalk(Module);
                }
            }
            
            // Переход на ходьбу.
            if (!isShifting)
            {
                // Debug.Log("Not shifting");
                return new MoveModuleWalk(Module);
            }
            
            // C этого момента игрок точно нажимает шифт и идет!!
            
            // Переход на прыжок.
            if (isSpacing)
            {
                // Над головой нет препятствия.
                if (Module.IsHeadUnderObstacle == false)
                    return new MoveModuleLongJump(Module, Module.MoveSpeed * Module.RunSpeedModifier);
            }
            
            // Вышло время спринта.
            if (_sprintTime >= Module.SprintTime)
            {  
                return new MoveModuleRun(Module);
            }
            
            // Игрок нажимает только в стороны.
            if (CheckIsOnlyHoriznotalInput(direction))
            {
                // Переходим на обычный бег.
                return new MoveModuleRun(Module);   
            }
            
            Module.PlayerEntity.AnimationModule.Play("Sprint");

            return this;
        }

        public override MoveModuleState FixedUpdateOnServer(float deltaTime)
        {
            var direction = ExtractOverallInputDirection();
            
            _sprintTime += deltaTime;
            
            Move(direction, Module.MoveSpeed * Module.SprintSpeedModifier, deltaTime);

            return this;
        }
    }
}