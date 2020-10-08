using System;

namespace Gasanov.Core.ObjectPooling
{
    public interface IPoolObject: IDisposable
    {
        /// <summary>
        /// Родительский контейнер
        /// </summary>
        IPoolContainer ParentPool { get; set; }

        /// <summary>
        /// Инструкция помещения объекта обратно в пул
        /// </summary>
        Action ReturnAction { get; set; }
        
        /// <summary>
        /// Возвращение в пул
        /// </summary>
        void ReturnToPool();

        /// <summary>
        /// Метод активации при выходе из пула
        /// </summary>
        void AwakeFromPool();
    }
}