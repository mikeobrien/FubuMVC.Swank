using System;

namespace FubuMVC.Swank.Documentation
{
    public class LazyCache<T>
    {
        private static readonly object _mutex = new object();
        private static Lazy<T> _lazy; 

        public void UseFactory(Func<T> factory)
        {
            if (_lazy != null) return;
            lock (_mutex)
            {
                if (_lazy != null) return;
                _lazy = new Lazy<T>(factory);
            }
        }

        public T Value
        {
            get { return _lazy.Value; }
        }
    }
}