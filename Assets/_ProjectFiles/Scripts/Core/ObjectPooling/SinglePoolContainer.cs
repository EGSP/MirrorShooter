using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gasanov.Core.ObjectPooling
{
    public class SinglePoolContainer<T> : IPoolContainer<T> where T : class, IPoolObject
    {
        private SinglePoolContainer()
        {
            pool = new List<T>();
        }

        public string Name { get; private set; }

        /// <summary>
        /// Список созданных объектов
        /// </summary>
        private List<T> pool;

        private Func<int, T> initializeFunction;

        /// <summary>
        /// Добавление нового объекта. Если объект не T, то он не будет добавлен
        /// </summary>
        public void Add(T poolObject)
        {
            if (poolObject != null)
            {
                poolObject.ParentPool = this;
                
                pool.Add(poolObject);
            }
        }

        /// <summary>
        /// Возвращает предмет из пула. Может вернуть null
        /// </summary>
        /// <returns></returns>
        public T Take()
        {
            if (pool.Count != 0)
            {
                var takedObject = pool[0];
                pool.RemoveAt(0);

                takedObject.AwakeFromPool();
                return takedObject;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Задает функцию создания экземпляров и вызывает ее.
        /// </summary>
        /// <param name="initializeFunc"> int - это передаваемый индекс из цикла. </param>
        public void SetInitializator(int invokeCount, Func<int,T> initializeFunc)
        {
            Clean();

            for (var i = 0; i < invokeCount; i++)
            {
                var instance = initializeFunc(i);
                instance.ReturnAction = () => Add(instance);
                
                Add(instance);
            }

            initializeFunction = initializeFunc;
        }

        public static SinglePoolContainer<T> CreateContainer(string poolName)
        {
            if (PoolManager.GetContainer(poolName) != null)
                throw new System.Exception($"Пул {poolName} уже был создан");

            var container = new SinglePoolContainer<T>
            {
                Name = poolName
            };

            PoolManager.AddContainer(container);
            return container;
        }

        public void Clean()
        {
            for (var i = 0; i < pool.Count; i++)
            {
                pool[i].Dispose();
            }
        }

        /// <summary>
        /// Уничтожение пула и объектов с ним связанных.
        /// </summary>
        public void Destroy()
        {
            Clean();
            pool = null;
            initializeFunction = null;
            PoolManager.RemoveContainer(this);
        }

    }
}