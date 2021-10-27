﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Base class for all overlap removal context.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    public class OverlapRemovalContext<TVertex> : IOverlapRemovalContext<TVertex>
    {
        /// <inheritdoc />
        public IDictionary<TVertex, Rect> Rectangles { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlapRemovalContext{TVertex}"/> class.
        /// </summary>
        /// <param name="rectangles">Overlap rectangles.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="rectangles"/> is <see langword="null"/>.</exception>
        public OverlapRemovalContext([NotNull] IDictionary<TVertex, Rect> rectangles)
        {
            Rectangles = rectangles ?? throw new ArgumentNullException(nameof(rectangles));
        }
    }
}