using System;
using System.Collections.Generic;
using GraphShape.Controls;
using JetBrains.Annotations;

namespace GraphShape.Helpers
{
    /// <summary>
    /// Pool of objects.
    /// </summary>
    /// <typeparam name="T">Pool object type.</typeparam>
    public class ObjectPool<T>
        where T : class, IPoolObject, new()
    {
        private const int PoolSize = 1024;

        [NotNull]
        private readonly object _lock = new object();

        [NotNull, ItemNotNull]
        private readonly Queue<T> _pool = new Queue<T>();

        private readonly bool _allowPoolGrowth;
        private readonly int _initialPoolSize;
        private int _activePoolObjectCount;

        /// <summary>
        /// Pool constructor, pool will allow growth.
        /// </summary>
        public ObjectPool()
            : this(PoolSize, true)
        {
        }

        /// <summary>
        /// Pool constructor.
        /// </summary>
        /// <param name="initialPoolSize">Initial pool size.</param>
        /// <param name="allowPoolGrowth">Allow pool growth or not.</param>
        public ObjectPool(int initialPoolSize, bool allowPoolGrowth)
        {
            _initialPoolSize = initialPoolSize;
            _allowPoolGrowth = allowPoolGrowth;

            InitializePool();
        }

        /// <summary>
        /// Fills the pool with objects.
        /// </summary>
        private void InitializePool()
        {
            // Adds some objects to the pool
            for (int i = 0; i < _initialPoolSize; ++i)
                CreateObject();
        }

        /// <summary>
        /// Creates a new pool object if the pool is not full
        /// or allow growth and adds it to the pool.
        /// </summary>
        /// <returns>
        /// Returns with the newly created object or null if the pool is full.
        /// </returns>
        [CanBeNull]
        private T CreateObject()
        {
            if (_activePoolObjectCount >= _initialPoolSize && !_allowPoolGrowth)
                return null;

            var newObject = new T();
            newObject.Disposing += OnPoolObjectDisposing;

            Add(newObject);

            return newObject;
        }

        /// <summary>
        /// Adds the <paramref name="poolObject"/> to the pool and increases the actual pool size.
        /// </summary>
        /// <param name="poolObject">The object which should be added to the pool.</param>
        private void Add([NotNull] T poolObject)
        {
            if (poolObject is null)
                throw new ArgumentNullException(nameof(poolObject));

            lock (_lock)
            {
                _pool.Enqueue(poolObject);
                ++_activePoolObjectCount;
            }
        }

        /// <summary>
        /// It puts back the disposed poolObject into the pull.
        /// </summary>
        /// <param name="sender">The disposed pool object.</param>
        private void OnPoolObjectDisposing([NotNull] object sender)
        {
            lock (_lock)
            {
                var poolObject = (T) sender;
                --_activePoolObjectCount;
                if (_pool.Count < _initialPoolSize)
                {
                    poolObject.Reset();
                    Add(poolObject);
                }
                else
                {
                    poolObject.Terminate();
                }
            }
        }

        /// <summary>
        /// Gets an object from the pool.
        /// </summary>
        /// <returns>
        /// Returns with the object or null if there isn't any free objects
        /// and the pool does not allow growth.
        /// </returns>
        [Pure]
        [CanBeNull]
        public T GetObject()
        {
            lock (_lock)
            {
                if (_pool.Count == 0)
                {
                    if (!_allowPoolGrowth)
                        return null;

                    T newObject = CreateObject();
                    _pool.Clear();
                    return newObject;
                }

                return _pool.Dequeue();
            }
        }
    }
}
