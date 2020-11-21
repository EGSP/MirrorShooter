using Game.Entities.Modules;
using UnityEngine;

namespace Game.Entities.States.Player
{
    public class MoveModuleLongJump : MoveModuleState
    {
        /// <summary>
        /// Направление взгляда в момент прыжка.
        /// </summary>
        private Vector3 _targetDirection;

        private readonly float _baseSpeed;
        
        public MoveModuleLongJump(PlayerMoveModule moveModule, float baseSpeed) : base(moveModule)
        {
            Module.JumpInitiated();
            
            Module.Rigidbody.AddForce(Module.Rigidbody.transform.up * Module.JumpForce,
                ForceMode.Impulse);
            
            var vertical = ExtractVerticalInput();
            _targetDirection += Module.Rigidbody.transform.forward * vertical;

            var horizontal = ExtractHorizontalInput();
            _targetDirection += Module.Rigidbody.transform.right * horizontal;
            
            _baseSpeed = baseSpeed;
        }
        
        public override MoveModuleState FixedUpdateOnServer(float deltaTime)
        {
            // Если на земле.
            if (Module.IsGrounded)
            {
                if (NextState != null)
                {
                    return NextState;
                }
                else
                {
                    return new MoveModuleWalk(Module);
                }
            }
            
            ProcessMovement(deltaTime);

            // Если в полете.
            return this;
        }
        
        private void ProcessMovement(float deltaTime)
        {
            var bodyCollider = Module.PlayerEntity.BodyModule.Collider;
            if (Physics.Raycast(bodyCollider.center+Module.Rigidbody.position,
                Module.Rigidbody.transform.forward,
                bodyCollider.radius + Module.WallCheckDistance, Module.GroundLayer))
            {
                _targetDirection = Vector3.zero;
                return;
            }
            
            Move(_targetDirection, _baseSpeed, deltaTime);
            return;

        }
    }
}