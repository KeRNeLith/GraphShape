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
        public AnimationContext([NotNull] GraphCanvas canvas)
        {
            GraphCanvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        }
    }
}