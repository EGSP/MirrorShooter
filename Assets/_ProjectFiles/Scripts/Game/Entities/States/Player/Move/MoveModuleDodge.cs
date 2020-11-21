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
            _targetDirection = Module.Rigidbody.transform.right * horizontalInput;;
            _baseSpeed = baseSpeed;
            
            Module.JumpInitiated();
            CallDualConstructor();
        }
        
        protected override void ConstructorServer()
        {
            base.ConstructorServer();
            
            Module.Rigidbody.AddForce(Module.Rigidbody.transform.up * Module.JumpForce,
                ForceMode.Impulse);
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

            return this;
        }

        public void ProcessMovement(float deltaTime)
        {
            var bodyCollider = Module.PlayerEntity.BodyModule.Collider;
            if (Physics.Raycast(bodyCollider.center+Module.Rigidbody.position,
                Module.Rigidbody.transform.forward,
                bodyCollider.radius + Module.WallCheckDistance, Module.GroundLayer))
            {
                return;
            }
            
            Move(_targetDirection, _baseSpeed, deltaTime);
        }
        
        
    }
}