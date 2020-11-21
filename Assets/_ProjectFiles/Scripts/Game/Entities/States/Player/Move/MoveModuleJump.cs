using Game.Entities.Modules;
using UnityEngine;

namespace Game.Entities.States.Player
{
    public class MoveModuleJump : MoveModuleState
    {
        /// <summary>
        /// Направление взгляда в момент прыжка.
        /// </summary>
        private Vector3 _targetDirection;

        private readonly float _baseSpeed;
         
        /// <param name="baseSpeed">Базовая скорость для перемещения во время полета</param>
        /// <param name="startSpeed">Стартовая скорость. 0 - если прыжок с места, 1 - если во время движения</param>
        public MoveModuleJump(PlayerMoveModule moveModule, float baseSpeed, int isWalking = 1) : base(moveModule)
        {
            Module.JumpInitiated();
            
            Module.Rigidbody.AddForce(Module.Rigidbody.transform.up * Module.JumpForce,
                ForceMode.Impulse);

            var vertical = ExtractVerticalInput();
            _targetDirection += Module.Rigidbody.transform.forward * vertical * isWalking;

            var horizontal = ExtractHorizontalInput();
            _targetDirection += Module.Rigidbody.transform.right * horizontal * isWalking;
            
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
            
            // Направление относительно взгляда (текущего вращения). 
            var horizontal = ExtractHorizontalInput();
            var vertical = ExtractVerticalInput();

            // Сила смещения вперед-назад
            var verticalModifier = 0f;
            switch (vertical)
            {
                case 1:
                    verticalModifier = Module.ForwardModifier;
                    break;
                case -1:
                    verticalModifier = Module.BackwardModifier;
                    break;
            }

            // Сила смещения вправо-влево
            var horizontalModifier = 0f;
            if (horizontal != 0)
                horizontalModifier = Module.SidewardsModifier;

            // Смещение изначальной траектории
            var additionalDirection = Module.Rigidbody.transform.forward * vertical * verticalModifier +
                Module.Rigidbody.transform.right * horizontal * horizontalModifier;
            
            additionalDirection = Vector3.ClampMagnitude(_targetDirection+additionalDirection,1f);
                
            Move(additionalDirection, _baseSpeed, deltaTime);
        }
    }

}