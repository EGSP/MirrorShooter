using System;

namespace Gasanov.Core
{
    public interface IHandledDisposable: IDisposable 
    {
        /// <summary>
        /// Вызывается при уничтожении объекта.
        /// </summary>
        event Action OnDispose;
    }
}