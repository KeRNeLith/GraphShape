using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    public partial class CompoundFDPLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Initializes the algorithm, and the following things:
        /// 
        /// - The nodes sizes (of the compound vertices)
        /// - The thresholds for the convergence
        /// - Random initial positions (if the position is not null)
        /// - Remove the 'tree-nodes' from the root graph (level 0)
        /// </summary>
        /// <param name="verticesSizes">The dictionary of the inner canvas sizes of the compound vertices.</param>
        /// <param name="verticesBorders">The dictionary of the border thickness of the compound vertices.</param>
        /// <param name="layoutTypes">The dictionary of the layout types of the compound vertices.</param>
        private void Init(
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [NotNull] IDictionary<TVertex, Thickness> verticesBorders,
            [NotNull] IDictionary<TVertex, CompoundVertexInnerLayoutType> layoutTypes)
        {
            InitializeWithRandomPositions(100, 100);

            var movableParentUpdateQueue = new Queue<TVertex>();
            _rootCompoundVertex.Children = new HashSet<VertexData>();

            InitialLevels();

            InitSimpleVertices(verticesSizes);
            InitCompoundVertices(verticesBorders, verticesSizes, layoutTypes, movableParentUpdateQueue);

            SetLevelIndices();

            // TODO Is it needed?
            InitMovableParentOfFixedVertices(movableParentUpdateQueue);

            RemoveTreeNodesFromRootGraph();

            InitGravitationMagnitude();
        }

        private void InitGravitationMagnitude()
        {
            // Get the average width and height
            double sumWidth = 0, sumHeight = 0;
            foreach (TVertex vertex in Levels[0])
            {
                VertexData data = _verticesData[vertex];
                sumWidth += data.Size.Width;
                sumHeight += data.Size.Height;
            }

            if (Levels[0].Count > 0)
                _gravityForceMagnitude = Math.Min(sumWidth, sumHeight) / Levels[0].Count;
        }

        private void RemoveTreeNodesFromRootGraph()
        {
            bool removed = true;
            for (int i = 0; removed; ++i)
            {
                removed = false;
                foreach (TVertex vertex in Levels[0])
                {
                    if (_compoundGraph.Degree(vertex) != 1 || _compoundGraph.IsCompoundVertex(vertex))
                        continue;

                    TEdge edge = _compoundGraph.InDegree(vertex) > 0
                        ? _compoundGraph.InEdge(vertex, 0)
                        : _compoundGraph.OutEdge(vertex, 0);
                    if (_removedRootTreeEdges.Contains(edge))
                        continue;

                    // The vertex is a leaf tree node
                    removed = true;
                    while (_removedRootTreeNodeLevels.Count <= i)
                        _removedRootTreeNodeLevels.Push(new List<RemovedTreeNodeData>());

                    // Add to the removed vertices
                    _removedRootTreeNodeLevels.Peek().Add(new RemovedTreeNodeData(vertex, edge));
                    _removedRootTreeNodes.Add(vertex);
                    _removedRootTreeEdges.Add(edge);
                }

                if (removed && _removedRootTreeNodeLevels.Count > 0)
                {
                    // Remove from the level
                    foreach (RemovedTreeNodeData data in _removedRootTreeNodeLevels.Peek())
                    {
                        Levels[0].Remove(data.Vertex);

                        // Remove the vertex from the graph
                        _compoundGraph.RemoveEdge(data.Edge);
                        _compoundGraph.RemoveVertex(data.Vertex);
                    }
                }
            }
        }

        private void InitMovableParentOfFixedVertices([NotNull, ItemNotNull] Queue<TVertex> movableParentUpdateQueue)
        {
            while (movableParentUpdateQueue.Count > 0)
            {
                // Get the compound vertex with the fixed layout
                TVertex fixedLayoutCompoundVertex = movableParentUpdateQueue.Dequeue();

                // Find the not fixed predecessor
                TVertex movableVertex = fixedLayoutCompoundVertex;
                while (movableVertex != null)
                {
                    // If the vertex hasn't parent
                    TVertex parent = _compoundGraph.GetParent(movableVertex);
                    if (parent is null)
                        break;

                    // If the parent's layout type is fixed
                    if (_compoundVerticesData[parent].InnerVertexLayoutType != CompoundVertexInnerLayoutType.Fixed)
                        break;

                    movableVertex = parent;
                }
                // The movable vertex is the ancestor of the children of the vertex that could be moved

                // Fix the child vertices and set the movable parent
                foreach (TVertex childVertex in _compoundGraph.GetChildrenVertices(fixedLayoutCompoundVertex))
                {
                    VertexData data = _verticesData[childVertex];
                    data.IsFixedToParent = true;
                    data.MovableParent = movableVertex is null ? null : _verticesData[movableVertex];
                }
            }
        }

        /// <summary>
        /// Initializes the data of the simple vertices.
        /// </summary>
        /// <param name="verticesSizes">Dictionary of the vertex sizes.</param>
        private void InitSimpleVertices([NotNull] IDictionary<TVertex, Size> verticesSizes)
        {
            foreach (TVertex vertex in _compoundGraph.SimpleVertices)
            {
                verticesSizes.TryGetValue(vertex, out Size vertexSize);

                VerticesPositions.TryGetValue(vertex, out Point position);

                // Create the information container for this simple vertex
                var dataContainer = new SimpleVertexData(vertex, _rootCompoundVertex, false, position, vertexSize)
                {
                    Parent = _rootCompoundVertex
                };
                _verticesData[vertex] = dataContainer;
                // ReSharper disable once PossibleNullReferenceException, Justification: root always has children
                _rootCompoundVertex.Children.Add(dataContainer);
            }
        }

        /// <summary>
        /// Initializes the data of the compound vertices.
        /// </summary>
        /// <param name="verticesBorders">Dictionary of the vertices border thicknesses.</param>
        /// <param name="verticesSizes">Dictionary of the vertices sizes.</param>
        /// <param name="layoutTypes">Dictionary of the layout types.</param>
        /// <param name="movableParentUpdateQueue">
        /// The compound vertices with fixed layout should be added to this queue.
        /// </param>
        private void InitCompoundVertices(
            [NotNull] IDictionary<TVertex, Thickness> verticesBorders,
            [NotNull] IDictionary<TVertex, Size> verticesSizes,
            [NotNull] IDictionary<TVertex, CompoundVertexInnerLayoutType> layoutTypes,
            [NotNull, ItemNotNull] Queue<TVertex> movableParentUpdateQueue)
        {
            for (int i = Levels.Count - 1; i >= 0; --i)
            {
                foreach (TVertex vertex in Levels[i])
                {
                    if (!_compoundGraph.IsCompoundVertex(vertex))
                        continue;

                    // Get the data of the vertex
                    verticesBorders.TryGetValue(vertex, out Thickness border);

                    verticesSizes.TryGetValue(vertex, out Size vertexSize);
                    layoutTypes.TryGetValue(vertex, out CompoundVertexInnerLayoutType layoutType);

                    if (layoutType == CompoundVertexInnerLayoutType.Fixed)
                    {
                        movableParentUpdateQueue.Enqueue(vertex);
                    }

                    VerticesPositions.TryGetValue(vertex, out Point position);

                    // Create the information container for this compound vertex
                    var dataContainer = new CompoundVertexData(vertex, _rootCompoundVertex, false, position, vertexSize, border, layoutType);
                    if (i == 0)
                    {
                        dataContainer.Parent = _rootCompoundVertex;
                        // ReSharper disable once PossibleNullReferenceException, Justification: root always has children
                        _rootCompoundVertex.Children.Add(dataContainer);
                    }
                    _compoundVerticesData[vertex] = dataContainer;
                    _verticesData[vertex] = dataContainer;

                    // Add the data of the children
                    IEnumerable<TVertex> children = _compoundGraph.GetChildrenVertices(vertex);
                    List<VertexData> childrenData = children.Select(v => _verticesData[v]).ToList();
                    dataContainer.Children = childrenData;
                    foreach (VertexData child in dataContainer.Children)
                    {
                        // ReSharper disable once PossibleNullReferenceException, Justification: root always has children
                        _rootCompoundVertex.Children.Remove(child);
                        child.Parent = dataContainer;
                    }
                }
            }
        }

        private void InitialLevels()
        {
            var verticesLeft = new HashSet<TVertex>(VisitedGraph.Vertices);

            // Initial 0th level
            Levels.Add(new HashSet<TVertex>(VisitedGraph.Vertices.Where(v => _compoundGraph.GetParent(v) == default(TVertex))));
            verticesLeft.RemoveAll(Levels[0]);

            // Other layers
            for (int i = 1; verticesLeft.Count > 0; ++i)
            {
                var nextLevel = new HashSet<TVertex>();
                foreach (TVertex parent in Levels[i - 1])
                {
                    if (_compoundGraph.GetChildrenCount(parent) <= 0)
                        continue;

                    foreach (TVertex children in _compoundGraph.GetChildrenVertices(parent))
                        nextLevel.Add(children);
                }

                Levels.Add(nextLevel);
                verticesLeft.RemoveAll(nextLevel);
            }
        }

        private void SetLevelIndices()
        {
            // Set the level indexes in the vertex data
            for (int i = 0; i < Levels.Count; ++i)
            {
                foreach (TVertex vertex in Levels[i])
                    _verticesData[vertex].Level = i;
            }
        }
    }
}