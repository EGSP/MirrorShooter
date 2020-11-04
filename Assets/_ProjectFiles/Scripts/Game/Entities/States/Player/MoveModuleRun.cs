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
            var input = MoveModule.PlayerInputManager;

            _longJumpInterval += deltaTime;
            
            // Если нет удержания бега.
            if (!input.GetHold(KeyCode.LeftShift))
            {
                return new MoveModuleWalk(MoveModule);
            }
            
            var direction = ExtractOverallInputDirection();
            int isWalking = direction.sqrMagnitude != 0 ? 1 : 0;
            
            // Если хотим прыгнуть.
            if (input.GetDown(KeyCode.Space))
            {
                if (MoveModule.JumpIntervaled)
                {
                    var horizontal = ExtractHorizontalInput();

                    // Длинный прыжок.
                    if (horizontal == 0)
                    {

                        var isLongJump = _longJumpInterval >= MoveModule.LongJumpInterval;
                        var jumpState = new MoveModuleJump(MoveModule,
                            MoveModule.MoveSpeed * MoveModule.RunSpeedModifier, isLongJump);

                        jumpState.SetNext(new MoveModuleRun(MoveModule));
                        return jumpState;
                    }
                    // Отскок.
                    else
                    {
                        // Прыжок со скоростью ходьбы
                        var dodgeState = new MoveModuleDodge(MoveModule,horizontal,MoveModule.MoveSpeed);
                        dodgeState.SetNext(new MoveModuleRun(MoveModule));

                        return dodgeState;
                    }

                }
            }
            
            Move(direction, MoveModule.MoveSpeed * MoveModule.RunSpeedModifier, deltaTime);
            
            return this;
        }
    }

}