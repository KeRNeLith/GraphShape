using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace GraphSharp.Helpers
{
	public delegate void DisposingHandler( object sender );

	public interface IPoolObject
	{
		void Reset();
		void Terminate();
		event DisposingHandler Disposing;
	}

	public class ObjectPool<T>
		where T : class, IPoolObject, new()
	{
		private const int POOL_SIZE = 1024;

		private readonly Queue<T> pool = new Queue<T>();

		private readonly bool allowPoolGrowth = true;
		private readonly int initialPoolSize;
		private int activePoolObjectCount = 0;

		/// <summary>
		/// Pool constructor, pool will allow growth.
		/// </summary>
		public ObjectPool()
			: this( POOL_SIZE, true )
		{

		}

		/// <summary>
		/// Pool constructor.
		/// </summary>
		/// <param name="initialPoolSize">Initial pool size.</param>
		/// <param name="allowPoolGrowth">Allow pool growth or not.</param>
		public ObjectPool( int initialPoolSize, bool allowPoolGrowth )
		{
			this.initialPoolSize = initialPoolSize;
			this.allowPoolGrowth = allowPoolGrowth;

			InitializePool();
		}

		/// <summary>
		/// Fills the pool with objects.
		/// </summary>
		private void InitializePool()
		{
			//adds some objects to the pool
			for ( int i = 0; i < initialPoolSize; i++ )
				CreateObject();
		}

		/// <summary>
		/// This method will create a new pool object if the pool is not full
		/// or allow growth and adds it to the poll.
		/// </summary>
		/// <returns>
		/// Returns with the newly created object or default(T) if the pool is full.
		/// </returns>
		private T CreateObject()
		{
			if ( activePoolObjectCount >= initialPoolSize && !allowPoolGrowth )
				return null;

			var newObject = new T();
			newObject.Disposing += Object_Disposing;

			Add( newObject );

			return newObject;
		}

		/// <summary>
		/// This method adds the object to the pool and increases the actual pool size.
		/// </summary>
		/// <param name="poolObject">The object which should be added to the pool.</param>
		private void Add( T poolObject )
		{
			pool.Enqueue( poolObject );
			activePoolObjectCount += 1;
		}

        /// <summary>
        /// It puts back the disposed poolObject into the pull.
        /// </summary>
        /// <param name="sender">The disposed pool object.</param>
		private void Object_Disposing( object sender )
		{
			lock ( this )
			{
				T poolObject = sender as T;
				activePoolObjectCount -= 1;
				if ( pool.Count < initialPoolSize )
				{
					poolObject.Reset();
					Add( poolObject );
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
		/// <returns>Returns with the object or null if there isn't any 
		/// free objects and the pool does not allow growth.</returns>
		public T GetObject()
		{
			lock ( this )
			{
				if ( pool.Count == 0 )
				{
					if ( !allowPoolGrowth )
						return null;

					T newObject = CreateObject();
					pool.Clear();
					return newObject;
				}
				
				return pool.Dequeue();
			}
		}
	}
}
