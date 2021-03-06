﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Swank.Extensions
{
    public static class LinqExtensions
    {
        public static string Join<TItem, TResult>(
            this IEnumerable<TItem> source, Func<TItem, TResult> result, string delimiter)
        {
            return String.Join(delimiter, source.Select(result).ToArray());
        }

        public static IEnumerable<T> NullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        } 

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

        public static IEnumerable<T> TraverseMany<T>(this T source,
            Func<T, IEnumerable<T>> results)
            where T : class
        {
            var nodes = results(source).ToList();
            return nodes.Concat(nodes.SelectMany(x => x.TraverseMany(results))).ToList();
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

        public static TResult MapOrEmpty<T, TResult>(this T source, Func<T, TResult> map, TResult @default = null)
            where T : class 
            where TResult : class 
        {
            return source != null ? map(source) : @default;
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item)
        {
            return source != null ? new List<T>(source) { item } : new List<T> { item };
        }

        public static List<T> ForEach<T>(this List<T> source, Action<T, int> action)
        {
            var index = 0;
            source.ForEach(x => action(x, index++));
            return source;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source) action(item);
            return source;
        }

        public static void ShrinkMultipartKeyRight<T, TKeyPart>(
            this IEnumerable<T> source,
            Func<T, IEnumerable<TKeyPart>> key,
            Action<T, List<TKeyPart>> setKey)
        {
            var keys = source.Select(x => new
            {
                Source = x,
                ShortKey = new Stack<TKeyPart>(),
                FullKey = new Stack<TKeyPart>(key(x).ToList())
            }).ToList();
            while (true)
            {
                var multiplicates = keys.Where(x => x.FullKey.Any())
                    .Multiplicates(x => x.ShortKey.GetItemsHashCode()).ToList();
                if (multiplicates.Count == 0) break;
                multiplicates.ForEach(x =>
                {
                    var segment = x.FullKey.Pop();
                    if (!x.ShortKey.Any() || !x.ShortKey.Peek().Equals(segment)) 
                        x.ShortKey.Push(segment);
                });
            }
            keys.ForEach(x => setKey(x.Source, x.ShortKey.ToList()));
        }

        public static IEnumerable<T> Multiplicates<T, TKey>(this IEnumerable<T> source, Func<T, TKey> key)
        {
            return source.Select(x => new { Context = x, Key = key(x) })
                .GroupBy(x => x.Key)
                .Where(x => x.Count() > 1)
                .SelectMany(x => x.Select(y => y.Context));
        }

        public static int GetItemsHashCode<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any() ? 0 : source.Select(x => x.GetHashCode()).Aggregate((a, i) => a | i);
        }

        public static bool EndsWith<T>(this IEnumerable<T> source, T value)
        {
            return source != null && source.Any() && source.Last().Equals(value);
        }

        public static IEnumerable<T> Shorten<T>(this IEnumerable<T> source, int by)
        {
            if (source == null || !source.Any()) return source;
            return @by >= source.Count() ? new List<T>() : source.Take(source.Count() - @by);
        }
    }
}