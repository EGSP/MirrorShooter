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
            // Если игрок на земле и над ним препятствие.
            if (Module.IsGrounded && Module.IsHeadUnderObstacle)
            {
                return new MoveModuleCrouch(Module);       
            }
            
            var input = Module.PlayerInputManager;
            
            var direction = ExtractOverallInputDirection();
            int isWalking = direction.sqrMagnitude != 0 ? 1 : 0;

            if (isWalking == 1)
            {
                Move(direction, Module.MoveSpeed, deltaTime);
                Play("Walk");
            }
            else
            {
                Play("Idle");
            }

            if (input.GetDown(KeyCode.Space))
            {
                // Над головой препятствие.
                if (Module.IsHeadUnderObstacle)
                    return this;
                
                if (Module.JumpIntervaled)
                    return new MoveModuleJump(Module, Module.MoveSpeed, isWalking);
            }
            
            if (input.GetHold(KeyCode.LeftShift) && isWalking == 1)
            {
                return new MoveModuleRun(Module);
            }

            if (InputManager.GetDown(KeyCode.LeftControl))
            {
                return new MoveModuleCrouch(Module);
            }

            return this;
        }
    }

}