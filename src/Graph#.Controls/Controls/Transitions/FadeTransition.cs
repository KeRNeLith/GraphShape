using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Diagnostics.Contracts;

namespace GraphSharp.Controls
{
    public class FadeTransition : TransitionBase
    {
        private readonly double startOpacity;
        private readonly double endOpacity;
        private readonly int rounds = 1;

        public FadeTransition( double startOpacity, double endOpacity )
            : this( startOpacity, endOpacity, 2 )
        {
        }

        public FadeTransition( double startOpacity, double endOpacity, int rounds )
        {
            this.startOpacity = startOpacity;
            this.endOpacity = endOpacity;
            this.rounds = rounds;
        }

        public override void Run(
            IAnimationContext context,
            Control control,
            TimeSpan duration,
            Action<Control> endMethod )
        {
            var storyboard = new Storyboard();

            DoubleAnimation fadeAnimation;

            if ( rounds > 1 )
            {
                fadeAnimation = new DoubleAnimation( startOpacity, endOpacity, new Duration( duration ) );
                fadeAnimation.AutoReverse = true;
                fadeAnimation.RepeatBehavior = new RepeatBehavior( rounds - 1 );
                storyboard.Children.Add( fadeAnimation );
                Storyboard.SetTarget( fadeAnimation, control );
                Storyboard.SetTargetProperty( fadeAnimation, new PropertyPath( UIElement.OpacityProperty ) );
            }

            fadeAnimation = new DoubleAnimation( startOpacity, endOpacity, new Duration( duration ) );
            fadeAnimation.BeginTime = TimeSpan.FromMilliseconds( duration.TotalMilliseconds * ( rounds - 1 ) * 2 );
            storyboard.Children.Add( fadeAnimation );
            Storyboard.SetTarget( fadeAnimation, control );
            Storyboard.SetTargetProperty( fadeAnimation, new PropertyPath( UIElement.OpacityProperty ) );

            if ( endMethod != null )
                storyboard.Completed += ( s, a ) => endMethod( control );
            storyboard.Begin( control );
        }
    }
}
