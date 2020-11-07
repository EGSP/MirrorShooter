using Game.Entities.Modules;
using UnityEngine;

namespace Game.Entities.States.Player
{
    public class MoveModuleRun : MoveModuleState
    {
        public MoveModuleRun(PlayerMoveModule moveModule) : base(moveModule)
        {
            _longJumpInterval = 0;
        }

        private float _longJumpInterval;

        public override MoveModuleState FixedUpdateOnServer(float deltaTime)
        {
            // Если игрок на земле и над ним препятствие.
            if (Module.IsGrounded && Module.IsHeadUnderObstacle)
            {
                return new MoveModuleCrouch(Module);       
            }
            
            var input = Module.PlayerInputManager;

            _longJumpInterval += deltaTime;
            
            var direction = ExtractOverallInputDirection();
            int isWalking = direction.sqrMagnitude != 0 ? 1 : 0;
            
            if (isWalking == 0)
            {
                return new MoveModuleWalk(Module);
            }
            
            // Если нет удержания бега.
            if (!input.GetHold(KeyCode.LeftShift))
            {
                return new MoveModuleWalk(Module);
            }
            
            
            // Если хотим прыгнуть.
            if (input.GetDown(KeyCode.Space))
            {
                // Над головой препятствие.
                if (Module.IsHeadUnderObstacle)
                    return this;
                
                if (Module.JumpIntervaled)
                {
                    var horizontal = ExtractHorizontalInput();
                    var vertical = ExtractVerticalInput();

                    
                    // Прыжок вперед-назад
                    if (horizontal == 0)
                    {
                        bool isLongJump = _longJumpInterval >= Module.LongJumpInterval && vertical != -1;

                        // Длинный прыжок.
                        if (isLongJump)
                        {
                            var jumpState = new MoveModuleJump(Module,
                                Module.MoveSpeed * Module.RunSpeedModifier, isWalking, isLongJump);

                            jumpState.SetNext(new MoveModuleRun(Module));
                            return jumpState;
                        }
                        // Прыжок назад.
                        else
                        {
                            var jumpState = new MoveModuleJump(Module, Module.MoveSpeed);

                            jumpState.SetNext(new MoveModuleWalk(Module));
                            return jumpState;
                        }
                    }
                    // Отскок.
                    else
                    {
                        // Прыжок со скоростью ходьбы
                        var dodgeState = new MoveModuleDodge(Module,horizontal,Module.MoveSpeed);
                        dodgeState.SetNext(new MoveModuleRun(Module));

                        return dodgeState;
                    }

                }
            }
            
            Play("Walk");

            Move(direction, Module.MoveSpeed * Module.RunSpeedModifier, deltaTime);
            
            return this;
        }
    }

}