using Game.Entities.Modules;
using UnityEngine;

namespace Game.Entities.States.Player
{
    public class MoveModuleWalk : MoveModuleState
    {
        public MoveModuleWalk(PlayerMoveModule moveModule) : base(moveModule)
        {
        }

        public override MoveModuleState FixedUpdateOnServer(float deltaTime)
        {
            var input = MoveModule.PlayerInputManager;
            
            var direction = ExtractOverallInputDirection();
            int isWalking = direction.sqrMagnitude != 0 ? 1 : 0;
            
            Move(direction, MoveModule.MoveSpeed, deltaTime);

            if (input.GetDown(KeyCode.Space))
            {
                if (MoveModule.JumpIntervaled)
                    return new MoveModuleJump(MoveModule, MoveModule.MoveSpeed, isWalking);
            }
            
            if (input.GetHold(KeyCode.LeftShift))
            {
                return new MoveModuleRun(MoveModule);
            }

            return this;
        }
    }

}