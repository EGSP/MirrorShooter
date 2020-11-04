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
        private readonly bool _longJump;
         
        /// <param name="baseSpeed">Базовая скорость для перемещения во время полета</param>
        /// <param name="startSpeed">Стартовая скорость. 0 - если прыжок с места, 1 - если во время движения</param>
        public MoveModuleJump(PlayerMoveModule moveModule, float baseSpeed, bool longJump = false) : base(moveModule)
        {
            MoveModule.JumpInitiated();
            
            MoveModule.Rigidbody.AddForce(MoveModule.Rigidbody.transform.up * MoveModule.JumpForce,
                ForceMode.Impulse);
            
            var direction = ExtractOverallInputDirection();
            int isWalking = direction.sqrMagnitude != 0 ? 1 : 0;

            var vertical = ExtractVerticalInput();
            _targetDirection += MoveModule.Rigidbody.transform.forward * vertical * isWalking;

            var horizontal = ExtractHorizontalInput();
            _targetDirection += MoveModule.Rigidbody.transform.right * horizontal * isWalking;
            
            _baseSpeed = baseSpeed;
            _longJump = longJump;
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

            // Если длинный прыжок, то игрок не может изменить свою траекторию.
            if (_longJump)
            {
                Move(_targetDirection, _baseSpeed, deltaTime);
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
            var additionalDirection = MoveModule.Rigidbody.transform.forward * vertical * verticalModifier +
                MoveModule.Rigidbody.transform.right * horizontal * horizontalModifier;
            
            additionalDirection = Vector3.ClampMagnitude(_targetDirection+additionalDirection,1f);
                
            Move(additionalDirection, _baseSpeed, deltaTime);
        }
    }

}