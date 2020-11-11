using System;
using System.Collections.Generic;

namespace Gasanov.Eppd.Proceeders
{
    public class QueueProceeder<T>
    {
        /// <summary>
        /// Функция просчета. На вход подается одно из начений очереди.
        /// Каждый возвращаемый объект - результат комбинации предыдущих.
        /// Первый аргумент - предыдущее значение, второй - новое значение.
        /// </summary>
        private readonly Func<T,T,T> _proceedFunc;

        /// <summary>
        /// Очередь данных.
        /// </summary>
        private readonly Queue<T> _dataQueue;
        
        public QueueProceeder(Func<T,T,T> proceedFunc)
        {
            _proceedFunc = proceedFunc;
            _dataQueue = new Queue<T>();
        }

        public T Proceed()
        {
            if (_dataQueue.Count == 0)
                return default;
            
            // Возвращаемый объект.
            T obj = _dataQueue.Dequeue();
            while (_dataQueue.Count > 0)
            {
                // Временный объект очереди.
                var temp = _dataQueue.Dequeue();
                obj = _proceedFunc(obj,temp);
            }

            return obj;
        }

        /// <summary>
        /// Добавление нового объекта в очередь.
        /// </summary>
        public void Add(T obj)
        {
            _dataQueue.Enqueue(obj);    
        }

        public void Clear()
        {
            _dataQueue.Clear();
        }
    }
    
}