using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FubuMVC.Swank.Extensions
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

        public static TResult Apply<TResult>(this List<Action<TResult>> applications, TResult result)
        {
            applications.ForEach(x => x(result));
            return result;
        }

        public static TResult Apply<T, TResult>(this List<Action<T, TResult>> applications, T arg, TResult result)
        {
            applications.ForEach(x => x(arg, result));
            return result;
        }

        public static TResult Apply<T1, T2, TResult>(this List<Action<T1, T2, TResult>> applications, T1 arg1, T2 arg2, TResult result)
        {
            applications.ForEach(x => x(arg1, arg2, result));
            return result;
        }

        public static TResult Apply<T1, T2, T3, TResult>(this List<Action<T1, T2, T3, TResult>> applications, T1 arg1, T2 arg2, T3 arg3, TResult result)
        {
            applications.ForEach(x => x(arg1, arg2, arg3, result));
            return result;
        }
    }
}