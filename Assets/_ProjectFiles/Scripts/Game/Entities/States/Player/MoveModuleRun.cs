using Game.Entities.Modules;
using UnityEngine;

namespace Game.Entities.States.Player
{
    public class MoveModuleRun : MoveModuleState
    {
        public MoveModuleRun(PlayerMoveModule moveModule) : base(moveModule)
        {
        }

        public override MoveModuleState FixedUpdateOnServer(float deltaTime)
        {
            var input = MoveModule.PlayerInputManager;
            
            // Если нет удержания.
            if (!input.GetHold(KeyCode.LeftShift))
            {
                return new MoveModuleWalk(MoveModule);
            }
            
            var direction = ExtractOverallInputDirection();
            int isWalking = direction.sqrMagnitude != 0 ? 1 : 0;
            
            Move(direction, MoveModule.MoveSpeed * MoveModule.RunSpeedModifier, deltaTime);

            if (input.GetDown(KeyCode.Space))
            {
                if (MoveModule.JumpIntervaled)
                    return new MoveModuleJump(MoveModule, MoveModule.MoveSpeed * MoveModule.RunSpeedModifier,
                        isWalking);
            }
            
            return this;
        }
    }

}