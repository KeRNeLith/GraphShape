using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GraphShape.Utils;
using JetBrains.Annotations;
using QuikGraph;
using QuikGraph.Algorithms;

namespace GraphShape.Algorithms
{
    /// <summary>
    /// Algorithm that sorts vertices by layer.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public class LayeredTopologicalSortAlgorithm<TVertex, TEdge> : AlgorithmBase<IVertexAndEdgeListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        // The list of the vertices in the layers
        [NotNull, ItemNotNull]
        private readonly List<IList<TVertex>> _layers = new List<IList<TVertex>>();

        [NotNull]
        private readonly IMutableBidirectionalGraph<TVertex, TEdge> _tmpGraph;

        #region Properties

        /// <summary>
        /// This dictionary contains the layer-index for every vertices.
        /// </summary>
        [NotNull]
        public IDictionary<TVertex, int> LayerIndices { get; } = new Dictionary<TVertex, int>();

        /// <summary>
        /// The count of the layers in the graph.
        /// </summary>
        public int LayerCount { get; private set; }

        /// <summary>
        /// The vertices grouped by their LayerIndex.
        /// </summary>
        [NotNull, ItemNotNull]
        public IList<IList<TVertex>> Layers => _layers;

        #endregion

        /// <summary>
        /// Handler for <see cref="LayeredTopologicalSortAlgorithm{TVertex,TEdge}.LayerFinished"/> event.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        public delegate void LayerFinishedDelegate([NotNull] object sender, [NotNull] LayeredTopologicalSortEventArgs args);

        /// <summary>
        /// Fired each time a layer is treated and finished.
        /// </summary>
        public event LayerFinishedDelegate LayerFinished;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayeredTopologicalSortAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="visitedGraph"/> is <see langword="null"/>.</exception>
        public LayeredTopologicalSortAlgorithm(
            [NotNull] IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph)
            : base(visitedGraph)
        {
            // Create a copy from the graph
            _tmpGraph = visitedGraph.CopyToBidirectionalGraph();
        }

        private void OnLayerFinished([NotNull] LayeredTopologicalSortEventArgs args)
        {
            Debug.Assert(args != null);

            LayerFinished?.Invoke(this, args);
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            // Initializing the sources
            IList<TVertex> sources = GetSources(_tmpGraph.Vertices);

            // Initializing the candidates (candidate for 'source' of the next layer)
            var newSources = new HashSet<TVertex>();

            for (LayerCount = 0; sources.Count != 0; ++LayerCount)
            {
                foreach (TVertex source in sources)
                {
                    LayerIndices[source] = LayerCount;

                    // Get the neighbors of this source
                    IEnumerable<TVertex> outNeighbors = _tmpGraph.GetOutNeighbors(source);

                    // Remove this source
                    _tmpGraph.RemoveVertex(source);

                    // Check if any of the neighbors became a source
                    foreach (TVertex neighbor in outNeighbors.Where(neighbor => _tmpGraph.IsInEdgesEmpty(neighbor)))
                    {
                        newSources.Add(neighbor);
                    }
                }

                // The actual layer have been finished
                _layers.Add(sources);
                OnLayerFinished(new LayeredTopologicalSortEventArgs(LayerCount, sources));

                // Prepare for the next layer
                sources = newSources.ToList();
                newSources = new HashSet<TVertex>();
            }

            // If the graph is not empty, it's a problem
            if (!_tmpGraph.IsVerticesEmpty)
                throw new NonAcyclicGraphException();
        }

        #endregion

        [NotNull, ItemNotNull]
        private IList<TVertex> GetSources([NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
        {
            Debug.Assert(vertices != null);

            return vertices.Where(v => _tmpGraph.IsInEdgesEmpty(v)).ToList();
        }

        /// <summary>
        /// Event arguments for a layered topological sort algorithm.
        /// </summary>
        public class LayeredTopologicalSortEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LayeredTopologicalSortEventArgs"/> class.
            /// </summary>
            internal LayeredTopologicalSortEventArgs(
                int layerIndex,
                [NotNull, ItemNotNull] IEnumerable<TVertex> vertices)
            {
                Debug.Assert(vertices != null);

                LayerIndex = layerIndex;
                Vertices = vertices.ToArray();
            }

            /// <summary>
            /// Layer index.
            /// </summary>
            public int LayerIndex { get; }

            /// <summary>
            /// Layer vertices.
            /// </summary>
            [NotNull, ItemNotNull]
            public TVertex[] Vertices { get; }
        }
    }
}