﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;
using QuikGraph.Utils;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Base class for all <see cref="ILayoutAlgorithm{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    /// <typeparam name="TParameters">Parameters type.</typeparam>
    public abstract class ParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, TParameters>
        : LayoutAlgorithmBase<TVertex, TEdge, TGraph>
        , IParameterizedLayoutAlgorithm<TVertex, TEdge, TGraph, TParameters>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
        where TParameters : class, ILayoutParameters
    {
        /// <inheritdoc />
        public TParameters Parameters { get; protected set; }

        /// <inheritdoc />
        public ILayoutParameters GetParameters()
        {
            return Parameters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedLayoutAlgorithmBase{TVertex,TEdge,TGraph,TParameters}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        // ReSharper disable once NotNullMemberIsNotInitialized, Justification: Initialized in InitParameters
        protected ParameterizedLayoutAlgorithmBase([NotNull] TGraph visitedGraph)
            : this(visitedGraph, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedLayoutAlgorithmBase{TVertex,TEdge,TGraph,TParameters}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="parameters">Optional algorithm parameters.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        // ReSharper disable once NotNullMemberIsNotInitialized, Justification: Initialized in InitParameters
        protected ParameterizedLayoutAlgorithmBase(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [CanBeNull] TParameters parameters)
            : base(visitedGraph, verticesPositions)
        {
            InitParameters(parameters);
        }

        #region Initializers

        /// <summary>
        /// Default algorithm parameters to use if no parameters provided at construction.
        /// </summary>
        protected abstract TParameters DefaultParameters { get; }

        /// <summary>
        /// Initializes the parameters (cloning or creating new parameter object with default values).
        /// </summary>
        /// <param name="parameters">
        /// Parameters from a previous layout.
        /// If it is <see langword="null"/>, the <see cref="Parameters"/> will be set to the default ones.
        /// </param>
        protected void InitParameters([CanBeNull] TParameters parameters)
        {
            if (parameters is null)
            {
                Parameters = DefaultParameters;
            }
            else
            {
                Parameters = (TParameters)parameters.Clone();
            }
        }

        [CanBeNull]
        private Random _rand;

        /// <summary>
        /// Gets or sets the random number generator used to initialize positions randomly (and eventually during algorithm computation).
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">Value is <see langword="null"/>.</exception>
        [NotNull]
        public Random Rand
        {
            get => _rand ?? (_rand = new CryptoRandom((int)DateTime.Now.Ticks));
            set => _rand = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Initializes the positions of the vertices. Assigns a random position inside the 'bounding box' to the vertices without positions.
        /// It does NOT modify the position of the other vertices.
        /// Bounding box:
        /// x coordinates: <see cref="F:double.Epsilon"/> - <paramref name="width"/>
        /// y coordinates: <see cref="F:double.Epsilon"/> - <paramref name="height"/>
        /// </summary>
        /// <param name="width">Width of the bounding box.</param>
        /// <param name="height">Height of the bounding box.</param>
        protected virtual void InitializeWithRandomPositions(double width, double height)
        {
            InitializeWithRandomPositions(width, height, 0, 0);
        }

        /// <summary>
        /// Initializes the positions of the vertices. Assigns a random position inside the 'bounding box' to the vertices without positions.
        /// It does NOT modify the position of the other vertices.
        /// Bounding box:
        /// x coordinates: <see cref="F:double.Epsilon"/> - <paramref name="width"/>
        /// y coordinates: <see cref="F:double.Epsilon"/> - <paramref name="height"/>
        /// </summary>
        /// <param name="width">Width of the bounding box.</param>
        /// <param name="height">Height of the bounding box.</param>
        /// <param name="translateX">Translates the generated x coordinate.</param>
        /// <param name="translateY">Translates the generated y coordinate.</param>
        protected virtual void InitializeWithRandomPositions(double width, double height, double translateX, double translateY)
        {
            LayoutUtils.FillWithRandomPositions(
                width,
                height,
                translateX,
                translateY,
                VisitedGraph.Vertices,
                VerticesPositions,
                Rand);
        }

        /// <summary>
        /// Normalizes the vertices positions.
        /// </summary>
        protected virtual void NormalizePositions()
        {
            lock (SyncRoot)
            {
                LayoutUtils.NormalizePositions(VerticesPositions);
            }
        }

        #endregion

        /// <summary>
        /// Creates event arguments for <see cref="LayoutAlgorithmBase{TVertex,TEdge,TGraph}.IterationEnded"/>.
        /// </summary>
        /// <param name="iteration">Number of the current iteration.</param>
        /// <param name="statusInPercent">Status of the layout algorithm in percent.</param>
        /// <param name="message">Message representing the status of the algorithm.</param>
        /// <param name="verticesPositions">Vertices positions associations.</param>
        /// <returns>A new instance of <see cref="ILayoutIterationEventArgs{TVertex}"/>.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="iteration"/> is negative.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="statusInPercent"/> is negative.</exception>
        [Pure]
        [NotNull]
        protected virtual ILayoutIterationEventArgs<TVertex> CreateLayoutIterationEventArgs(
            int iteration,
            double statusInPercent,
            [NotNull] string message,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions)
        {
            return new LayoutIterationEventArgs<TVertex, TEdge>(iteration, statusInPercent, message, verticesPositions);
        }

        /// <summary>
        /// Raises an <see cref="LayoutAlgorithmBase{TVertex,TEdge,TGraph}.IterationEnded"/> event.
        /// </summary>
        /// <param name="iteration">Number of the current iteration.</param>
        /// <param name="statusInPercent">Status of the layout algorithm in percent.</param>
        /// <param name="message">Message representing the status of the algorithm.</param>
        /// <param name="normalizePositions">Indicates if given positions must be normalized.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="iteration"/> is negative.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="statusInPercent"/> is negative.</exception>
        protected void OnIterationEnded(
            int iteration,
            double statusInPercent,
            [NotNull] string message,
            bool normalizePositions)
        {
            // Copy the actual positions
            IDictionary<TVertex, Point> verticesPositions;
            lock (SyncRoot)
            {
                verticesPositions = new Dictionary<TVertex, Point>(VerticesPositions);
            }

            if (normalizePositions)
            {
                LayoutUtils.NormalizePositions(verticesPositions);
            }

            ILayoutIterationEventArgs<TVertex> args = CreateLayoutIterationEventArgs(
                iteration,
                statusInPercent,
                message,
                verticesPositions);
            OnIterationEnded(args);
        }
    }

    /// <summary>
    /// Base class for all <see cref="ILayoutAlgorithm{TVertex,TEdge,TGraph,TVertexInfo,TEdgeInfo}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    /// <typeparam name="TParameters">Parameters type.</typeparam>
    public abstract class DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, TParameters>
        : ParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, TParameters>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
        where TParameters : class, ILayoutParameters, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultParameterizedLayoutAlgorithmBase{TVertex,TEdge,TGraph,TParameters}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        protected DefaultParameterizedLayoutAlgorithmBase([NotNull] TGraph visitedGraph)
            : base(visitedGraph)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultParameterizedLayoutAlgorithmBase{TVertex,TEdge,TGraph,TParameters}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="parameters">Optional algorithm parameters.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        protected DefaultParameterizedLayoutAlgorithmBase(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [CanBeNull] TParameters parameters)
            : base(visitedGraph, verticesPositions, parameters)
        {
        }

        /// <inheritdoc />
        protected override TParameters DefaultParameters { get; } = new TParameters();
    }

    /// <summary>
    /// Base class for all <see cref="ILayoutAlgorithm{TVertex,TEdge,TGraph,TVertexInfo,TEdgeInfo}"/>.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    /// <typeparam name="TVertexInfo">Vertex information type.</typeparam>
    /// <typeparam name="TEdgeInfo">Edge information type.</typeparam>
    /// <typeparam name="TParameters">Parameters type.</typeparam>
    public abstract class ParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, TVertexInfo, TEdgeInfo, TParameters>
        : ParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, TParameters>
        , ILayoutAlgorithm<TVertex, TEdge, TGraph, TVertexInfo, TEdgeInfo>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
        where TParameters : class, ILayoutParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedLayoutAlgorithmBase{TVertex,TEdge,TGraph,TVertexInfo,TEdgeInfo,TParameters}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        protected ParameterizedLayoutAlgorithmBase([NotNull] TGraph visitedGraph)
            : base(visitedGraph, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedLayoutAlgorithmBase{TVertex,TEdge,TGraph,TVertexInfo,TEdgeInfo,TParameters}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="parameters">Optional algorithm parameters.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        protected ParameterizedLayoutAlgorithmBase(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [CanBeNull] TParameters parameters)
            : base(visitedGraph, verticesPositions, parameters)
        {
        }

        /// <inheritdoc />
        /// <returns>A new instance of <see cref="ILayoutInfoIterationEventArgs{TVertex,TEdge,TVertexInfo,TEdgeInfo}"/>.</returns>
        protected override ILayoutIterationEventArgs<TVertex> CreateLayoutIterationEventArgs(
            int iteration,
            double statusInPercent,
            string message,
            IDictionary<TVertex, Point> verticesPositions)
        {
            return new LayoutIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>(
                iteration,
                statusInPercent,
                message,
                verticesPositions,
                VerticesInfos,
                EdgesInfos);
        }

        #region ILayoutAlgorithm<TVertex,TEdge,TGraph,TVertexInfo,TEdgeInfo>

        /// <inheritdoc />
        public event LayoutIterationEndedEventHandler<TVertex, TEdge, TVertexInfo, TEdgeInfo> InfoIterationEnded;

        /// <inheritdoc />
        public IDictionary<TVertex, TVertexInfo> VerticesInfos { get; } = new Dictionary<TVertex, TVertexInfo>();

        /// <inheritdoc />
        public IDictionary<TEdge, TEdgeInfo> EdgesInfos { get; } = new Dictionary<TEdge, TEdgeInfo>();

        /// <inheritdoc />
        public override object GetVertexInfo(TVertex vertex)
        {
            if (VerticesInfos.TryGetValue(vertex, out TVertexInfo info))
                return info;
            return null;
        }

        /// <inheritdoc />
        public override object GetEdgeInfo(TEdge edge)
        {
            if (EdgesInfos.TryGetValue(edge, out TEdgeInfo info))
                return info;
            return null;
        }

        #endregion

        /// <inheritdoc />
        internal override void RaiseIterationEnded(ILayoutIterationEventArgs<TVertex> args)
        {
            base.RaiseIterationEnded(args);

            var castArgs = (ILayoutInfoIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>)args;
            InfoIterationEnded?.Invoke(this, castArgs);
        }
    }
}