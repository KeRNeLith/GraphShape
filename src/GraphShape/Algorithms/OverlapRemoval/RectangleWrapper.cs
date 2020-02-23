using System;
using System.Windows;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// <see cref="Rect"/>angle wrapper.
    /// </summary>
    public class RectangleWrapper<TObject>
    {
        /// <summary>
        /// Rectangle Id.
        /// </summary>
        [NotNull]
        public TObject Id { get; }

        /// <summary>
        /// Wrapped rectangle.
        /// </summary>
        public Rect Rectangle;

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleWrapper{TObject}"/> class.
        /// </summary>
        /// <param name="rectangle">Rectangle to wrap.</param>
        /// <param name="id">
        /// Rectangle id (at the end of the overlap removal, we must know which rectangle
        /// is associated to which object. The id will be resolved at that time.
        /// </param>
        public RectangleWrapper([NotNull] TObject id, Rect rectangle)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            Id = id;
            Rectangle = rectangle;
        }

        /// <summary>
        /// Rectangle center.
        /// </summary>
        public Point Center => Rectangle.GetCenter();

        /// <summary>
        /// Rectangle center on X axis.
        /// </summary>
        public double CenterX => Rectangle.Left + Rectangle.Width / 2;

        /// <summary>
        /// Rectangle center on Y axis.
        /// </summary>
        public double CenterY => Rectangle.Top + Rectangle.Height / 2;
    }
}