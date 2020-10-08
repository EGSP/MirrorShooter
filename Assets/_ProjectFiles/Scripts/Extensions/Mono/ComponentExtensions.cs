using UnityEngine;

namespace Gasanov.Extensions.Mono
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Проверяет компонент на null.
        /// При отсутствии ищет компонент типа T.
        /// Изменение происходит по ссылке.
        /// </summary>
        /// <param name="component">ссылка на текущий компонент</param>
        public static T ValidateComponentRef<T>(this Component parent, ref T component)
        {
            if (component != null)
                return component;

            component = parent.GetComponent<T>();

            return component;
        }
        
        /// <summary>
        /// Проверяет компонент на null.
        /// При отсутствии ищет компонент типа T.
        /// Изменение происходит по ссылке.
        /// </summary>
        /// <param name="component">ссылка на текущий компонент</param>
        public static T ValidateComponent<T>(this Component parent,T component)
        {
            if (component != null)
                return component;

            component = parent.GetComponent<T>();

            return component;
        }
    }
}