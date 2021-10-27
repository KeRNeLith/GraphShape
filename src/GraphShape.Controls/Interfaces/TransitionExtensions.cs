using System;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace GraphShape.Controls.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ITransition"/>.
    /// </summary>
    public static class TransitionExtensions
    {
        /// <summary>
        /// Runs the transition.
        /// </summary>
        /// <param name="transition">Transition handler.</param>
        /// <param name="context">The context of the transition.</param>
        /// <param name="control">The control which the transition should be run on.</param>
        /// <param name="duration">The duration of the transition.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="transition"/> is <see langword="null"/>.</exception>
        public static void Run(
            [NotNull] this ITransition transition,
            [NotNull] IAnimationContext context,
            [NotNull] Control control,
            TimeSpan duration)
        {
            if (transition is null)
                throw new ArgumentNullException(nameof(transition));
            transition.Run(context, control, duration, null);
        }
    }
}