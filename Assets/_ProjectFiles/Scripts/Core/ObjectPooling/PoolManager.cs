using System.Collections.Generic;
using System.Linq;

namespace Gasanov.Core.ObjectPooling
{
    public static class PoolManager
    {
        static PoolManager()
        {
            _containers = new List<IPoolContainer>();
        }
        
        private static List<IPoolContainer> _containers;

        /// <summary>
        /// Получение контейнера пула из существующих по названию. Может вернуть Null. 
        /// </summary>
        public static IPoolContainer GetContainer (string poolName)
        {
            return _containers.FirstOrDefault(x => x.Name == poolName);
        }
        
        /// <summary>
        /// Получение контейнера пула из существующих по названию, по первому вхождению.
        /// Может вернуть Null. 
        /// </summary>
        public static T GetContainer<T> (string poolName) where T: class
        {
            // Два as, т.к. контейнеры могут иметь одно имя, но разный тип (сейчас as одно)
            return _containers.FirstOrDefault(x =>
                (x.Name == poolName)) as T;
        }

        /// <summary>
        /// Получение контейнера пула из существующих по названию и типу, по первому вхождению.
        /// Может вернуть Null. 
        /// </summary>
        public static T GetContainerWithType<T>(string poolName) where T : class
        {
            return _containers.FirstOrDefault(x =>
                (x.Name == poolName) && (x as T != null))as T;
        }

        /// <summary>
        /// Добавление нового пула
        /// </summary>
        public static void AddContainer(IPoolContainer container)
        {
            if (container != null)
                _containers.Add(container);
        }

        public static void RemoveContainer(IPoolContainer container)
        {
            if (container != null)
                _containers.Remove(container);
        }
    }
}