using Game.Configuration;
using Game.Entities.Controllers;
using Game.Entities.Modules;
using Gasanov.Eppd.Proceeders;
using UnityEngine;

namespace Game.Entities.States.Player
{
    public abstract class MoveModuleState : LogicState<MoveModuleState, PlayerMoveModule>
    {
        
        protected PlayerInputManager InputManager => Module.PlayerInputManager;
        
        protected MoveModuleState(PlayerMoveModule moveModule): base(moveModule)
        {
        }

        public override MoveModuleState ReturnThis()
        {
            return this;
        }

        public virtual void Move(Vector3 direction, float speed ,float deltaTime)
        {
            Module.Rigidbody.MovePosition(Module.Rigidbody.position +
                                              direction * speed * deltaTime );
        }

        /// <summary>
        /// Запускает анимацию.
        /// </summary>
        /// <param name="animation"></param>
        public virtual void Play(string animation)
        {
            Module.PlayerEntity.AnimationModule.Play(animation);
        }

        /// <summary>
        /// Извлекает направление ввода по вертикали (forward, backward). 
        /// </summary>
        protected int ExtractVerticalInput()
        {
            int dir = 0;
            if (InputManager.GetHold(InputSettings.MoveForward) ||
                InputManager.GetDown(InputSettings.MoveForward))
                dir++;

            if (InputManager.GetHold(InputSettings.MoveBackward) ||
                InputManager.GetDown(InputSettings.MoveBackward))
                dir--;

            return dir;
        }

        /// <summary>
        /// Извлекает направление ввода по горизонтали (rightward, leftward). 
        /// </summary>
        protected int ExtractHorizontalInput()
        {
            int dir = 0;
            if (InputManager.GetHold(InputSettings.MoveRightward) ||
                InputManager.GetDown(InputSettings.MoveRightward))
                dir++;

            if (InputManager.GetHold(InputSettings.MoveLeftward) ||
                InputManager.GetDown(InputSettings.MoveLeftward))
                dir--;

            return dir;
        }

        /// <summary>
        /// Извлекает направление по двум осям относительно объекта. Магнитуда равна 1 или 0.
        /// </summary>
        /// <returns></returns>
        protected Vector3 ExtractOverallInputDirection()
        {
            var forward = Module.Rigidbody.transform.forward;
            var rightward = Module.Rigidbody.transform.right;

            var direction = forward * ExtractVerticalInput() + rightward * ExtractHorizontalInput();
            direction = Vector3.ClampMagnitude(direction, 1);

            return direction;
        }

        /// <summary>
        /// Извлекает направление по двум осям в нулевых координатах. Магнитуда равна 1 или 0.
        /// </summary>
        protected Vector3 ExtractRawInputDirection()
        {
            var direction = Vector3.forward * ExtractVerticalInput() + Vector3.right * ExtractHorizontalInput();
            direction = Vector3.ClampMagnitude(direction, 1);

            return direction;
        }

        public bool CheckIsOnlyHoriznotalInput()
        {
            return CheckIsOnlyHoriznotalInput(ExtractOverallInputDirection());
        }
        
        public bool CheckIsOnlyHoriznotalInput(Vector3 rawDirection)
        {
            if (rawDirection.x != 0)
            {
                if (rawDirection.z == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckIsOnlyVerticalInput()
        {
            return CheckIsOnlyVerticalInput(ExtractOverallInputDirection());
        }
        
        public bool CheckIsOnlyVerticalInput(Vector3 rawDirection)
        {
            if (rawDirection.z != 0)
            {
                if (rawDirection.x == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public int CheckIsWalking()
        {
            return CheckIsWalking(ExtractOverallInputDirection());
        }
        
        public int CheckIsWalking(Vector3 inputDirection)
        {
            return inputDirection.sqrMagnitude != 0 ? 1 : 0;
        }
        
        /// <summary>
        /// Игрок пятится назад.
        /// </summary>
        public bool CheckIsBacking(Vector3 rawDirection)
        {
            return rawDirection.z == -1;
        }

    }

}