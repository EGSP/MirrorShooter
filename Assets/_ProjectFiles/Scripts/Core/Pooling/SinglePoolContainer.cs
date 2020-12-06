using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gasanov.Core.Pooling
{
    public class SinglePoolContainer<T> : IPoolContainer<T> where T : MonoBehaviour, IPoolObject
    {
        private SinglePoolContainer()
        {
            _pool = new Queue<T>();
        }

        public string Name { get; private set; }

        /// <summary>
        /// Список созданных объектов.
        /// </summary>
        private Queue<T> _pool;

        private Func<int, T> _initializeFunction;

        private GameObject _sceneProvider;

        /// <summary>
        /// Добавление нового объекта. Если объект не T, то он не будет добавлен
        /// </summary>
        public void Add(T poolObject)
        {
            if (poolObject != null)
            {
                poolObject.ParentPool = this;
                poolObject.transform.parent = _sceneProvider.transform;
                
                _pool.Enqueue(poolObject);
            }
        }

        /// <summary>
        /// Возвращает предмет из пула. Может вернуть null
        /// </summary>
        /// <returns></returns>
        public T Take()
        {
            if (_pool.Count != 0)
            {
                var takedObject = _pool.Dequeue();

                takedObject.transform.parent = null;
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
        public void InitializePool(int pooledObjects, Func<int,T> initializeFunc)
        {
            Clean();

            for (var i = 0; i < pooledObjects; i++)
            {
                var instance = initializeFunc(i);
                instance.ReturnInstruction = () => Add(instance);
                instance.ReturnToPool();
            }

            _initializeFunction = initializeFunc;
        }

        public static SinglePoolContainer<T> CreateContainer(string poolName)
        {
            if (PoolManager.GetContainer(poolName) != null)
                throw new System.Exception($"Пул {poolName} уже был создан");

            var container = new SinglePoolContainer<T>
            {
                Name = poolName,
                _sceneProvider = new GameObject($"SinglePool [{typeof(T).Name}]")
            };

            PoolManager.AddContainer(container);
            return container;
        }

        public void Clean()
        {
            foreach (var obj in _pool)
            {
                obj.DisposeByPool();
            }
        }

        /// <summary>
        /// Уничтожение пула и объектов с ним связанных.
        /// </summary>
        public void Destroy()
        {
            Clean();
            _pool = null;
            _initializeFunction = null;
            UnityEngine.Object.Destroy(_sceneProvider);
            PoolManager.RemoveContainer(this);
        }

    }
}