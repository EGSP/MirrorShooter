using Game.Entities.Modules;
using Game.Entities.States.Player.Body;
using UnityEngine;

namespace Game.Entities.States.Player
{
    public class MoveModuleCrouch : MoveModuleState
    {
        private float _crouchTime;
        
        public MoveModuleCrouch(PlayerMoveModule moveModule) : base(moveModule)
        {
            _crouchTime = 0;
            CallDualConstructor();
        }

        protected override void ConstructorServer()
        {
            base.ConstructorServer();
            
            // Нужно убрать, чтобы из-за нестыковки обновления физики не было сбоя.
            InputManager.RemoveKey(KeyCode.LeftControl);
            Module.PlayerEntity.BodyModule
                .SetState(new BodyModuleInOutCrouch(Module.PlayerEntity.BodyModule, InOut.In));
        }

        public override MoveModuleState UpdateOnServer(float deltaTime)
        {
            bool canSprint = false;
            if (_crouchTime > Module.SprintInterval)
            {
                canSprint = true;
            }
            else
            {
                _crouchTime += deltaTime;
            }

            // Над головой препятствие.
            if (Module.IsHeadUnderObstacle)
                return this;
            
            // Встать.
            if (InputManager.GetDown(KeyCode.LeftControl))
            {
                BodyOut();
                
                // Нужно убрать нажатие, чтобы из-за нестыковки обновления физики не было сбоя.
                InputManager.RemoveKey(KeyCode.LeftControl);
                return new MoveModuleWalk(Module);
            }

            var direction = ExtractRawInputDirection();
            int isWalking = CheckIsWalking(direction);

            // Бежать.
            if (isWalking == 1)
            {
                if (InputManager.GetHold(KeyCode.LeftShift))
                {
                    if (canSprint)
                    {
                        // Переходим на спринт.
                        return new MoveModuleSprint(Module);
                    }
                    else
                    {
                        BodyOut();
                        return new MoveModuleRun(Module);
                    }
                }
            }

            return this;
        }

        public override MoveModuleState FixedUpdateOnServer(float deltaTime)
        {
            var direction = ExtractOverallInputDirection();
            
            // Ходьба.
            if (direction.sqrMagnitude > 0)
            {
                Play("Crouch_Walk");
                Move(direction, Module.MoveSpeed*Module.CrouchSpeedModifier, deltaTime);
            }
            // Бездействие.
            else
            {
                Play("Crouch_Idle");
            }

            return this;
        }

        private void BodyOut()
        {
            Module.PlayerEntity.BodyModule
                .SetState(new BodyModuleInOutCrouch(Module.PlayerEntity.BodyModule, InOut.Out));
        }
    }
}