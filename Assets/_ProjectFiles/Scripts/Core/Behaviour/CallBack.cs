using System;

namespace Gasanov.Core
{
    /// <summary>
    /// Объект для реализации функций с обратным вызовом.
    /// </summary>
    public class CallBack<TValue>
    {
        /// <summary>
        /// Событие обратного вызова.
        /// </summary>
        public event Action<TValue> on;
        
        /// <summary>
        /// Вызывает событие "on".
        /// </summary>
        public void On(TValue value)
        {
            on(value);
        }

        /// <summary>
        /// Подписывает действие на событие.
        /// Возвращает ссылку на себя.
        /// </summary>
        public CallBack<TValue> With(Action<TValue> onCallAction)
        {
            on += onCallAction;
            return this;
        }
    }
    
    /// <summary>
    /// Объект для реализации функций с обратным вызовом.
    /// </summary>
    public class CallBack
    {
        /// <summary>
        /// Событие обратного вызова.
        /// </summary>
        public event Action on = delegate {  };
        
        /// <summary>
        /// Вызывает событие "on".
        /// </summary>
        public void On()
        {
            on();
        }
        
        /// <summary>
        /// Подписывает действие на событие.
        /// Возвращает ссылку на себя.
        /// </summary>
        public CallBack With(Action onCallAction)
        {
            on += onCallAction;
            return this;
        }
    }
}