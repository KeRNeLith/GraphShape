using System;
using JetBrains.Annotations;

namespace GraphShape.Controls
{
    /// <summary>
    /// Animation context.
    /// </summary>
    public class AnimationContext : IAnimationContext
    {
        /// <inheritdoc />
        public GraphCanvas GraphCanvas { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationContext"/> class.
        /// </summary>
        /// <param name="canvas">Graph canvas.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="canvas"/> is <see langword="null"/>.</exception>
        public AnimationContext([NotNull] GraphCanvas canvas)
        {
            GraphCanvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        }
    }
}