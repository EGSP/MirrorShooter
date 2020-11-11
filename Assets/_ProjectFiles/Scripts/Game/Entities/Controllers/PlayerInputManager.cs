using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Entities.Controllers
{
    public class PlayerInputManager
    {
        #region Objects
        public enum KeyCodeState
        {
            Down,
            Up,
            Hold
        }
        
        public class KeyCodeHandler
        {
            public readonly KeyCode Code;
            
            public KeyCodeHandler(KeyCode code, KeyCodeState state)
            {
                Code = code;
                State = state;
            }

            /// <summary>
            /// Текущее состояние клавиши.
            /// </summary>
            public KeyCodeState State { get; set; }
            
            /// <summary>
            /// Время удерживания клавиши.
            /// </summary>
            public float TimeHold { get; set; }
        }

        #endregion

        public List<KeyCodeHandler> KeyCodeHandlers;
        
        public PlayerInputManager()
        {
            KeyCodeHandlers = new List<KeyCodeHandler>();
        }

        /// <summary>
        /// Задержка перед удержанием.
        /// </summary>
        public float HoldSpan = 0.1f; 
        
        /// <summary>
        /// Нужно удостовериться, что все кому нужно получили данные по кнопкам.
        /// </summary>
        public void LateUpdate(float deltaTime)
        {
            for (var i = KeyCodeHandlers.Count - 1; i > -1; i--)
            {
                var handler = KeyCodeHandlers[i];

                // Если идет удержание.
                if (handler.State == KeyCodeState.Hold)
                {
                    // Debug.Log($"Holding {handler.Code}");
                    handler.TimeHold += deltaTime;
                    return;
                }

                // Новое удержание клавиши.
                if (handler.State == KeyCodeState.Down)
                {
                    handler.TimeHold += deltaTime;

                    // Кнопка удерживается.
                    if (handler.TimeHold > HoldSpan)
                    {
                        // Debug.Log($"New hold {handler.Code}");
                        handler.State = KeyCodeState.Hold;
                    }
                }
                
                // Если человек отжал кнопку, то ее нужно удалить из списка.
                if (handler.State == KeyCodeState.Up)
                {
                    // Debug.Log($"New up {handler.Code}");
                    KeyCodeHandlers.RemoveAt(i);
                }
                    
            }
        }

        /// <summary>
        /// Обработка нового нажатия.
        /// </summary>
        public void NewDown(KeyCode keyCode)
        {
            var coincidence = KeyCodeHandlers.FirstOrDefault(x => 
                x.Code == keyCode);

            if (coincidence == null)
            {
                // Ставим на учет кнопку.
                coincidence = new KeyCodeHandler(keyCode, KeyCodeState.Down);
                KeyCodeHandlers.Add(coincidence);
                
                return;
            }
            // Если кнопка на учете.
            else
            {
                if(coincidence.State == KeyCodeState.Up)
                    coincidence.State = KeyCodeState.Down;
            }
        }

        /// <summary>
        /// Обработка нового отжатия.
        /// </summary>
        /// <param name="keyCode"></param>
        public void NewUp(KeyCode keyCode)
        {
            var coincidence = KeyCodeHandlers.FirstOrDefault(x => 
                x.Code == keyCode);
            
            if (coincidence == null)
            {
                // Ставим на учет кнопку.
                coincidence = new KeyCodeHandler(keyCode, KeyCodeState.Up);
                KeyCodeHandlers.Add(coincidence);
                
                return;
            }
            // Если кнопка на учете.
            else
            {
                if (coincidence.State != KeyCodeState.Up)
                    coincidence.State = KeyCodeState.Up;
            }
        }

        /// <summary>
        /// Нажата ли кнопка.
        /// </summary>
        public bool GetDown(KeyCode keyCode)
        {
            return KeyCodeHandlers.Exists(x =>
                x.Code == keyCode && x.State == KeyCodeState.Down);
        }

        /// <summary>
        /// Отжата ли кнопка.
        /// </summary>
        public bool GetUp(KeyCode keyCode)
        {
            return KeyCodeHandlers.Exists(x =>
                x.Code == keyCode && x.State == KeyCodeState.Up);
        }

        /// <summary>
        /// Удерживается ли кнопка.
        /// </summary>
        public bool GetHold(KeyCode keyCode)
        {
            return KeyCodeHandlers.Exists(x =>
                x.Code == keyCode && x.State == KeyCodeState.Hold);
        }

        public bool GetHoldOrDown(KeyCode keyCode)
        {
            var keyHandler = KeyCodeHandlers.FirstOrDefault(x => x.Code == keyCode);

            if (keyHandler == null)
                return false;

            if (keyHandler.State == KeyCodeState.Down || keyHandler.State == KeyCodeState.Hold)
                return true;

            return false;
        }

        /// <summary>
        /// Убирает ключ из списка.
        /// </summary>
        public void RemoveKey(KeyCode keyCode)
        {
            for (var i = 0; i < KeyCodeHandlers.Count; i++)
            {
                var handler = KeyCodeHandlers[i];

                if (handler.Code == keyCode)
                {
                    KeyCodeHandlers.RemoveAt(i);
                }
            }
        }
        
    }
}