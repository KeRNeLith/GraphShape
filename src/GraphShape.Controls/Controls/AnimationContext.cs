using System;
using System.Diagnostics.Contracts;

namespace GraphShape.Controls
{
    public class AnimationContext : IAnimationContext
    {
        public GraphCanvas GraphCanvas { get; private set; }

        public AnimationContext( GraphCanvas canvas )
        {
            GraphCanvas = canvas;
        }
    }
}
