using System;
using System.Collections.Generic;
using System.Windows;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Base class for all overlap removal algorithm.
    /// </summary>
    /// <typeparam name="TObject">Object type.</typeparam>
    /// <typeparam name="TParameters">Algorithm parameters type.</typeparam>
    public abstract class OverlapRemovalAlgorithmBase<TObject, TParameters>
        : AlgorithmBase
        , IOverlapRemovalAlgorithm<TObject, TParameters>
        where TParameters : IOverlapRemovalParameters
    {
        /// <summary>
        /// Wrapped rectangles.
        /// </summary>
        [NotNull, ItemNotNull]
        protected List<RectangleWrapper<TObject>> WrappedRectangles;

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlapRemovalAlgorithmBase{TObject,TParameters}"/> class.
        /// </summary>
        /// <param name="rectangles">Overlap rectangles.</param>
        /// <param name="parameters">Algorithm parameters.</param>
        protected OverlapRemovalAlgorithmBase(
            [NotNull] IDictionary<TObject, Rect> rectangles,
            [NotNull] TParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            // Original rectangles
            Rectangles = rectangles ?? throw new ArgumentNullException(nameof(rectangles));

            // Wrapping the old rectangles, to remember which one belongs to which object
            WrappedRectangles = new List<RectangleWrapper<TObject>>();
            int i = 0;
            foreach (KeyValuePair<TObject, Rect> rectanglePair in rectangles)
            {
                WrappedRectangles.Insert(
                    i,
                    new RectangleWrapper<TObject>(rectanglePair.Key, rectanglePair.Value));
                ++i;
            }

            Parameters = parameters;
        }

        #region IOverlapRemovalAlgorithm

        /// <inheritdoc />
        public IDictionary<TObject, Rect> Rectangles { get; }

        /// <inheritdoc />
        public TParameters Parameters { get; }

        /// <inheritdoc />
        public IOverlapRemovalParameters GetParameters()
        {
            return Parameters;
        }

        #endregion

        #region AlgorithmBase

        /// <inheritdoc />
        protected sealed override void InternalCompute()
        {
            if (WrappedRectangles.Count == 0)
                return;

            AddGaps();

            RemoveOverlap();

            RemoveGaps();

            foreach (RectangleWrapper<TObject> rectangle in WrappedRectangles)
                Rectangles[rectangle.Id] = rectangle.Rectangle;
        }

        #endregion

        /// <summary>
        /// Adds gaps between rectangles.
        /// </summary>
        protected virtual void AddGaps()
        {
            foreach (RectangleWrapper<TObject> wrapper in WrappedRectangles)
            {
                wrapper.Rectangle.Width += Parameters.HorizontalGap;
                wrapper.Rectangle.Height += Parameters.VerticalGap;
                wrapper.Rectangle.Offset(-Parameters.HorizontalGap / 2, -Parameters.VerticalGap / 2);
            }
        }

        /// <summary>
        /// Removes overlap.
        /// </summary>
        protected abstract void RemoveOverlap();

        /// <summary>
        /// Removes gaps between rectangles.
        /// </summary>
        protected virtual void RemoveGaps()
        {
            foreach (RectangleWrapper<TObject> wrapper in WrappedRectangles)
            {
                wrapper.Rectangle.Width -= Parameters.HorizontalGap;
                wrapper.Rectangle.Height -= Parameters.VerticalGap;
                wrapper.Rectangle.Offset(Parameters.HorizontalGap / 2, Parameters.VerticalGap / 2);
            }
        }
    }
}