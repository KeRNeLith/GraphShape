using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GraphShape.Controls
{
    /// <summary>
    /// Fade transition.
    /// </summary>
    public class FadeTransition : ITransition
    {
        private readonly double _startOpacity;
        private readonly double _endOpacity;
        private readonly int _rounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="FadeTransition"/> class.
        /// </summary>
        /// <param name="startOpacity">Starting opacity value.</param>
        /// <param name="endOpacity">Ending opacity value.</param>
        public FadeTransition(double startOpacity, double endOpacity)
            : this(startOpacity, endOpacity, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FadeTransition"/> class.
        /// </summary>
        /// <param name="startOpacity">Starting opacity value.</param>
        /// <param name="endOpacity">Ending opacity value.</param>
        /// <param name="rounds">Number of transition rounds.</param>
        public FadeTransition(double startOpacity, double endOpacity, int rounds)
        {
            _startOpacity = startOpacity;
            _endOpacity = endOpacity;
            _rounds = rounds;
        }

        /// <inheritdoc />
        public void Run(
            IAnimationContext context,
            Control control,
            TimeSpan duration,
            Action<Control> endAction)
        {
            var storyboard = new Storyboard();

            DoubleAnimation fadeAnimation;

            if (_rounds > 1)
            {
                fadeAnimation = new DoubleAnimation(_startOpacity, _endOpacity, new Duration(duration))
                {
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(_rounds - 1)
                };

                storyboard.Children.Add(fadeAnimation);
                Storyboard.SetTarget(fadeAnimation, control);
                Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(UIElement.OpacityProperty));
            }

            fadeAnimation = new DoubleAnimation(_startOpacity, _endOpacity, new Duration(duration))
            {
                BeginTime = TimeSpan.FromMilliseconds(duration.TotalMilliseconds * (_rounds - 1) * 2)
            };

            storyboard.Children.Add(fadeAnimation);
            Storyboard.SetTarget(fadeAnimation, control);
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath(UIElement.OpacityProperty));

            if (endAction != null)
            {
                storyboard.Completed += (s, a) => endAction(control);
            }
            storyboard.Begin(control);
        }
    }
}