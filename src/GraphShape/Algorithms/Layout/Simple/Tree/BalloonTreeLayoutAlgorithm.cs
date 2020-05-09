using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    /// <summary>
    /// Balloon tree layout algorithm.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
	public class BalloonTreeLayoutAlgorithm<TVertex, TEdge, TGraph> : DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, BalloonTreeLayoutParameters>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        [NotNull]
        private readonly TVertex _root;

        [NotNull]
        private readonly IDictionary<TVertex, BalloonData> _data = new Dictionary<TVertex, BalloonData>();

        [NotNull, ItemNotNull]
        private readonly HashSet<TVertex> _visitedVertices = new HashSet<TVertex>();

        private class BalloonData
        {
            public int D;
            public int R;
            public float A;
            public float C;
            public float F;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BalloonTreeLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="selectedVertex">Root vertex.</param>
        /// <param name="parameters">Optional algorithm parameters.</param>
        public BalloonTreeLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [NotNull] TVertex selectedVertex,
            [CanBeNull] BalloonTreeLayoutParameters parameters = null)
            : this(visitedGraph, null, selectedVertex, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BalloonTreeLayoutAlgorithm{TVertex,TEdge,TGraph}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to layout.</param>
        /// <param name="verticesPositions">Vertices positions.</param>
        /// <param name="selectedVertex">Root vertex.</param>
        /// <param name="parameters">Optional algorithm parameters.</param>
        public BalloonTreeLayoutAlgorithm(
            [NotNull] TGraph visitedGraph,
            [CanBeNull] IDictionary<TVertex, Point> verticesPositions,
            [NotNull] TVertex selectedVertex,
            [CanBeNull] BalloonTreeLayoutParameters parameters = null)
            : base(visitedGraph, verticesPositions, parameters)
        {
            if (selectedVertex == null)
                throw new ArgumentNullException(nameof(selectedVertex));
            if (!visitedGraph.ContainsVertex(selectedVertex))
                throw new ArgumentException("The provided vertex is not part of the graph.", nameof(selectedVertex));

            _root = selectedVertex;
        }

        #region AlgorithmBase

        /// <inheritdoc />
        protected override void Initialize()
        {
            InitializeData();
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            FirstWalk(_root);

            _visitedVertices.Clear();

            SecondWalk(_root, 0, 0, 1, 0);

            NormalizePositions();
        }

        #endregion

        private void InitializeData()
        {
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                _data[vertex] = new BalloonData();
            }

            _visitedVertices.Clear();
        }

        private void FirstWalk([NotNull] TVertex vertex)
        {
            Debug.Assert(vertex != null);

            BalloonData data = _data[vertex];
            _visitedVertices.Add(vertex);
            data.D = 0;

            float s = 0;

            foreach (TEdge edge in VisitedGraph.OutEdges(vertex))
            {
                TVertex otherVertex = edge.Target;
                BalloonData otherData = _data[otherVertex];

                if (!_visitedVertices.Contains(otherVertex))
                {
                    FirstWalk(otherVertex);
                    data.D = Math.Max(data.D, otherData.R);
                    otherData.A = (float)Math.Atan((float)otherData.R / (data.D + otherData.R));
                    s += otherData.A;
                }
            }

            AdjustChildren(data, s);
            SetRadius(data);
        }

        private void SecondWalk([NotNull] TVertex vertex, double x, double y, float l, float t)
        {
            Debug.Assert(vertex != null);

            var position = new Point(x, y);
            VerticesPositions[vertex] = position;
            _visitedVertices.Add(vertex);
            BalloonData data = _data[vertex];

            float dd = l * data.D;
            float p = (float)(t + Math.PI);
            int degree = VisitedGraph.OutDegree(vertex);
            float fs = degree == 0 ? 0 : data.F / degree;
            float pr = 0;

            foreach (TEdge edge in VisitedGraph.OutEdges(vertex))
            {
                TVertex otherVertex = edge.Target;
                if (_visitedVertices.Contains(otherVertex))
                    continue;

                BalloonData otherData = _data[otherVertex];
                float aa = data.C * otherData.A;
                float rr = (float)(data.D * Math.Tan(aa) / (1 - Math.Tan(aa)));
                p += pr + aa + fs;

                float xx = (float)((l * rr + dd) * Math.Cos(p));
                float yy = (l * rr + dd) * Math.Sign(p);
                pr = aa;
                SecondWalk(otherVertex, x + xx, y + yy, l * data.C, p);
            }
        }

        private void SetRadius([NotNull] BalloonData data)
        {
            Debug.Assert(data != null);

            data.R = Math.Max(data.D / 2, Parameters.MinRadius);
        }

        private static void AdjustChildren([NotNull] BalloonData data, float s)
        {
            Debug.Assert(data != null);

            if (s > Math.PI)
            {
                data.C = (float)Math.PI / s;
                data.F = 0;
            }
            else
            {
                data.C = 1;
                data.F = (float)Math.PI - s;
            }
        }
    }
}
