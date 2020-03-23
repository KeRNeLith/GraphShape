using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using GraphShape.Utils;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Controls
{
    public partial class GraphLayout<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Removes the given <paramref name="vertex"/> from graph.
        /// </summary>
        /// <param name="vertex">Vertex to remove.</param>
        protected virtual void RemoveVertexControl([NotNull] TVertex vertex)
        {
            RunDestructionTransition(VerticesControls[vertex], false);
            VerticesControls.Remove(vertex);
        }

        /// <summary>
        /// Removes the given <paramref name="edge"/> from graph.
        /// </summary>
        /// <param name="edge">Edge to remove.</param>
        protected virtual void RemoveEdgeControl([NotNull] TEdge edge)
        {
            RunDestructionTransition(EdgesControls[edge], false);
            EdgesControls.Remove(edge);
        }

        /// <summary>
        /// Removes all vertices and edges from graph.
        /// </summary>
        protected void RemoveAllGraphElement()
        {
            foreach (TVertex vertex in VerticesControls.Keys.ToArray())
                RemoveVertexControl(vertex);

            foreach (TEdge edge in EdgesControls.Keys.ToArray())
                RemoveEdgeControl(edge);

            VerticesControls.Clear();
            EdgesControls.Clear();
        }

        /// <summary>
        /// If the graph has been changed, the elements will be regenerated.
        /// </summary>
        protected void RecreateGraphElements(bool tryKeepControls)
        {
            if (Graph is null)
            {
                RemoveAllGraphElement();
            }
            else
            {
                if (tryKeepControls && !IsCompoundMode)
                {
                    // Remove the old graph elements
                    foreach (KeyValuePair<TEdge, EdgeControl> pair in EdgesControls.ToArray())
                    {
                        bool remove = false;
                        try
                        {
                            remove = !Graph.ContainsEdge(pair.Key.Source, pair.Key.Target)
                                     || !Graph.ContainsEdge(pair.Key);
                        }
                        catch
                        {
                            // ignored
                        }

                        if (remove)
                        {
                            RemoveEdgeControl(pair.Key);
                        }
                    }

                    foreach (KeyValuePair<TVertex, VertexControl> pair in VerticesControls.ToArray())
                    {
                        if (!Graph.ContainsVertex(pair.Key))
                        {
                            RemoveVertexControl(pair.Key);
                        }
                    }
                }
                else
                {
                    RemoveAllGraphElement();
                }

                // Vertices controls
                foreach (TVertex vertex in Graph.Vertices)
                {
                    if (!VerticesControls.ContainsKey(vertex))
                        CreateVertexControl(vertex);
                }

                // Edges controls
                foreach (TEdge edge in Graph.Edges)
                {
                    if (!EdgesControls.ContainsKey(edge))
                        CreateEdgeControl(edge);
                }

                // Subscribe to events of the Graph mutations
                if (!IsCompoundMode && Graph is IMutableBidirectionalGraph<TVertex, TEdge> mutableGraph)
                {
                    mutableGraph.VertexAdded += OnMutableGraphVertexAdded;
                    mutableGraph.VertexRemoved += OnMutableGraphVertexRemoved;
                    mutableGraph.EdgeAdded += OnMutableGraphEdgeAdded;
                    mutableGraph.EdgeRemoved += OnMutableGraphEdgeRemoved;
                }
            }

            Sizes = null;
        }

        private void DoNotificationLayout()
        {
            lock (_notificationSyncRoot)
            {
                _lastNotificationTimestamp = DateTime.Now;
            }

            if (Worker != null)
                return;

            Worker = new BackgroundWorker();
            Worker.DoWork += (sender, args) =>
            {
                var worker = (BackgroundWorker)sender;
                lock (_notificationSyncRoot)
                {
                    while (DateTime.Now - _lastNotificationTimestamp < _notificationLayoutDelay)
                    {
                        Thread.Sleep(_notificationLayoutDelay);
                        if (worker.CancellationPending)
                            break;
                    }
                }
            };

            Worker.RunWorkerCompleted += (sender, args) =>
            {
                Worker = null;
                OnMutation();
                ContinueLayout();
                HighlightAlgorithm?.ResetHighlight();
            };

            Worker.RunWorkerAsync();
        }

        private void OnMutation()
        {
            while (_edgesRemoved.Count > 0)
            {
                TEdge edge = _edgesRemoved.Dequeue();
                RemoveEdgeControl(edge);
            }

            while (_verticesRemoved.Count > 0)
            {
                TVertex vertex = _verticesRemoved.Dequeue();
                RemoveVertexControl(vertex);
            }

            TVertex[] verticesToInitPos = _verticesAdded.ToArray();
            while (_verticesAdded.Count > 0)
            {
                TVertex vertex = _verticesAdded.Dequeue();
                CreateVertexControl(vertex);
            }

            while (_edgesAdded.Count > 0)
            {
                TEdge edge = _edgesAdded.Dequeue();
                CreateEdgeControl(edge);
            }

            foreach (TVertex vertex in verticesToInitPos)
            {
                InitializePosition(vertex);
            }
        }

        private void OnMutableGraphEdgeRemoved([NotNull] TEdge edge)
        {
            if (EdgesControls.ContainsKey(edge))
            {
                _edgesRemoved.Enqueue(edge);
                DoNotificationLayout();
            }
        }

        private void OnMutableGraphEdgeAdded([NotNull] TEdge edge)
        {
            _edgesAdded.Enqueue(edge);
            DoNotificationLayout();
        }

        private void OnMutableGraphVertexRemoved([NotNull] TVertex vertex)
        {
            if (VerticesControls.ContainsKey(vertex))
            {
                _verticesRemoved.Enqueue(vertex);
                DoNotificationLayout();
            }
        }

        private void OnMutableGraphVertexAdded([NotNull] TVertex vertex)
        {
            _verticesAdded.Enqueue(vertex);
            DoNotificationLayout();
        }

        /// <summary>
        /// Gets the <see cref="VertexControl"/> corresponding to the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Graph vertex.</param>
        /// <returns>The corresponding <see cref="VertexControl"/>, null otherwise.</returns>
        [Pure]
        [CanBeNull]
        public VertexControl GetVertexControl([NotNull] TVertex vertex)
        {
            return VerticesControls.TryGetValue(vertex, out VertexControl control)
                ? control
                : null;
        }

        /// <summary>
        /// Gets or creates a <see cref="VertexControl"/> for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Graph vertex.</param>
        /// <returns>A <see cref="VertexControl"/>.</returns>
        [NotNull]
        protected VertexControl GetOrCreateVertexControl([NotNull] TVertex vertex)
        {
            VertexControl vertexControl = GetVertexControl(vertex);
            if (vertexControl is null)
                return CreateVertexControl(vertex);
            return vertexControl;
        }

        /// <summary>
        /// Creates a <see cref="VertexControl"/> for the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Graph vertex.</param>
        /// <returns>A <see cref="VertexControl"/>.</returns>
        [NotNull]
        protected virtual VertexControl CreateVertexControl([NotNull] TVertex vertex)
        {
            var compoundGraph = Graph as ICompoundGraph<TVertex, TEdge>;

            VertexControl vertexControl;
            if (IsCompoundMode && compoundGraph != null && compoundGraph.IsCompoundVertex(vertex))
            {
                var compoundVertexControl = new CompoundVertexControl
                {
                    Vertex = vertex,
                    DataContext = vertex,
                };
                vertexControl = compoundVertexControl;
            }
            else
            {
                // Create the Control of the vertex
                vertexControl = new VertexControl
                {
                    Vertex = vertex,
                    DataContext = vertex,
                };
            }

            VerticesControls[vertex] = vertexControl;
            vertexControl.RootCanvas = this;

            if (IsCompoundMode && compoundGraph != null && compoundGraph.IsChildVertex(vertex))
            {
                TVertex parent = compoundGraph.GetParent(vertex);

                Debug.Assert(parent != null, "Vertex considered as child one has no parent.");

                var parentControl = GetOrCreateVertexControl(parent) as CompoundVertexControl;

                Debug.Assert(parentControl != null);

                parentControl.Vertices.Add(vertexControl);
            }
            else
            {
                // Add the presenter to the GraphLayout
                Children.Add(vertexControl);
            }

            // Measure & Arrange
            vertexControl.InvalidateMeasure();
            SetHighlightProperties(vertex, vertexControl);
            RunCreationTransition(vertexControl);

            return vertexControl;
        }

        /// <summary>
        /// Initializes the position of the given <paramref name="vertex"/>.
        /// </summary>
        /// <param name="vertex">Graph vertex.</param>
        protected virtual void InitializePosition([NotNull] TVertex vertex)
        {
            VertexControl vertexControl = VerticesControls[vertex];
            // Initialize position
            if (Graph.ContainsVertex(vertex) && Graph.Degree(vertex) > 0)
            {
                var position = default(Point);
                int count = 0;
                foreach (TVertex neighbor in Graph.GetNeighbors(vertex))
                {
                    if (VerticesControls.TryGetValue(neighbor, out VertexControl neighborControl))
                    {
                        double x = GetX(neighborControl);
                        double y = GetY(neighborControl);
                        position.X += double.IsNaN(x) ? 0.0 : x;
                        position.Y += double.IsNaN(y) ? 0.0 : y;
                        ++count;
                    }
                }

                if (count > 0)
                {
                    position.X /= count;
                    position.Y /= count;
                    SetX(vertexControl, position.X);
                    SetY(vertexControl, position.Y);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="EdgeControl"/> corresponding to the given <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">Graph edge.</param>
        /// <returns>The corresponding <see cref="EdgeControl"/>, null otherwise.</returns>
        [Pure]
        [CanBeNull]
        public EdgeControl GetEdgeControl([NotNull] TEdge edge)
        {
            return EdgesControls.TryGetValue(edge, out EdgeControl control)
                ? control
                : null;
        }

        /// <summary>
        /// Gets or creates a <see cref="EdgeControl"/> for the given <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">Graph edge.</param>
        /// <returns>A <see cref="EdgeControl"/>.</returns>
        [NotNull]
        protected EdgeControl GetOrCreateEdgeControl([NotNull] TEdge edge)
        {
            EdgeControl edgeControl = GetEdgeControl(edge);
            if (edgeControl is null)
                return CreateEdgeControl(edge);
            return edgeControl;
        }

        /// <summary>
        /// Creates a <see cref="EdgeControl"/> for the given <paramref name="edge"/>.
        /// </summary>
        /// <param name="edge">Graph edge.</param>
        /// <returns>A <see cref="EdgeControl"/>.</returns>
        [NotNull]
        protected virtual EdgeControl CreateEdgeControl([NotNull] TEdge edge)
        {
            var edgeControl = new EdgeControl
            {
                Edge = edge,
                DataContext = edge
            };

            EdgesControls[edge] = edgeControl;

            // Set the Source and the Target
            edgeControl.Source = VerticesControls[edge.Source];
            edgeControl.Target = VerticesControls[edge.Target];

            if (ActualLayoutMode == Algorithms.Layout.LayoutMode.Simple)
                Children.Insert(0, edgeControl);
            else
                Children.Add(edgeControl);
            SetHighlightProperties(edge, edgeControl);
            RunCreationTransition(edgeControl);

            return edgeControl;
        }
    }
}
