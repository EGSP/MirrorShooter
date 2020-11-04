using Game.Entities.Modules;
using UnityEngine;

namespace Game.Entities.States.Player
{
    public class MoveModuleDodge : MoveModuleState
    {
        private readonly Vector3 _targetDirection;
        private readonly float _baseSpeed;

        public MoveModuleDodge(PlayerMoveModule moveModule, int horizontalInput, float baseSpeed) : base(moveModule)
        {
            _targetDirection = MoveModule.Rigidbody.transform.right * horizontalInput;;
            _baseSpeed = baseSpeed;
            
            MoveModule.JumpInitiated();
            
            MoveModule.Rigidbody.AddForce(MoveModule.Rigidbody.transform.up * MoveModule.JumpForce,
                ForceMode.Impulse);
            
        }

        public override MoveModuleState FixedUpdateOnServer(float deltaTime)
        {
            // Если на земле.
            if (MoveModule.IsGrounded)
            {
                if (NextState != null)
                {
                    return NextState;
                }
                else
                {
                    return new MoveModuleWalk(MoveModule);
                }
            }
            
            ProcessMovement(deltaTime);

            return this;
        }

        public void ProcessMovement(float deltaTime)
        {
            var bodyCollider = MoveModule.PlayerEntity.BodyEntity.Collider;
            if (Physics.Raycast(bodyCollider.center+MoveModule.Rigidbody.position,
                MoveModule.Rigidbody.transform.forward,
                bodyCollider.radius + MoveModule.WallCheckDistance, MoveModule.GroundLayer))
            {
                return;
            }
            
            Move(_targetDirection, _baseSpeed, deltaTime);
        }
        
        
    }
}