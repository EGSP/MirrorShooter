using Game.Configuration;
using Game.Entities.Controllers;
using Game.Entities.Modules;
using Gasanov.Eppd.Proceeders;
using UnityEngine;

namespace Game.Entities.States.Player
{
    public abstract class MoveModuleState
    {
        // public enum DirectionInput
        // {
        //     None,
        //     Forward,
        //     Backward,
        //     Rightward,
        //     Leftward
        // }
        
        protected readonly PlayerMoveModule MoveModule;
        protected PlayerInputManager InputManager => MoveModule.PlayerInputManager;

        /// <summary>
        /// Следующее состояние. Может быть пустым, тогда состояние само определяет следующее.
        /// </summary>
        protected MoveModuleState NextState { get; private set; }

        protected MoveModuleState(PlayerMoveModule moveModule)
        {
            MoveModule = moveModule;
        }

        public abstract MoveModuleState FixedUpdateOnServer(float deltaTime);

        public virtual void Move(Vector3 direction, float speed ,float deltaTime)
        {
            MoveModule.Rigidbody.MovePosition(MoveModule.Rigidbody.position +
                                              direction * speed * deltaTime );
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
        /// Извлекает направление по двум осям. Магнитуда равна 1 или 0.
        /// </summary>
        /// <returns></returns>
        protected Vector3 ExtractOverallInputDirection()
        {
            var forward = MoveModule.Rigidbody.transform.forward;
            var rightward = MoveModule.Rigidbody.transform.right;

            var direction = forward * ExtractVerticalInput() + rightward * ExtractHorizontalInput();
            direction = Vector3.ClampMagnitude(direction, 1);

            return direction;
        }

        public void SetNext(MoveModuleState nextState)
        {
            NextState = nextState;
        }
    }

}