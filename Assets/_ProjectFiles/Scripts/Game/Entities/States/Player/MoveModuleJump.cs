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
        public MoveModuleJump(PlayerMoveModule moveModule, float baseSpeed, float startSpeed) : base(moveModule)
        {
            MoveModule.JumpInitiated();
            
            MoveModule.Rigidbody.AddForce(MoveModule.Rigidbody.transform.up * MoveModule.JumpForce,
                ForceMode.Impulse);

            var vertical = ExtractVerticalInput();
            _targetDirection += MoveModule.Rigidbody.transform.forward * vertical * startSpeed;

            var horizontal = ExtractHorizontalInput();
            _targetDirection += MoveModule.Rigidbody.transform.right * horizontal * startSpeed;
            
            _baseSpeed = baseSpeed;
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

            // Если в полете.
            return this;
        }

        private void ProcessMovement(float deltaTime)
        {
            var bodyCollider = MoveModule.PlayerEntity.BodyEntity.Collider;
            if (Physics.Raycast(bodyCollider.center+MoveModule.Rigidbody.position,
                MoveModule.Rigidbody.transform.forward,
                bodyCollider.radius + MoveModule.WallCheckDistance, MoveModule.GroundLayer))
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
                    verticalModifier = MoveModule.ForwardModifier;
                    break;
                case -1:
                    verticalModifier = MoveModule.BackwardModifier;
                    break;
            }

            // Сила смещения вправо-влево
            var horizontalModifier = 0f;
            if (horizontal != 0)
                horizontalModifier = MoveModule.SidewardsModifier;

            // Смещение изначальной траектории
            _targetDirection += MoveModule.Rigidbody.transform.forward * vertical * verticalModifier +
                MoveModule.Rigidbody.transform.right * horizontal * horizontalModifier;
            
            _targetDirection = Vector3.ClampMagnitude(_targetDirection,1f);
                
            Move(_targetDirection, _baseSpeed, deltaTime);
        }
    }

}