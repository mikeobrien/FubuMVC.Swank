using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Swank.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<TResult> SelectDistinct<TItem, TResult>(
            this IEnumerable<TItem> source, Func<TItem, TResult> result)
        {
            return source.Select(result).Distinct();
        } 

        public static IEnumerable<TItem> DistinctBy<TItem, TCompare>(
            this IEnumerable<TItem> source, Func<TItem, TCompare> compare)
        {
            return source.GroupBy(compare).Select(x => x.First());
        } 

        public static IEnumerable<TItem> DistinctBy<TItem, TCompare1, TCompare2>(
            this IEnumerable<TItem> source, Func<TItem, TCompare1> compare1, Func<TItem, TCompare2> compare2)
        {
            return source.GroupBy(compare1).SelectMany(x => x.GroupBy(compare2).Select(y => y.First()));
        }

        public static IEnumerable<TResult> OuterJoin<TItem, TKey, TResult>(
            this IEnumerable<TItem> join1, 
            IEnumerable<TItem> join2, 
            Func<TItem, TKey> key, Func<TKey, IEnumerable<TItem>, TResult> selector)
        {
            return (join1 ?? Enumerable.Empty<TItem>()).Concat(join2 ?? Enumerable.Empty<TItem>()).GroupBy(key, selector);
        }

        public static IEnumerable<T> Traverse<T>(this T source, Func<T, T> result)
            where T : class
        {
            var node = source;
            while (node != null)
            {
                yield return node;
                node = result(node);
            }
        }

        public static TValue Map<T, TValue>(this T source, Func<T, TValue> map)
        {
            return map(source);
        }

        public static IEnumerable<TResult> SelectOrEmpty<T, TResult>(
            this IEnumerable<T> source, Func<T, TResult> map)
        {
            return (source ?? Enumerable.Empty<T>()).Select(map);
        }

        public static TResult MapOrEmpty<T, TResult>(this T source, Func<T, TResult> map)
            where T : class 
            where TResult : class 
        {
            return source != null ? map(source) : null;
        }
    }
}