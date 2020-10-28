using System;
using Gasanov.Extensions;

namespace Gasanov.Eppd.Data
{
    public abstract class DataBlock : IDisposable
    {
        public bool IsDisposed { get; protected set; }

        public virtual void Dispose()
        {
            IsDisposed = true;
        }
    }

    public interface ICloneable<T>
    {
        T Clone();
    }
}