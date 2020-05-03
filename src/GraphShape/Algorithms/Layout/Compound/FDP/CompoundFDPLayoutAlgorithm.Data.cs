﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Compound.FDP
{
    public partial class CompoundFDPLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Information for compound vertices.
        /// </summary>
        [NotNull]
        private readonly IDictionary<TVertex, CompoundVertexData> _compoundVerticesData =
            new Dictionary<TVertex, CompoundVertexData>();

        /// <summary>
        /// Information for all kind of vertices.
        /// </summary>
        [NotNull]
        private readonly IDictionary<TVertex, VertexData> _verticesData =
            new Dictionary<TVertex, VertexData>();

        /// <summary>
        /// The levels of the graph (generated by the containment associations).
        /// </summary>
        [NotNull, ItemNotNull]
        public IList<HashSet<TVertex>> Levels { get; } = new List<HashSet<TVertex>>();

        private class RemovedTreeNodeData
        {
            [NotNull]
            public readonly TVertex Vertex;

            [NotNull]
            public readonly TEdge Edge;

            public RemovedTreeNodeData([NotNull] TVertex vertex, [NotNull] TEdge edge)
            {
                Vertex = vertex;
                Edge = edge;
            }
        }

        /// <summary>
        /// The list of the removed root-tree-nodes and edges by it's level
        /// (level = distance from the closest not removed node).
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly Stack<IList<RemovedTreeNodeData>> _removedRootTreeNodeLevels =
            new Stack<IList<RemovedTreeNodeData>>();

        [NotNull, ItemNotNull]
        private readonly HashSet<TVertex> _removedRootTreeNodes = new HashSet<TVertex>();

        [NotNull, ItemNotNull]
        private readonly HashSet<TEdge> _removedRootTreeEdges = new HashSet<TEdge>();

        /// <summary>
        /// The dictionary of the initial vertices sizes.
        /// </summary>
        /// <remarks>Do not use it after initialization.</remarks>
        [NotNull]
        private readonly IDictionary<TVertex, Size> _verticesSizes;

        /// <summary>
        /// The dictionary of the vertices borders.
        /// </summary>
        /// <remarks>Do not use it after initialization.</remarks>
        [NotNull]
        private readonly IDictionary<TVertex, Thickness> _verticesBorders;

        /// <summary>
        /// The dictionary of the layout types of the compound vertices.
        /// </summary>
        /// <remarks>Do not use it after initialization.</remarks>
        [NotNull]
        private readonly IDictionary<TVertex, CompoundVertexInnerLayoutType> _layoutTypes;

        [NotNull]
        private readonly IMutableCompoundGraph<TVertex, TEdge> _compoundGraph;

        /// <summary>
        /// Represents the root vertex.
        /// </summary>
        [NotNull]
        private readonly CompoundVertexData _rootCompoundVertex =
            new CompoundVertexData(
                null, null, false, default(Point),
                default(Size), default(Thickness),
                CompoundVertexInnerLayoutType.Automatic);

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundFDPLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="verticesBorders">Vertices borders.</param>
        /// <param name="layoutTypes">Layout types per vertex.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public CompoundFDPLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [NotNull] IDictionary<TVertex, Thickness> verticesBorders,
            [NotNull] IDictionary<TVertex, CompoundVertexInnerLayoutType> layoutTypes,
            [CanBeNull] CompoundFDPLayoutParameters oldParameters = null)
            : this(visitedGraph, null, verticesSizes, verticesBorders, layoutTypes, oldParameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundFDPLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="verticesSizes">Vertices sizes.</param>
        /// <param name="verticesBorders">Vertices borders.</param>
        /// <param name="layoutTypes">Layout types per vertex.</param>
        /// <param name="oldParameters">Optional algorithm parameters.</param>
        public CompoundFDPLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [NotNull] IDictionary<TVertex, Thickness> verticesBorders,
            [NotNull] IDictionary<TVertex, CompoundVertexInnerLayoutType> layoutTypes,
            [CanBeNull] CompoundFDPLayoutParameters oldParameters = null)
            : base(visitedGraph, verticesPositions, oldParameters)
        {
            _verticesSizes = verticesSizes ?? throw new ArgumentNullException(nameof(verticesSizes));
            _verticesBorders = verticesBorders ?? throw new ArgumentNullException(nameof(verticesBorders));
            _layoutTypes = layoutTypes ?? throw new ArgumentNullException(nameof(layoutTypes));

            _compoundGraph = VisitedGraph is ICompoundGraph<TVertex, TEdge> compoundGraph
                ? new CompoundGraph<TVertex, TEdge>(compoundGraph)
                : new CompoundGraph<TVertex, TEdge>(VisitedGraph);
        }

        #endregion

        /// <summary>
        /// Gets the level of a given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Vertex to get its level.</param>
        /// <returns>Vertex level.</returns>
        [Pure]
        public int LevelOfVertex([NotNull] TVertex vertex)
        {
            if (_verticesData.TryGetValue(vertex, out VertexData data))
                return data.Level;
            throw new VertexNotFoundException("Vertex is not present in the treated graph.");
        }

        #region ICompoundLayoutAlgorithm<TVertex,TEdge,TGraph>

        /// <inheritdoc />
        public IDictionary<TVertex, Size> InnerCanvasSizes =>
            _compoundVerticesData.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.InnerCanvasSize);

        #endregion

        #region Nested type: VertexData

        /// <summary>
        /// Data for the simple vertices.
        /// </summary>
        protected abstract class VertexData
        {
            /// <summary>
            /// Gets the vertex which is wrapped by this object.
            /// Can be null for the root vertex.
            /// </summary>
            [CanBeNull]
            public TVertex Vertex { get; }

            /// <summary>
            /// Parent compound vertex.
            /// </summary>
            /// <remarks>
            /// Should be not null except for root compound vertex
            /// (<see cref="CompoundFDPLayoutAlgorithm{TVertex,TEdge,TGraph}._rootCompoundVertex"/>.
            /// </remarks>
            public CompoundVertexData Parent { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="VertexData"/> class.
            /// </summary>
            /// <param name="vertex">Target vertex.</param>
            /// <param name="movableParent">Moveable parent vertex data.</param>
            /// <param name="isFixedToParent">Indicates if the vertex position is fixed to its parent.</param>
            /// <param name="position">Vertex position.</param>
            protected VertexData(
                [CanBeNull] TVertex vertex,
                [CanBeNull] VertexData movableParent,
                bool isFixedToParent,
                Point position)
            {
                Vertex = vertex;
                MovableParent = movableParent;
                IsFixedToParent = isFixedToParent;
                Parent = null;
                Position = position;
            }

            /// <summary>
            /// If the vertex is fixed (cannot be moved), that's it's parent
            /// that could be moved (if there's any).
            /// 
            /// This property can only be set once.
            /// </summary>
            [CanBeNull]
            public VertexData MovableParent { get; set; }

            /// <summary>
            /// Gets or sets that the position of the vertex is fixed to
            /// it's parent vertex or not.
            /// </summary>
            public bool IsFixedToParent { get; set; }

            /// <summary>
            /// Gets the actual size of the vertex (inner size + border + anything else...).
            /// </summary>
            public abstract Size Size { get; }

            /// <summary>
            /// The level of the vertex inside the graph.
            /// </summary>
            public int Level { get; set; }

            /// <summary>
            /// The position of the vertex.
            /// </summary>
            public Point Position { get; set; }

            private Vector _springForce;

            /// <summary>
            /// Gets or sets the spring force.
            /// </summary>
            public Vector SpringForce
            {
                get => IsFixedToParent ? default(Vector) : _springForce;
                set => _springForce = IsFixedToParent ? default(Vector) : value;
            }

            private Vector _repulsionForce;

            /// <summary>
            /// Gets or sets the repulsion force.
            /// </summary>
            public Vector RepulsionForce
            {
                get => IsFixedToParent ? default(Vector) : _repulsionForce;
                set => _repulsionForce = IsFixedToParent ? default(Vector) : value;
            }

            private Vector _gravitationForce;

            /// <summary>
            /// Gets or sets the gravitation force.
            /// </summary>
            public Vector GravitationForce
            {
                get => IsFixedToParent ? default(Vector) : _gravitationForce;
                set => _gravitationForce = IsFixedToParent ? default(Vector) : value;
            }

            private Vector _applicationForce;

            /// <summary>
            /// Gets or sets the spring force.
            /// </summary>
            public Vector ApplicationForce
            {
                get => IsFixedToParent ? default(Vector) : _applicationForce;
                set => _applicationForce = IsFixedToParent ? default(Vector) : value;
            }

            private Vector _previousForce;
            private Vector _childrenForce;

            internal abstract void ApplyForce(Vector force);

            /// <summary>
            /// Applies forces on the wrapped vertex (bound to <paramref name="limit"/>).
            /// </summary>
            /// <returns>Computed force.</returns>
            public Vector ApplyForce(double limit)
            {
                Vector force = _springForce
                               + _repulsionForce
                               + _gravitationForce
                               + _applicationForce
                               + 0.5 * _childrenForce;

                Parent._childrenForce += force;

                if (force.Length > limit)
                    force *= limit / force.Length;
                force += 0.7 * _previousForce;
                if (force.Length > limit)
                    force *= limit / force.Length;

                ApplyForce(force);
                _springForce = default(Vector);
                _repulsionForce = default(Vector);
                _gravitationForce = default(Vector);
                _applicationForce = default(Vector);
                _childrenForce = default(Vector);

                _previousForce = force;
                return force;
            }
        }

        #endregion

        #region Nested type: SimpleVertexData

        private class SimpleVertexData : VertexData
        {
            public SimpleVertexData(
                [NotNull] TVertex vertex,
                VertexData movableParent,
                bool isFixedToParent,
                Point position,
                Size size)
                : base(vertex, movableParent, isFixedToParent, position)
            {
                Size = size;
            }

            /// <inheritdoc />
            public override Size Size { get; }

            /// <inheritdoc />
            internal override void ApplyForce(Vector force)
            {
                Position += force;
            }
        }

        #endregion

        #region Nested type: CompoundVertexData

        /// <summary>
        /// Data for the compound vertices.
        /// </summary>
        protected class CompoundVertexData : VertexData
        {
            /// <summary>
            /// The thickness of the borders of the compound vertex.
            /// </summary>
            public readonly Thickness Borders;

            /// <summary>
            /// Gets the layout type of the compound vertex.
            /// </summary>
            public readonly CompoundVertexInnerLayoutType InnerVertexLayoutType;

            /// <summary>
            /// Initializes a new instance of the <see cref="CompoundVertexData"/> class.
            /// </summary>
            /// <param name="vertex">Target vertex.</param>
            /// <param name="movableParent">Moveable parent vertex data.</param>
            /// <param name="isFixedToParent">Indicates if the vertex position is fixed to its parent.</param>
            /// <param name="position">Vertex position.</param>
            /// <param name="size">Vertex size.</param>
            /// <param name="borders">Vertex borders.</param>
            /// <param name="innerVertexLayoutType">Vertex layout type.</param>
            public CompoundVertexData(
                [CanBeNull] TVertex vertex,
                [CanBeNull] VertexData movableParent,
                bool isFixedToParent,
                Point position,
                Size size,
                Thickness borders,
                CompoundVertexInnerLayoutType innerVertexLayoutType)
                : base(vertex, movableParent, isFixedToParent, position)
            {
                Borders = borders;

                // Calculate the size of the inner canvas
                InnerCanvasSize = new Size(
                    Math.Max(0.0, size.Width - Borders.Left - Borders.Right),
                    Math.Max(0.0, size.Height - Borders.Top - Borders.Bottom));
                InnerVertexLayoutType = innerVertexLayoutType;
            }

            private Size _size;

            /// <inheritdoc />
            public override Size Size => _size;

            private Size _innerCanvasSize;

            /// <summary>
            /// The size of the inner canvas of the compound vertex.
            /// </summary>
            public Size InnerCanvasSize
            {
                get => _innerCanvasSize;
                set
                {
                    _innerCanvasSize = value;

                    // Set the size of the canvas
                    _size = new Size(
                        _innerCanvasSize.Width + Borders.Left + Borders.Right,
                        _innerCanvasSize.Height + Borders.Top + Borders.Bottom);
                }
            }

            /// <inheritdoc />
            internal override void ApplyForce(Vector force)
            {
                Position += force;
                PropagateToChildren(force);
                RecalculateBounds();
            }

            /// <summary>
            /// Modifies the position of the children with the given <paramref name="force"/>.
            /// </summary>
            /// <param name="force">The vector of the position modification.</param>
            private void PropagateToChildren(Vector force)
            {
                if (Children is null)
                    return;

                foreach (VertexData child in Children)
                {
                    child.ApplyForce(force);
                }
            }

            /// <summary>
            /// Children vertices data.
            /// </summary>
            [CanBeNull, ItemNotNull]
            public ICollection<VertexData> Children { get; set; }

            /// <summary>
            /// Gets or sets the inner canvas center position.
            /// </summary>
            public Point InnerCanvasCenter
            {
                get => new Point(
                    Position.X - Size.Width / 2 + Borders.Left + InnerCanvasSize.Width / 2,
                    Position.Y - Size.Height / 2 + Borders.Top + InnerCanvasSize.Height / 2);
                set => Position = new Point(
                    value.X - InnerCanvasSize.Width / 2 - Borders.Left + Size.Width / 2,
                    value.Y - InnerCanvasSize.Height / 2 - Borders.Top + Size.Height / 2);
            }

            /// <summary>
            /// Recompute vertex bounds.
            /// </summary>
            public void RecalculateBounds()
            {
                if (Children is null)
                {
                    InnerCanvasSize = new Size(); // Consider adding padding?
                    return;
                }

                Point topLeft = new Point(double.PositiveInfinity, double.PositiveInfinity);
                Point bottomRight = new Point(double.NegativeInfinity, double.NegativeInfinity);
                foreach (VertexData child in Children)
                {
                    topLeft.X = Math.Min(topLeft.X, child.Position.X - child.Size.Width / 2);
                    topLeft.Y = Math.Min(topLeft.Y, child.Position.Y - child.Size.Height / 2);

                    bottomRight.X = Math.Max(bottomRight.X, child.Position.X + child.Size.Width / 2);
                    bottomRight.Y = Math.Max(bottomRight.Y, child.Position.Y + child.Size.Height / 2);
                }

                InnerCanvasSize = new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
                InnerCanvasCenter = new Point((topLeft.X + bottomRight.X) / 2.0, (topLeft.Y + bottomRight.Y) / 2.0);
            }
        }

        #endregion
    }
}
