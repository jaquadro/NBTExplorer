using System;
using System.Collections.Generic;

namespace Substrate
{
    /// <summary>
    /// Provides read-only indexed access to an underlying resource.
    /// </summary>
    /// <typeparam name="T">The type of the underlying resource.</typeparam>
    public interface ICacheTable<T>
    {
        /// <summary>
        /// Gets the value at the given index.
        /// </summary>
        /// <param name="index">The index to fetch.</param>
        T this[int index] { get; }
    }

    /*internal class CacheTableArray<T> : ICacheTable<T>
    {
        private T[] _cache;

        public T this[int index]
        {
            get { return _cache[index]; }
        }

        public CacheTableArray (T[] cache)
        {
            _cache = cache;
        }
    }

    internal class CacheTableDictionary<T> : ICacheTable<T>
    {
        private Dictionary<int, T> _cache;
        private static Random _rand = new Random();

        public T this[int index]
        {
            get
            {
                T val;
                if (_cache.TryGetValue(index, out val)) {
                    return val;
                }
                return default(T);
            }
        }

        public CacheTableDictionary (Dictionary<int, T> cache)
        {
            _cache = cache;
        }
    }

    /// <summary>
    /// Provides read-only indexed access to an underlying resource.
    /// </summary>
    /// <typeparam name="T">The type of the underlying resource.</typeparam>
    public class CacheTable<T>
    {
        ICacheTable<T> _cache;

        /// <summary>
        /// Gets the value at the given index.
        /// </summary>
        /// <param name="index"></param>
        public T this[int index]
        {
            get { return _cache[index]; }
        }

        internal CacheTable (T[] cache)
        {
            _cache = new CacheTableArray<T>(cache);
        }

        internal CacheTable (Dictionary<int, T> cache)
        {
            _cache = new CacheTableDictionary<T>(cache);
        }
    }*/
}
