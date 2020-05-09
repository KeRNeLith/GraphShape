using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GraphShape.Algorithms.EdgeRouting;
using GraphShape.Algorithms.Highlight;
using GraphShape.Algorithms.Layout;
using GraphShape.Algorithms.OverlapRemoval;
using GraphShape.Controls.Extensions;
using GraphShape.Controls.Utils;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Controls
{
    /// <summary>
    /// Default graph layout control.
    /// </summary>
    /// <remarks>For general purposes, with general types.</remarks>
    public class GraphLayout : GraphLayout<object, IEdge<object>, IBidirectionalGraph<object, IEdge<object>>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphLayout"/> class.
        /// </summary>
        public GraphLayout()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                var graph = new BidirectionalGraph<object, IEdge<object>>();
                var vertices = new object[] { "S", "A", "M", "P", "L", "E" };
                var edges = new IEdge<object>[]
                {
                    new Edge<object>(vertices[0], vertices[1]),
                    new Edge<object>(vertices[1], vertices[2]),
                    new Edge<object>(vertices[1], vertices[3]),
                    new Edge<object>(vertices[3], vertices[4]),
                    new Edge<object>(vertices[0], vertices[4]),
                    new Edge<object>(vertices[4], vertices[5])
                };

                graph.AddVerticesAndEdgeRange(edges);
                OverlapRemovalAlgorithmType = "FSA";
                LayoutAlgorithmType = "FR";
                Graph = graph;
            }
        }
    }

    /// <summary>
    /// Graph layout control. Support layout, edge routing and overlap removal algorithms, with multiple layout states.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public partial class GraphLayout<TVertex, TEdge, TGraph> : GraphCanvas
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        [NotNull]
        private readonly object _notificationSyncRoot = new object();

        [NotNull, ItemNotNull]
        private readonly List<LayoutState<TVertex, TEdge>> _layoutStates = new List<LayoutState<TVertex, TEdge>>();

        private readonly TimeSpan _notificationLayoutDelay = new TimeSpan(0, 0, 0, 0, 5); // 5 ms

        /// <summary>
        /// Edges controls.
        /// </summary>
        [NotNull]
        protected readonly Dictionary<TEdge, EdgeControl> EdgesControls = new Dictionary<TEdge, EdgeControl>();
        
        [NotNull, ItemNotNull]
        private readonly Queue<TEdge> _edgesAdded = new Queue<TEdge>();

        [NotNull, ItemNotNull]
        private readonly Queue<TEdge> _edgesRemoved = new Queue<TEdge>();

        /// <summary>
        /// Vertices controls.
        /// </summary>
        [NotNull]
        protected readonly Dictionary<TVertex, VertexControl> VerticesControls = new Dictionary<TVertex, VertexControl>();

        [NotNull, ItemNotNull]
        private readonly Queue<TVertex> _verticesAdded = new Queue<TVertex>();

        [NotNull, ItemNotNull]
        private readonly Queue<TVertex> _verticesRemoved = new Queue<TVertex>();
        
        [NotNull]
        private readonly Stopwatch _stopWatch = new Stopwatch();
        
        private DateTime _lastNotificationTimestamp = DateTime.Now;

        /// <summary>
        /// Vertices sizes.
        /// </summary>
        [CanBeNull]
        protected IDictionary<TVertex, SizeF> Sizes;
        
        /// <summary>
        /// Background worker to run layout algorithm.
        /// </summary>
        protected BackgroundWorker Worker;

        /// <inheritdoc cref="Sizes"/>
        [NotNull]
        protected IDictionary<TVertex, SizeF> VerticesSizes
        {
            get
            {
                if (Sizes is null)
                {
                    InvalidateMeasure();
                    UpdateLayout();
                    Sizes = new Dictionary<TVertex, SizeF>();

                    foreach (KeyValuePair<TVertex, VertexControl> pair in VerticesControls)
                    {
                        System.Windows.Size size = pair.Value.DesiredSize;
                        Sizes.Add(
                            pair.Key,
                            new SizeF(
                                double.IsNaN(size.Width) ? 0 : (float)size.Width,
                                double.IsNaN(size.Height) ? 0 : (float)size.Height));
                    }
                }

                return Sizes;
            }
        }

        #region Helpers

        private void RunOnDispatcherThread([NotNull] Action action)
        {
            Debug.Assert(action != null);

            Dispatcher dispatcher = Application.Current?.Dispatcher ?? Dispatcher;
            if (dispatcher.CheckAccess())
            {
                action(); // Already on UI thread => direct call
            }
            else
            {
                dispatcher.Invoke(action);
            }
        }

        #endregion

        #region Layout

        private class AsyncThreadArgument
        {
            [NotNull]
            public ILayoutAlgorithm<TVertex, TEdge, TGraph> Algorithm { get; }
            
            public bool ShowAllStates { get; }

            public AsyncThreadArgument(
                [NotNull] ILayoutAlgorithm<TVertex, TEdge, TGraph> algorithm,
                bool showAllStates)
            {
                Algorithm = algorithm;
                ShowAllStates = showAllStates;
            }
        }

        [Pure]
        private static Algorithms.Layout.LayoutMode GetLayoutMode(LayoutMode mode, [CanBeNull] TGraph graph)
        {
            if (mode == LayoutMode.Compound || mode == LayoutMode.Automatic && graph is ICompoundGraph<TVertex, TEdge>)
                return Algorithms.Layout.LayoutMode.Compound;
            return Algorithms.Layout.LayoutMode.Simple;
        }

        [Pure]
        private static bool IsCompoundModeInternal(Algorithms.Layout.LayoutMode mode)
        {
            return mode == Algorithms.Layout.LayoutMode.Compound;
        }

        [Pure]
        private static bool IsCompoundModeInternal(LayoutMode mode, [CanBeNull] TGraph graph)
        {
            return IsCompoundModeInternal(GetLayoutMode(mode, graph));
        }

        /// <summary>
        /// Current <see cref="Algorithms.Layout.LayoutMode"/>.
        /// </summary>
        protected Algorithms.Layout.LayoutMode ActualLayoutMode => GetLayoutMode(LayoutMode, Graph);

        /// <summary>
        /// Indicates if <see cref="ActualLayoutMode"/> is a <see cref="Algorithms.Layout.LayoutMode.Compound"/> mode.
        /// </summary>
        protected bool IsCompoundMode => IsCompoundModeInternal(ActualLayoutMode);

        /// <summary>
        /// Indicates if layout can be done.
        /// </summary>
        protected virtual bool CanLayout { get; } = true;

        /// <inheritdoc />
        public override void ContinueLayout()
        {
            Layout(true);
        }

        /// <inheritdoc />
        public override void Relayout()
        {
            // Clear the states before
            _layoutStates.Clear();
            Layout(false);
        }

        /// <summary>
        /// Cancels layout computing.
        /// </summary>
        public void CancelLayout()
        {
            if (Worker != null && Worker.IsBusy && Worker.WorkerSupportsCancellation)
                Worker.CancelAsync();
        }

        /// <summary>
        /// Re-compute edge routing.
        /// </summary>
        public void RecalculateEdgeRouting()
        {
            foreach (LayoutState<TVertex, TEdge> state in _layoutStates)
            {
                state.RouteInfos = RouteEdges(state.OverlapRemovedPositions, GetLatestVerticesSizes());
            }

            ChangeState(StateIndex);
        }

        /// <summary>
        /// Re-compute overlap removal.
        /// </summary>
        public void RecalculateOverlapRemoval()
        {
            foreach (LayoutState<TVertex, TEdge> state in _layoutStates)
            {
                state.OverlapRemovedPositions = OverlapRemoval(state.Positions, GetLatestVerticesSizes());
            }

            ChangeState(StateIndex);
        }

        /// <summary>
        /// Creates a <see cref="IHighlightContext{TVertex,TEdge,TGraph}"/> from given parameters.
        /// </summary>
        /// <returns>Created <see cref="IHighlightContext{TVertex,TEdge,TGraph}"/>.</returns>
        [Pure]
        [NotNull]
        protected virtual IHighlightContext<TVertex, TEdge, TGraph> CreateHighlightContext()
        {
            return new HighlightContext<TVertex, TEdge, TGraph>(Graph);
        }

        /// <summary>
        /// Creates a <see cref="IOverlapRemovalContext{TVertex}"/> from given parameters.
        /// </summary>
        /// <param name="positions">Vertices positions.</param>
        /// <param name="sizes">Vertices sizes.</param>
        /// <returns>Created <see cref="IOverlapRemovalContext{TVertex}"/>.</returns>
        [Pure]
        [NotNull]
        protected virtual IOverlapRemovalContext<TVertex> CreateOverlapRemovalContext(
            [NotNull] IDictionary<TVertex, Point> positions,
            [NotNull] IDictionary<TVertex, Size> sizes)
        {
            if (positions is null)
                throw new ArgumentNullException(nameof(positions));
            if (sizes is null)
                throw new ArgumentNullException(nameof(sizes));

            var rectangles = new Dictionary<TVertex, Rect>();
            foreach (TVertex vertex in Graph.Vertices)
            {
                if (!positions.TryGetValue(vertex, out Point position) || !sizes.TryGetValue(vertex, out Size size))
                    continue;

                rectangles[vertex] = new Rect(
                    position.X - size.Width * (float)0.5,
                    position.Y - size.Height * (float)0.5,
                    size.Width,
                    size.Height);
            }

            return new OverlapRemovalContext<TVertex>(rectangles);
        }

        /// <summary>
        /// Creates a <see cref="ILayoutContext{TVertex,TEdge,TGraph}"/> from given parameters.
        /// </summary>
        /// <param name="positions">Vertices positions.</param>
        /// <param name="sizes">Vertices sizes.</param>
        /// <returns>Created <see cref="ILayoutContext{TVertex,TEdge,TGraph}"/>, null if <see cref="CanLayout"/> is false.</returns>
        [Pure]
        protected virtual ILayoutContext<TVertex, TEdge, TGraph> CreateLayoutContext(
            [CanBeNull] IDictionary<TVertex, Point> positions,
            [NotNull] IDictionary<TVertex, Size> sizes)
        {
            if (sizes is null)
                throw new ArgumentNullException(nameof(sizes));

            if (!CanLayout)
                return null;

            if (ActualLayoutMode == Algorithms.Layout.LayoutMode.Simple)
                return new LayoutContext<TVertex, TEdge, TGraph>(Graph, positions, sizes, ActualLayoutMode);

            Dictionary<TVertex, Thickness> borders = VerticesControls
                .Where(pair => pair.Value is CompoundVertexControl)
                .ToDictionary(
                    pair => pair.Key,
                    pair => ((CompoundVertexControl)pair.Value).VertexBorderThickness);

            Dictionary<TVertex, CompoundVertexInnerLayoutType> layoutTypes = VerticesControls
                .Where(pair => pair.Value is CompoundVertexControl)
                .ToDictionary(
                    pair => pair.Key,
                    pair => ((CompoundVertexControl)pair.Value).LayoutMode);

            return new CompoundLayoutContext<TVertex, TEdge, TGraph>(Graph, positions, sizes, ActualLayoutMode, borders, layoutTypes);
        }

        /// <summary>
        /// Layouts the current <see cref="Graph"/>.
        /// </summary>
        /// <param name="continueLayout">
        /// Indicates if we should continue layout.
        /// If layout is continued it gets the relative position of every vertex
        /// otherwise it's gets it only for the vertices with fixed parents.
        /// </param>
        protected virtual void Layout(bool continueLayout)
        {
            if (Graph is null || Graph.VertexCount == 0 || !LayoutAlgorithmFactory.IsValidAlgorithm(LayoutAlgorithmType) || !CanLayout)
                return; // No graph to layout, or wrong layout algorithm

            UpdateLayout();
            if (!IsLoaded)
            {
                void Handler(object sender, RoutedEventArgs args)
                {
                    Layout(continueLayout);
                    var graphLayout = (GraphLayout<TVertex, TEdge, TGraph>)args.Source;
                    graphLayout.Loaded -= Handler;
                }

                Loaded += Handler;
                return;
            }

            // Get the actual positions if we want to continue the layout
            IDictionary<TVertex, Point> oldPositions = GetOldVerticesPositions(continueLayout);
            IDictionary<TVertex, Size> oldSizes = GetLatestVerticesSizes();

            // Create the context
            ILayoutContext<TVertex, TEdge, TGraph> layoutContext = CreateLayoutContext(oldPositions, oldSizes);

            // Create the layout algorithm using the factory
            LayoutAlgorithm = LayoutAlgorithmFactory.CreateAlgorithm(
                LayoutAlgorithmType,
                layoutContext,
                LayoutParameters);
            if (LayoutAlgorithm is null)
                return;

            if (AsyncCompute)
            {
                // Asynchronous computing - progress report & anything else
                // If there's a running progress than cancel that
                RunAsynchronousLayout();
            }
            else
            {
                // Synchronous computing - no progress report
                RunSynchronousLayout();
            }
        }

        private void RunAsynchronousLayout()
        {
            CancelLayout();

            Worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            // Run the algorithm on a background thread
            Worker.DoWork += (sender, args) =>
            {
                var worker = (BackgroundWorker) sender;
                var argument = (AsyncThreadArgument) args.Argument;

                using (AlgorithmScope())
                {
                    argument.Algorithm.Compute();
                }

                #region Local functions

                IDisposable AlgorithmScope()
                {
                    argument.Algorithm.Started += OnLayoutAlgorithmStarted;

                    if (argument.ShowAllStates)
                        argument.Algorithm.IterationEnded += OnLayoutAlgorithmIterationEnded;
                    else
                        argument.Algorithm.ProgressChanged += OnLayoutAlgorithmProgress;

                    argument.Algorithm.Finished += OnLayoutAlgorithmFinished;

                    return DisposableHelpers.Finally(() =>
                    {
                        argument.Algorithm.Finished -= OnLayoutAlgorithmFinished;

                        if (argument.ShowAllStates)
                            argument.Algorithm.IterationEnded -= OnLayoutAlgorithmIterationEnded;
                        else
                            argument.Algorithm.ProgressChanged -= OnLayoutAlgorithmProgress;

                        argument.Algorithm.Started -= OnLayoutAlgorithmStarted;
                    });
                }

                void OnLayoutAlgorithmStarted(object s, EventArgs _)
                {
                    RunOnDispatcherThread(OnLayoutStarted);
                }

                void OnLayoutAlgorithmIterationEnded(object _, ILayoutIterationEventArgs<TVertex> iterationsArgs)
                {
                    if (iterationsArgs != null)
                    {
                        worker.ReportProgress((int)Math.Round(iterationsArgs.StatusInPercent), iterationsArgs);
                        iterationsArgs.Abort = worker.CancellationPending;
                    }
                }

                void OnLayoutAlgorithmProgress(object _, double percent)
                {
                    worker.ReportProgress((int) Math.Round(percent));
                }

                void OnLayoutAlgorithmFinished(object s, EventArgs _)
                {
                    RunOnDispatcherThread(OnLayoutFinished);
                }

                #endregion
            };

            // Progress changed if an iteration ended
            Worker.ProgressChanged += (sender, args) =>
            {
                if (args.UserState is null)
                    LayoutStatusPercent = args.ProgressPercentage;
                else
                    OnLayoutIterationFinished(args.UserState as ILayoutIterationEventArgs<TVertex>);
            };

            // Background thread finished if the iteration ended
            Worker.RunWorkerCompleted += (sender, args) =>
            {
                Worker = null;
            };

            Worker.RunWorkerAsync(new AsyncThreadArgument(LayoutAlgorithm, ShowAllStates));
        }

        private void RunSynchronousLayout()
        {
            ILayoutAlgorithm<TVertex, TEdge, TGraph> layoutAlgorithm = LayoutAlgorithm;
            using (AlgorithmScope(layoutAlgorithm))
            {
                layoutAlgorithm.Compute();
            }

            #region Local functions

            IDisposable AlgorithmScope(ILayoutAlgorithm<TVertex, TEdge, TGraph> algorithm)
            {
                algorithm.Started += OnLayoutAlgorithmStarted;
                bool showAllStates = ShowAllStates;
                if (showAllStates)
                    algorithm.IterationEnded += OnLayoutAlgorithmIterationEnded;
                algorithm.Finished += OnLayoutAlgorithmFinished;

                return DisposableHelpers.Finally(() =>
                {
                    LayoutAlgorithm.Finished -= OnLayoutAlgorithmFinished;
                    if (showAllStates)
                        LayoutAlgorithm.IterationEnded -= OnLayoutAlgorithmIterationEnded;
                    LayoutAlgorithm.Started -= OnLayoutAlgorithmStarted;
                });
            }

            void OnLayoutAlgorithmStarted(object sender, EventArgs args)
            {
                OnLayoutStarted();
            }

            void OnLayoutAlgorithmIterationEnded(object sender, ILayoutIterationEventArgs<TVertex> args)
            {
                OnLayoutIterationFinished(args);
            }

            void OnLayoutAlgorithmFinished(object sender, EventArgs args)
            {
                OnLayoutFinished();
            }

            #endregion
        }

        [Pure]
        [CanBeNull]
        private IDictionary<TVertex, Point> GetOldVerticesPositions(bool continueLayout)
        {
            if (ActualLayoutMode == Algorithms.Layout.LayoutMode.Simple)
            {
                return continueLayout ? GetLatestVerticesPositions() : null;
            }

            return GetRelativePositions(continueLayout);
        }

        [Pure]
        [NotNull]
        private IDictionary<TVertex, Point> GetLatestVerticesPositions()
        {
            IDictionary<TVertex, Point> verticesPositions = new Dictionary<TVertex, Point>(VerticesControls.Count);

            // Go through the vertices presenters and get the actual layout positions
            if (ActualLayoutMode == Algorithms.Layout.LayoutMode.Simple)
            {
                foreach (KeyValuePair<TVertex, VertexControl> pair in VerticesControls)
                {
                    double x = GetX(pair.Value);
                    double y = GetY(pair.Value);
                    verticesPositions[pair.Key] = new Point(
                        double.IsNaN(x) ? 0.0 : x,
                        double.IsNaN(y) ? 0.0 : y);
                }
            }
            else
            {
                var topLeft = default(System.Windows.Point);
                foreach (KeyValuePair<TVertex, VertexControl> pair in VerticesControls)
                {
                    System.Windows.Point position = pair.Value.TranslatePoint(topLeft, this);
                    position.X += pair.Value.ActualWidth / 2;
                    position.Y += pair.Value.ActualHeight / 2;
                    verticesPositions[pair.Key] = position.ToGraphShapePoint();
                }
            }

            return verticesPositions;
        }

        [Pure]
        private static System.Windows.Point GetRelativePosition([NotNull] VertexControl vertexControl, [CanBeNull] UIElement relativeTo)
        {
            Debug.Assert(vertexControl != null);

            return vertexControl.TranslatePoint(
                new System.Windows.Point(vertexControl.ActualWidth / 2.0, vertexControl.ActualHeight / 2.0),
                relativeTo);
        }

        [Pure]
        private System.Windows.Point GetRelativePosition([NotNull] VertexControl vertexControl)
        {
            Debug.Assert(vertexControl != null);

            return GetRelativePosition(vertexControl, this);
        }

        [Pure]
        [NotNull]
        private IDictionary<TVertex, Point> GetRelativePositions(bool continueLayout)
        {
            // If layout is continued it gets the relative position of every vertex
            // otherwise it's gets it only for the vertices with fixed parents
            var verticesPositions = new Dictionary<TVertex, Point>(VerticesControls.Count);
            if (continueLayout)
            {
                foreach (KeyValuePair<TVertex, VertexControl> pair in VerticesControls)
                {
                    verticesPositions[pair.Key] = GetRelativePosition(pair.Value).ToGraphShapePoint();
                }
            }
            else
            {
                foreach (KeyValuePair<TVertex, VertexControl> pair in VerticesControls.Where(pair => pair.Value is CompoundVertexControl))
                {
                    var compoundVertexControl = (CompoundVertexControl)pair.Value;
                    foreach (VertexControl vertexControl in compoundVertexControl.Vertices)
                    {
                        verticesPositions[(TVertex)vertexControl.Vertex] = GetRelativePosition(vertexControl, compoundVertexControl).ToGraphShapePoint();
                    }
                }
            }

            return verticesPositions;
        }

        [NotNull]
        private IDictionary<TVertex, Size> GetLatestVerticesSizes()
        {
            if (!IsMeasureValid)
                Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));

            IDictionary<TVertex, Size> verticesSizes = new Dictionary<TVertex, Size>(VerticesControls.Count);

            // Go through the vertices presenters and get the actual layout positions
            foreach (KeyValuePair<TVertex, VertexControl> pair in VerticesControls)
                verticesSizes[pair.Key] = new Size(pair.Value.ActualWidth, pair.Value.ActualHeight);

            return verticesSizes;
        }

        /// <summary>
        /// Layout process started callback.
        /// </summary>
        protected void OnLayoutStarted()
        {
            _stopWatch.Reset();
            _stopWatch.Start();
            LayoutStatusPercent = 0.0;
        }

        /// <summary>
        /// Layout process iteration ended callback.
        /// </summary>
        /// <param name="iterationArgs">Iteration arguments.</param>
        protected virtual void OnLayoutIterationFinished([CanBeNull] ILayoutIterationEventArgs<TVertex> iterationArgs)
        {
            if (iterationArgs?.VerticesPositions is null)
            {
                OnLayoutIterationFinished(LayoutAlgorithm.VerticesPositions, null);
                SetLayoutInformation();
            }
            else
            {
                OnLayoutIterationFinished(iterationArgs.VerticesPositions, iterationArgs.Message);
                LayoutStatusPercent = iterationArgs.StatusInPercent;
                SetLayoutInformation(iterationArgs as ILayoutInfoIterationEventArgs<TVertex, TEdge>);
            }
        }

        /// <summary>
        /// Layout process iteration ended callback.
        /// </summary>
        /// <param name="positions">Vertices positions.</param>
        /// <param name="message">Iteration information message.</param>
        protected virtual void OnLayoutIterationFinished(
            [NotNull] IDictionary<TVertex, Point> positions,
            [CanBeNull] string message)
        {
            if (positions is null)
                throw new ArgumentNullException(nameof(positions));

            IDictionary<TVertex, Size> sizes = GetLatestVerticesSizes();
            IDictionary<TVertex, Point> overlapRemovedPositions = OverlapRemoval(positions, sizes);
            IDictionary<TEdge, Point[]> edgeRoutingInfos = RouteEdges(overlapRemovedPositions, sizes);

            var state = new LayoutState<TVertex, TEdge>(
                positions,
                overlapRemovedPositions,
                edgeRoutingInfos,
                _stopWatch.Elapsed,
                _layoutStates.Count,
                message ?? string.Empty);

            _layoutStates.Add(state);
            StateCount = _layoutStates.Count;
        }

        /// <summary>
        /// Layout process ended callback.
        /// </summary>
        protected virtual void OnLayoutFinished()
        {
            _stopWatch.Stop();
            OnLayoutIterationFinished(null);
            StateIndex = StateCount - 1;

            // Animating to the finish state
            if (StateIndex == 0)
                ChangeState(StateIndex);

            LayoutStatusPercent = 100;
        }

        private void SetLayoutInformation([CanBeNull] ILayoutInfoIterationEventArgs<TVertex, TEdge> iterationArgs)
        {
            if (iterationArgs is null)
                return;

            foreach (KeyValuePair<TVertex, VertexControl> pair in VerticesControls)
            {
                TVertex vertex = pair.Key;
                VertexControl control = pair.Value;

                GraphElementBehaviour.SetLayoutInfo(control, iterationArgs.GetVertexInfo(vertex));
            }

            foreach (KeyValuePair<TEdge, EdgeControl> pair in EdgesControls)
            {
                TEdge edge = pair.Key;
                EdgeControl control = pair.Value;

                GraphElementBehaviour.SetLayoutInfo(control, iterationArgs.GetEdgeInfo(edge));
            }
        }

        private void SetLayoutInformation()
        {
            if (LayoutAlgorithm is null)
                return;

            foreach (KeyValuePair<TVertex, VertexControl> pair in VerticesControls)
            {
                TVertex vertex = pair.Key;
                VertexControl control = pair.Value;

                GraphElementBehaviour.SetLayoutInfo(control, LayoutAlgorithm.GetVertexInfo(vertex));
            }

            foreach (KeyValuePair<TEdge, EdgeControl> pair in EdgesControls)
            {
                TEdge edge = pair.Key;
                EdgeControl control = pair.Value;

                GraphElementBehaviour.SetLayoutInfo(control, LayoutAlgorithm.GetEdgeInfo(edge));
            }
        }

        /// <summary>
        /// Runs overlap removal algorithm.
        /// </summary>
        /// <param name="positions">Vertices positions.</param>
        /// <param name="sizes">Vertices sizes.</param>
        /// <returns>
        /// Vertices positions after overlap removal process (if run),
        /// otherwise return input <paramref name="positions"/>.
        /// </returns>
        [CanBeNull]
        protected IDictionary<TVertex, Point> OverlapRemoval(
            [CanBeNull] IDictionary<TVertex, Point> positions,
            [CanBeNull] IDictionary<TVertex, Size> sizes)
        {
            if (positions is null || sizes is null)
                return positions; // Not valid positions or sizes

            bool isValidAlgorithm = OverlapRemovalAlgorithmFactory.IsValidAlgorithm(OverlapRemovalAlgorithmType);
            if (OverlapRemovalConstraint == AlgorithmConstraints.Skip
                ||
                OverlapRemovalConstraint == AlgorithmConstraints.Automatic &&
                (!LayoutAlgorithmFactory.NeedOverlapRemoval(LayoutAlgorithmType) || !isValidAlgorithm)
                ||
                OverlapRemovalConstraint == AlgorithmConstraints.Must && !isValidAlgorithm)
            {
                return positions;
            }

            // Create the algorithm parameters based on the old parameters
            OverlapRemovalParameters = OverlapRemovalAlgorithmFactory.CreateParameters(
                OverlapRemovalAlgorithmType,
                OverlapRemovalParameters);

            // Create the context - rectangles, ...
            IOverlapRemovalContext<TVertex> context = CreateOverlapRemovalContext(positions, sizes);

            // Create the concrete algorithm
            OverlapRemovalAlgorithm = OverlapRemovalAlgorithmFactory.CreateAlgorithm(
                OverlapRemovalAlgorithmType,
                context,
                OverlapRemovalParameters);

            if (OverlapRemovalAlgorithm != null)
            {
                OverlapRemovalAlgorithm.Compute();

                var result = new Dictionary<TVertex, Point>();
                foreach (KeyValuePair<TVertex, Rect> res in OverlapRemovalAlgorithm.Rectangles)
                {
                    result[res.Key] = new Point(
                        res.Value.Left + res.Value.Size.Width * 0.5,
                        res.Value.Top + res.Value.Size.Height * 0.5);
                }

                return result;
            }

            return positions;
        }

        /// <summary>
        /// Runs the proper edge routing algorithm.
        /// </summary>
        /// <param name="positions">The vertices positions.</param>
        /// <param name="sizes">The vertices sizes.</param>
        /// <returns>The routes of the edges.</returns>
        [CanBeNull]
        protected IDictionary<TEdge, Point[]> RouteEdges(
            [CanBeNull] IDictionary<TVertex, Point> positions,
            [NotNull] IDictionary<TVertex, Size> sizes)
        {
            if (sizes is null)
                throw new ArgumentNullException(nameof(sizes));

            IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph> algorithm = null;
            bool isValidAlgorithmType = EdgeRoutingAlgorithmFactory.IsValidAlgorithm(EdgeRoutingAlgorithmType);

            if (EdgeRoutingConstraint == AlgorithmConstraints.Must && isValidAlgorithmType)
            {
                // An edge routing algorithm must be used
                EdgeRoutingParameters = EdgeRoutingAlgorithmFactory.CreateParameters(
                    EdgeRoutingAlgorithmType,
                    EdgeRoutingParameters);

                ILayoutContext<TVertex, TEdge, TGraph> context = CreateLayoutContext(positions, sizes);

                algorithm = EdgeRoutingAlgorithmFactory.CreateAlgorithm(
                    EdgeRoutingAlgorithmType,
                    context,
                    EdgeRoutingParameters);
                algorithm.Compute();
            }
            else if (EdgeRoutingConstraint == AlgorithmConstraints.Automatic)
            {
                if (!LayoutAlgorithmFactory.NeedEdgeRouting(LayoutAlgorithmType)
                    && LayoutAlgorithm is IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph>)
                {
                    // The layout algorithm routes the edges
                    algorithm = LayoutAlgorithm as IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph>;
                }
                else if (isValidAlgorithmType)
                {
                    // The selected EdgeRouting algorithm will route the edges
                    EdgeRoutingParameters = EdgeRoutingAlgorithmFactory.CreateParameters(
                        EdgeRoutingAlgorithmType,
                        EdgeRoutingParameters);

                    ILayoutContext<TVertex, TEdge, TGraph> context = CreateLayoutContext(positions, sizes);
                    algorithm = EdgeRoutingAlgorithmFactory.CreateAlgorithm(
                        EdgeRoutingAlgorithmType,
                        context,
                        EdgeRoutingParameters);

                    algorithm.Compute();
                }
            }

            // Route the edges
            return algorithm?.EdgeRoutes;
        }

        /// <summary>
        /// Changes which layout state should be shown.
        /// </summary>
        /// <param name="stateIndex">The index of the shown layout state.</param>
        protected void ChangeState(int stateIndex)
        {
            LayoutState<TVertex, TEdge> activeState = _layoutStates[stateIndex];

            LayoutState = activeState;

            IDictionary<TVertex, Point> positions = activeState.OverlapRemovedPositions;
            if (positions != null)
            {
                ApplyVerticesPositions(positions);
            }

            IDictionary<TEdge, Point[]> routeInfos = activeState.RouteInfos;
            if (routeInfos != null)
            {
                ApplyEdgesRoutes(routeInfos);
            }
        }

        private void ApplyVerticesPositions([NotNull] IDictionary<TVertex, Point> positions)
        {
            // Animate the vertices
            foreach (TVertex vertex in Graph.Vertices)
            {
                if (!VerticesControls.TryGetValue(vertex, out VertexControl control))
                    continue;

                if (positions.TryGetValue(vertex, out Point pos))
                    RunMoveAnimation(control, pos.X, pos.Y);
            }
        }

        private void ApplyEdgesRoutes([NotNull] IDictionary<TEdge, Point[]> routeInfos)
        {
            // Change the edge routes
            foreach (TEdge edge in Graph.Edges)
            {
                if (!EdgesControls.TryGetValue(edge, out EdgeControl control))
                    continue;

                control.RoutePoints = routeInfos.TryGetValue(edge, out Point[] routePoints)
                    ? routePoints.ToPoints().ToArray()
                    : null;
            }
        }

        #endregion
    }
}