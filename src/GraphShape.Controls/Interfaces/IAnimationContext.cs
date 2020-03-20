using JetBrains.Annotations;

namespace GraphShape.Controls
{
    /// <summary>
    /// Represents an animation context.
    /// </summary>
    public interface IAnimationContext
    {
        /// <summary>
        /// Graph canvas.
        /// </summary>
        [NotNull]
        GraphCanvas GraphCanvas { get; }
    }
}