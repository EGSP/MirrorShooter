using System;

namespace Gasanov.Core.Pooling
{
    public interface IPoolObject
    {
        /// <summary>
        /// Родительский контейнер.
        /// </summary>
        IPoolContainer ParentPool { get; set; }

        /// <summary>
        /// Инструкция помещения объекта обратно в пул.
        /// Устанавливается из вне, т.к. пулы могут быть разными.
        /// </summary>
        Action ReturnInstruction { get; set; }
        
        /// <summary>
        /// Метод активации при выходе из пула.
        /// </summary>
        void AwakeFromPool();
        
        /// <summary>
        /// Возвращение в пул.
        /// </summary>
        void ReturnToPool();

        /// <summary>
        /// Удаление объекта через пул.
        /// </summary>
        void DisposeByPool();
    }
}