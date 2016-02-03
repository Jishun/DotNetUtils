using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DotNetUtils
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Filters out null elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> collection)
            where T : class
        {
            return collection.Where(t => t != null);
        }

        /// <summary>
        /// Executes the given action for each member of the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        [System.Diagnostics.DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        /// <summary>
        /// Executes the given action for each member of the collection using the ThreadPool.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        /// <param name="continueOnException">Function to determine whether to continue on an exception</param>
        public static void ForEachParallel<T>(this IEnumerable<T> collection, Action<T> action, Func<Exception, bool> continueOnException)
        {
            using (var helper = new ForEachParallelHelper(continueOnException))
            {
                helper.Go(collection, action);
            }
        }

        /// <summary>
        /// Executes the given action for each member of the collection using the ThreadPool.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        /// <param name="continueOnException">Function to determine whether to continue on an exception</param>
        public static void ForEachParallel<T>(this IEnumerable<T> collection, Action<T> action, bool continueOnException)
        {
            collection.ForEachParallel(action, e => continueOnException);
        }

        /// <summary>
        /// Executes the given action for each member of the collection using the ThreadPool.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        public static void ForEachParallel<T>(this IEnumerable<T> collection, Action<T> action)
        {
            collection.ForEachParallel(action, false);
        }

        /// <summary>
        /// Runs all functions in the collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public static void RunEach(this IEnumerable<Action> collection)
        {
            collection.ForEach(x => x());
        }

        /// <summary>
        /// Assigns each value from the right to the corresponding element in the left according to the action.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="left">The collection to assign to.</param>
        /// <param name="right">The collection to assign from.</param>
        /// <param name="action">Action that handles the assignment.</param>
        public static void Assign<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right, Action<T1, T2> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var e1 = left.EmptyIfNull().GetEnumerator();
            var e2 = right.EmptyIfNull().GetEnumerator();
            while (e1.MoveNext() && e2.MoveNext())
            {
                action(e1.Current, e2.Current);
            }
        }

        /// <summary>
        /// Executes the given action for each member of the collection as it is enumerated.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        [DebuggerStepThrough]
        public static IEnumerable<T> Do<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
                yield return item;
            }
        }

        /// <summary>
        /// Append an item to IEmumerable collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item.</param>
        [DebuggerStepThrough]
        public static IEnumerable<T> Append<T>(this IEnumerable<T> collection, T item)
        {
            foreach (var cur in collection)
            {
                yield return cur;
            }
            yield return item;
        }

        /// <summary>
        /// Prepend an item to IEmumerable collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item.</param>
        [DebuggerStepThrough]
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> collection, T item)
        {
            yield return item;
            foreach (var cur in collection)
            {
                yield return cur;
            }
        }

        /// <summary>
        /// Executes the given action for each member of the collection as it is enumerated; ends enumeration if false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="interval">How often to check the condition</param>
        /// <param name="action">The action.</param>
        public static IEnumerable<T> CheckEvery<T>(this IEnumerable<T> collection, int interval, Func<T, bool> action)
        {
            var i = 0;
            return collection.TakeWhile(item => (i++ % interval != 0) || action(item));
        }

        /// <summary>
        /// Check if the 2 collections are both empty or contains the same elements
        /// </summary>
        /// <typeparam name="T">the Element type</typeparam>
        /// <param name="srcCollection">source collection</param>
        /// <param name="compareTo">the other collection comparing to</param>
        /// <returns>true if all matches</returns>
        public static bool CollectionEquals<T>(this IEnumerable<T> srcCollection, IEnumerable<T> compareTo)
        {
            if (srcCollection.IsNullOrEmpty())
            {
                return compareTo.IsNullOrEmpty();
            }
            if (compareTo.IsNullOrEmpty())
            {
                return false;
            }
            return compareTo.Count() == srcCollection.Count() && !compareTo.Any(item => !srcCollection.Contains(item));
        }

        /// <summary>
        /// Check if the 2 collections are both empty or contains the exact same contents
        /// </summary>
        /// <typeparam name="T">the Element type</typeparam>
        /// <param name="srcCollection">source collection</param>
        /// <param name="compareTo">the other collection comparing to</param>
        /// <returns>true if all matches</returns>
        public static bool EqualsTo<T>(this IEnumerable<T> srcCollection, IEnumerable<T> compareTo) where T : IEquatable<T>
        {
            if (srcCollection.IsNullOrEmpty())
            {
                return compareTo.IsNullOrEmpty();
            }
            if (compareTo.IsNullOrEmpty())
            {
                return false;
            }
            return compareTo.Count() == srcCollection.Count() && compareTo.All(item => srcCollection.Any(i => i.Equals(item)));
        }

        /// <summary>
        /// Break a list of items into chunks of a specific size
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            if (source != null && chunksize > 0)
            {
                var list = new List<T>();
                foreach (var item in source)
                {
                    list.Add(item);
                    if (list.Count == chunksize)
                    {
                        yield return list;
                        list = new List<T>();
                    }
                }
            }
        }


        /// <summary>
        /// Adds all items to the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items.</param>
        [DebuggerStepThrough]
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            items.ForEach(collection.Add);
        }

        /// <summary>
        /// Checks whether the Enumerable is null or contains no items.
        /// </summary>
        /// <typeparam name="T">The type of objects in the collection</typeparam>
        /// <param name="instance">The instance of collection</param>
        /// <returns>True if the instance is null or empty.</returns>
        [DebuggerStepThrough]
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> instance)
        {
            return ((instance == null) || (!instance.Any()));
        }

        /// <summary>
        /// Returns the specified collection, or an empty collection if it's null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The collection.</param>
        [DebuggerStepThrough]
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> instance)
        {
            return instance ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Returns the specified collection, or an null if it's an empty collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The collection.</param>
        [DebuggerStepThrough]
        public static IEnumerable<T> NullIfEmpty<T>(this IEnumerable<T> instance)
        {
            return instance == null ? null : instance.Any() ? instance : null;
        }

        /// <summary>
        /// Returns the given item as the sole item in an IEnumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item to return.</param>
        /// <returns>An IEnumerable containing the specified item.</returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> Single<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Check if the collection contains more than number of count items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static bool MoreThan<T>(this IEnumerable<T> item, int count)
        {
            if (item == null)
            {
                return false;
            }
            foreach (var i in item)
            {
                if (count > 0)
                {
                    count--;
                    continue;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the FirstOrdefault item's specific property if the item exists, otherwise return default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TP"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <param name="propertySelector"></param>
        /// <returns></returns>
        public static TP PropertyOfFirstOrDefault<T, TP>(this IEnumerable<T> collection, Func<T, bool> predicate, Func<T, TP> propertySelector)
        {
            if (collection == null)
            {
                return default(TP);
            }
            var item = collection.FirstOrDefault(predicate);
            return item == null ? default(TP) : propertySelector(item);
        }

        /// <summary>
        /// Get the SingleOrdefault item's specific property if the item exists, otherwise return default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TP"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <param name="propertySelector"></param>
        /// <returns></returns>
        public static TP PropertyOfSingleOrDefault<T, TP>(this IEnumerable<T> collection, Func<T, bool> predicate, Func<T, TP> propertySelector)
        {
            if (collection == null)
            {
                return default(TP);
            }
            var item = collection.SingleOrDefault(predicate);
            return item == null ? default(TP) : propertySelector(item);
        }

        /// <summary>
        /// Get the LastOrdefault item's specific property if the item exists, otherwise return default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TP"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <param name="propertySelector"></param>
        /// <returns></returns>
        public static TP PropertyOfLastOrDefault<T, TP>(this IEnumerable<T> collection, Func<T, bool> predicate, Func<T, TP> propertySelector)
        {
            if (collection == null)
            {
                return default(TP);
            }
            var item = collection.LastOrDefault(predicate);
            return item == null ? default(TP) : propertySelector(item);
        }


        /// <summary>
        /// Gets current collection with null check. 
        /// If the collection is null, this will return an empty collection
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="collection">The input collection</param>
        /// <returns>Collection of T element</returns>
        public static IEnumerable<T> SafeGet<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Convert an IEnumerable into HashSet
        /// </summary>
        /// <typeparam name="T">collection item type</typeparam>
        /// <param name="src">collection</param>
        /// <returns>Hashset</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> src, bool autoSkipDups = false)
        {
            var hash = new HashSet<T>();
            foreach (var item in src)
            {
                if (!autoSkipDups || !hash.Contains(item))
                {
                    hash.Add(item);
                }
            }
            return hash;
        }

        /// <summary>
        /// Convert to array with null check
        /// If collection is null, will return an empty array
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="collection">Input collection</param>
        /// <returns>Array of type T</returns>
        public static T[] SafeToArray<T>(this IEnumerable<T> collection)
        {
            return collection.SafeGet().ToArray();
        }

        /// <summary>
        /// Returns sum of given collection, if collection is null or no item matches selector, it will return null
        /// </summary>
        public static decimal? NullableSum<TItem>(this IEnumerable<TItem> collection, Func<TItem, decimal?> selector)
        {
            return collection.IsNullOrEmpty() ? (decimal?)null : collection.Sum(selector);
        }

        /// <summary>
        /// distinct a collection by specific field
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        private sealed class ForEachParallelHelper : IDisposable
        {
            private readonly Object lockObject = new object();
            private readonly Func<Exception, bool> continueOnException;
            private readonly EventWaitHandle eventCompleted = new EventWaitHandle(false, EventResetMode.ManualReset);

            private int outstandingWorkItems;
            private bool complete;

            public ForEachParallelHelper(Func<Exception, bool> continueOnException)
            {
                this.continueOnException = continueOnException ?? (e => false);
            }

            ~ForEachParallelHelper()
            {
                this.Dispose(false);
            }

            public void Go<T>(IEnumerable<T> collection, Action<T> action)
            {
                using (var enumerator = collection.GetEnumerator())
                {
                    var initialTaskCount = Environment.ProcessorCount + 1;
                    for (int i = 0; i < initialTaskCount; i++)
                    {
                        this.Queue(enumerator, action);
                    }

                    this.eventCompleted.WaitOne();
                }
            }

            private void Queue<T>(IEnumerator<T> enumerator, Action<T> action)
            {
                Interlocked.Increment(ref this.outstandingWorkItems);
                ThreadPool.QueueUserWorkItem(o => this.Task(enumerator, action));
            }

            private void Task<T>(IEnumerator<T> enumerator, Action<T> action)
            {
                try
                {
                    // no longer executing
                    if (this.complete)
                    {
                        return;
                    }

                    // Note: this will queue up more tasks than there are items in the enumerator
                    // This is intentional because we can't tell that we are at the end of the enumerator
                    // until we call MoveNext, but at the same time we don't want to call MoveNext
                    // until we are ready to perform the action so that any lazy-load work done by
                    // the enumerator is done as close to the action as possible.
                    // The additional items queued are very small and exit at the first "if" statement.
                    // The .NET 4.0 ThreadPool will enable canceling of queued items -- although by
                    // then we'll also have the Task Parallel Library.
                    this.Queue(enumerator, action);

                    T current;
                    lock (this.lockObject)
                    {
                        if (!enumerator.MoveNext())
                        {
                            this.complete = true;
                            return;
                        }
                        current = enumerator.Current;
                    }

                    try
                    {
                        action(current);
                    }
                    catch (Exception e)
                    {
                        // end processing on exception
                        this.complete = !this.continueOnException(e);
                    }
                }
                finally
                {
                    if (Interlocked.Decrement(ref this.outstandingWorkItems) == 0)
                    {
                        this.eventCompleted.Set();
                    }
                }
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.eventCompleted.Close();
                }
            }
        }

        public static void AddIfCondition<T>(this ICollection<T> collection,T item, Func<T, bool> condition)
        {
            if (collection!=null && condition!=null && condition(item))
            {
                collection.Add(item);
            }
        }
    }
}
