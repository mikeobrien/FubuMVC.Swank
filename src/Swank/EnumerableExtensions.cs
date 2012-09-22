using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Swank
{
    public static class EnumerableExtensions
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

        public static TResult WhenNotNull<TSource, TResult>(this TSource value, Func<TSource, TResult> returnThis)
            where TSource : class
        {
            return value.WhenNotNull(returnThis, default(TResult));
        }

        public static TResult WhenNotNull<TSource, TResult>(this TSource value, Func<TSource, TResult> returnThis, TResult orThisDefault)
            where TSource : class
        {
            return value != null ? returnThis(value) : orThisDefault;
        }
    }
}