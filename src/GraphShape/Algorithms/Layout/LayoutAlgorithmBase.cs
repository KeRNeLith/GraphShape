using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Base class for all <see cref="ILayoutAlgorithm{TVertex, TEdge, TGraph}"/> implementations.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public abstract class LayoutAlgorithmBase<TVertex, TEdge, TGraph> : AlgorithmBase, ILayoutAlgorithm<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <inheritdoc />
        public TGraph VisitedGraph { get; }

        /// <inheritdoc />
        public IDictionary<TVertex, Point> VerticesPositions { get; }

        /// <inheritdoc />
        public event ProgressChangedEventHandler ProgressChanged;

        /// <inheritdoc />
        public event LayoutIterationEndedEventHandler<TVertex> IterationEnded;

        /// <summary>
        /// Indicates if there is any watcher on algorithm progress.
        /// </summary>
        public bool ReportOnProgressChangedNeeded => ProgressChanged != null;

        /// <summary>
        /// Indicates if there is any watcher on iterations ends.
        /// </summary>
        public virtual bool ReportOnIterationEndNeeded => IterationEnded != null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutAlgorithmBase{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        protected LayoutAlgorithmBase([NotNull] TGraph visitedGraph)
            : this(visitedGraph, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutAlgorithmBase{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        protected LayoutAlgorithmBase([NotNull] TGraph visitedGraph, [CanBeNull] IDictionary<TVertex, Point> verticesPositions)
        {
            if (visitedGraph == null)
                throw new ArgumentNullException(nameof(visitedGraph));

            VisitedGraph = visitedGraph;
            VerticesPositions = verticesPositions is null
                ? new Dictionary<TVertex, Point>(visitedGraph.VertexCount)
                : new Dictionary<TVertex, Point>(verticesPositions);
        }

        /// <inheritdoc />
        public virtual object GetVertexInfo(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));
            return null;
        }

        /// <inheritdoc />
        public virtual object GetEdgeInfo(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));
            return null;
        }

        /// <summary>
        /// Called when some progress was made in layout algorithm.
        /// </summary>
        /// <param name="percent">New progress percentage.</param>
        protected virtual void OnProgressChanged(double percent)
        {
            ProgressChanged?.Invoke(this, percent);
        }

        internal virtual void RaiseIterationEnded([NotNull] ILayoutIterationEventArgs<TVertex> args)
        {
            IterationEnded?.Invoke(this, args);
        }

        /// <summary>
        /// Called when a layout iteration has ended.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnIterationEnded([NotNull] ILayoutIterationEventArgs<TVertex> args)
        {
            Debug.Assert(args != null);

            RaiseIterationEnded(args);

            // If the layout should be aborted
            if (args.Abort)
                Abort();
        }
    }

    /// <summary>
    /// Base class for all <see cref="ILayoutAlgorithm{TVertex, TEdge, TGraph, TVertexInfo, TEdgeInfo}"/> implementations.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    /// <typeparam name="TVertexInfo">Vertex information type.</typeparam>
    /// <typeparam name="TEdgeInfo">Edge information type.</typeparam>
    public abstract class LayoutAlgorithmBase<TVertex, TEdge, TGraph, TVertexInfo, TEdgeInfo>
        : LayoutAlgorithmBase<TVertex, TEdge, TGraph>
        , ILayoutAlgorithm<TVertex, TEdge, TGraph, TVertexInfo, TEdgeInfo>
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <inheritdoc />
        public event LayoutIterationEndedEventHandler<TVertex, TEdge, TVertexInfo, TEdgeInfo> InfoIterationEnded;

        /// <summary>
        /// Indicates if there is any watcher on iterations ends.
        /// </summary>
        public override bool ReportOnIterationEndNeeded => base.ReportOnIterationEndNeeded || InfoIterationEnded != null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutAlgorithmBase{TVertex,TEdge,TGraph,TVertexInfo,TEdgeInfo}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        protected LayoutAlgorithmBase([NotNull] TGraph visitedGraph)
            : base(visitedGraph)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutAlgorithmBase{TVertex,TEdge,TGraph,TVertexInfo,TEdgeInfo}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        protected LayoutAlgorithmBase([NotNull] TGraph visitedGraph, [CanBeNull] IDictionary<TVertex, Point> verticesPositions)
            : base(visitedGraph, verticesPositions)
        {
        }

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

        /// <inheritdoc />
        internal override void RaiseIterationEnded(ILayoutIterationEventArgs<TVertex> args)
        {
            base.RaiseIterationEnded(args);

            var castArgs = (ILayoutInfoIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>)args;
            InfoIterationEnded?.Invoke(this, castArgs);
        }
    }
}