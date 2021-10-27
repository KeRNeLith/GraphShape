﻿using System;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape
{
    /// <summary>
    /// Edge implementation with a <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    [Serializable]
    public class TypedEdge<TVertex> : Edge<TVertex>, ITypedEdge<TVertex>
    {
        /// <inheritdoc />
        public EdgeTypes Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedEdge{TVertex}"/> class.
        /// </summary>
        /// <param name="source">Source vertex.</param>
        /// <param name="target">Target vertex.</param>
        /// <param name="type">Edge type.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        public TypedEdge([NotNull] TVertex source, [NotNull] TVertex target, EdgeTypes type)
            : base(source, target)
        {
            Type = type;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Type}: {Source}->{Target}";
        }
    }
}