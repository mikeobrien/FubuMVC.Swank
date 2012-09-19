using System;
using System.Collections.Concurrent;

namespace FubuMVC.Swank
{
    public static class Func
    {
        public static Func<T1, T2> Memoize<T1, T2>(this Func<T1, T2> func)
        {
            var map = new ConcurrentDictionary<T1, T2>();
            return x =>
            {
                if (map.ContainsKey(x)) return map[x];
                var result = func(x);
                map.TryAdd(x, result);
                return result;
            };
        }
    }
}