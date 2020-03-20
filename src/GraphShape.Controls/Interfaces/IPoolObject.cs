using JetBrains.Annotations;

namespace GraphShape.Controls
{
    /// <summary>
    /// Handler for an object <see cref="IPoolObject.Disposing"/> event.
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DisposingHandler([NotNull] object sender);

    /// <summary>
    /// Represents an entity that can be part of an object pool.
    /// </summary>
    public interface IPoolObject
    {
        /// <summary>
        /// Resets object state.
        /// </summary>
        void Reset();
     
        /// <summary>
        /// Frees object resources.
        /// </summary>
        void Terminate();

        /// <summary>
        /// Fired when the object is disposing its resources.
        /// </summary>
        event DisposingHandler Disposing;
    }
}