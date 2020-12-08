using System;

namespace Egsp.Core
{
    public class LazyInstanceException : Exception
    {
        public LazyInstanceException(Type type)
            : base($"Lazy instance not allowed for {type.FullName}")
        {
            
        }
    }
}