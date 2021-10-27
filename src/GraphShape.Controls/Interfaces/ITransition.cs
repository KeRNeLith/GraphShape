using System.Windows.Controls;
using System;
using JetBrains.Annotations;

namespace GraphShape.Controls
{
    /// <summary>
    /// Represents an entity able to perform animation transitions
    /// </summary>
    public interface ITransition
    {
        /// <summary>
        /// Runs the transition.
        /// </summary>
        /// <param name="context">The context of the transition.</param>
        /// <param name="control">The control which the transition should be run on.</param>
        /// <param name="duration">The duration of the transition.</param>
        /// <param name="endAction">The method that should be called when the transition finished.</param>
        void Run(
            [NotNull] IAnimationContext context,
            [NotNull] Control control,
            TimeSpan duration,
            [CanBeNull, InstantHandle] Action<Control> endAction);
    }
}