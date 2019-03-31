using System;

namespace GraphSharp.Controls
{
    public abstract class TransitionBase : ITransition
    {
        #region ITransition Members

        public void Run( IAnimationContext context, System.Windows.Controls.Control control, TimeSpan duration )
        {
            Run( context, control, duration, null );
        }

        public abstract void Run( IAnimationContext context,
            System.Windows.Controls.Control control,
            TimeSpan duration,
            Action<System.Windows.Controls.Control> endMethod );

        #endregion
    }
}
