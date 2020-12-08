using System;

namespace Egsp.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class LazyInstanceAttribute : Attribute
    {
        public LazyInstanceAttribute()
        {
            AllowLazyInstance = true;
        }
        
        public LazyInstanceAttribute(bool allowLazyInstance)
        {
            AllowLazyInstance = allowLazyInstance;
        }
        
        public bool AllowLazyInstance { get; private set; }
    }
}