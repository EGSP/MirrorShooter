using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasanov.Core.ObjectPooling
{
    /// <summary>
    /// Пул объектов универсального типа
    /// </summary>
    interface IPoolContainer<T>: IPoolContainer where T : class, IPoolObject
    {
        void Add(T poolObject);

        T Take();
    }
}
