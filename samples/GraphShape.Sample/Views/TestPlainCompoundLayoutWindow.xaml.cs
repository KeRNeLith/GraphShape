using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GraphShape.Algorithms.Layout.Compound;
using GraphShape.Algorithms.Layout.Compound.FDP;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Sample
{
    /// <summary>
    /// Interaction logic for TestPlainCompoundLayoutWindow.xaml
    /// </summary>
    internal partial class TestPlainCompoundLayoutWindow
    {
        private const int BigGraph = 0;
        private const int SmallGraph = 1;
        private const int FlatGraph = 2;
        private const int RepGraph = 3;
        private const int StarGraph = 4;
        private const int CombinedGraph = 5;

        private bool _paused;

        [NotNull, ItemNotNull]
        private readonly CompoundGraph<object, IEdge<object>>[] _graphs;

        [NotNull]
        private readonly CompoundFDPLayoutParameters _parameters = new CompoundFDPLayoutParameters();

        public TestPlainCompoundLayoutWindow()
        {
            InitializeComponent();
            _graphs = InitGraphs();

            DataContext = _parameters;
        }

        #region Helpers

        [NotNull, ItemNotNull]
        private static string[] InitVertices([NotNull] IMutableVertexSet<object> graph, int vertexCount)
        {
            var vertices = new string[vertexCount];
            for (int i = 0; i < vertexCount; ++i)
            {
                vertices[i] = i.ToString();
                graph.AddVertex(vertices[i]);
            }

            return vertices;
        }

        [Pure]
        [NotNull]
        private static Line CreateLine(Point startPoint, Vector vector, [NotNull] Brush color)
        {
            Debug.Assert(color != null);

            return new Line
            {
                X1 = startPoint.X,
                X2 = startPoint.X + vector.X,
                Y1 = startPoint.Y,
                Y2 = startPoint.Y + vector.Y,
                Fill = color,
                Stroke = color,
                StrokeThickness = 1
            };
        }

        #endregion

        [NotNull]
        private readonly IDictionary<object, Rectangle> _rectangles = new Dictionary<object, Rectangle>();

        [NotNull]
        private readonly IDictionary<object, Line> _lines = new Dictionary<object, Line>();

        [NotNull, ItemNotNull]
        private readonly IList<Line> _forceLines = new List<Line>();

        private void ShowGraph(int graphIndex)
        {
            CompoundGraph<object, IEdge<object>> graph = _graphs[graphIndex];

            _rectangles.Clear();
            _lines.Clear();
            Layout.Children.Clear();

            var origo = new Ellipse
            {
                Width = 100,
                Height = 100,
                Fill = System.Windows.Media.Brushes.Black,
                OpacityMask = new RadialGradientBrush(Colors.Black, Colors.Transparent)
            };

            Canvas.SetLeft(origo, -5);
            Canvas.SetTop(origo, -5);
            Layout.Children.Add(origo);

            var sizes = new Dictionary<object, Size>();
            var borders = new Dictionary<object, Thickness>();
            var layoutType = new Dictionary<object, CompoundVertexInnerLayoutType>();

            var size = new Size(20, 20);
            foreach (object vertex in graph.SimpleVertices)
            {
                sizes[vertex] = size;
            }

            var thickness = new Thickness(5, 10, 5, 5);
            foreach (object vertex in graph.CompoundVertices)
            {
                sizes[vertex] = default;
                borders[vertex] = thickness;
                layoutType[vertex] = CompoundVertexInnerLayoutType.Automatic;
            }

            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            worker.DoWork += (sender, args) =>
            {
                var layoutAlgorithm = new CompoundFDPLayoutAlgorithm<object, IEdge<object>, ICompoundGraph<object, IEdge<object>>>(
                    graph,
                    sizes,
                    borders,
                    layoutType,
                    null,
                    _parameters);

                layoutAlgorithm.IterationEnded += (s, iterationArgs) =>
                {
                    var testIterationArgs = iterationArgs as TestingCompoundLayoutIterationEventArgs<object, IEdge<object>, TestingCompoundVertexInfo, object>;

                    IDictionary<object, Point> positions = testIterationArgs?.VerticesPositions;
                    if (positions is null)
                        return;

                    IDictionary<object, Size> innerSizes = ((ICompoundLayoutIterationEventArgs<object>)testIterationArgs).InnerCanvasSizes;
                    IDictionary<object, TestingCompoundVertexInfo> verticesInfos = testIterationArgs.VerticesInfos;

                    Dispatcher.Invoke(() =>
                    {
                        var points = new Dictionary<object, Point>();

                        var compoundVerticesToCheck = new Queue<object>();
                        var compoundVertices = new List<object>();
                        var roots = graph.CompoundVertices.Where(vertex => graph.GetParent(vertex) is null);
                        foreach (object root in roots)
                            compoundVerticesToCheck.Enqueue(root);

                        while (compoundVerticesToCheck.Count > 0)
                        {
                            object next = compoundVerticesToCheck.Dequeue();
                            if (!graph.IsCompoundVertex(next))
                                continue;

                            compoundVertices.Add(next);
                            foreach (object childrenVertex in graph.GetChildrenVertices(next))
                            {
                                compoundVerticesToCheck.Enqueue(childrenVertex);
                            }
                        }

                        // Draw the compound vertices
                        foreach (object vertex in compoundVertices)
                        {
                            Size innerSize = innerSizes[vertex];
                            innerSize.Width += thickness.Left + thickness.Right;
                            innerSize.Height += thickness.Top + thickness.Bottom;

                            Point pos = positions[vertex];
                            points[vertex] = pos;
                            AddRectangle(vertex, pos, innerSize);
                        }

                        // Draw the simple vertices
                        foreach (object vertex in graph.SimpleVertices)
                        {
                            Point pos = positions[vertex];
                            points[vertex] = pos;
                            AddRectangle(vertex, pos, size);
                        }

                        // Draw the simple edges
                        foreach (IEdge<object> edge in graph.Edges)
                        {
                            Point pos1 = points[edge.Source];
                            Point pos2 = points[edge.Target];
                            AddLine(edge, pos1, pos2, true);
                        }

                        // Draw the containment edges
                        foreach (object vertex in graph.CompoundVertices)
                        {
                            Point pos1 = points[vertex];
                            foreach (object child in graph.GetChildrenVertices(vertex))
                            {
                                Point pos2 = points[child];
                                AddLine(child, pos1, pos2, false);
                            }
                        }

                        // Draw the lines of the forces
                        foreach (Line forceLine in _forceLines)
                            Layout.Children.Remove(forceLine);

                        SolidColorBrush springColor = System.Windows.Media.Brushes.Orange;
                        SolidColorBrush repulsionColor = System.Windows.Media.Brushes.Red;
                        SolidColorBrush gravityColor = System.Windows.Media.Brushes.Green;
                        SolidColorBrush applicationColor = System.Windows.Media.Brushes.Yellow;

                        foreach (KeyValuePair<object, TestingCompoundVertexInfo> pair in verticesInfos)
                        {
                            Line line = CreateLine(points[pair.Key], pair.Value.SpringForce, springColor);
                            Layout.Children.Add(line);
                            _forceLines.Add(line);

                            line = CreateLine(points[pair.Key], pair.Value.RepulsionForce, repulsionColor);
                            Layout.Children.Add(line);
                            _forceLines.Add(line);

                            line = CreateLine(points[pair.Key], pair.Value.GravityForce, gravityColor);
                            Layout.Children.Add(line);
                            _forceLines.Add(line);

                            line = CreateLine(points[pair.Key], pair.Value.ApplicationForce, applicationColor);
                            Layout.Children.Add(line);
                            _forceLines.Add(line);
                        }

                        // Set the position of the gravity center
                        Animate(
                            origo,
                            Canvas.LeftProperty,
                            testIterationArgs.GravitationCenter.X - origo.Width / 2.0,
                            _animationDuration);
                        Animate(
                            origo,
                            Canvas.TopProperty,
                            testIterationArgs.GravitationCenter.Y - origo.Height / 2.0,
                            _animationDuration);
                        TxtMessage.Text = testIterationArgs.Message;
                    });

                    do
                    {
                        Thread.Sleep((int)_animationDuration.TimeSpan.TotalMilliseconds);
                    } while (_paused);
                };

                layoutAlgorithm.Compute();
            };

            worker.RunWorkerAsync();
        }

        [Pure]
        [NotNull, ItemNotNull]
        private static CompoundGraph<object, IEdge<object>>[] InitGraphs()
        {
            var graphs = new CompoundGraph<object, IEdge<object>>[6];

            #region Big graph

            var graph = new CompoundGraph<object, IEdge<object>>();

            string[] vertices = InitVertices(graph, 20);

            for (int i = 6; i < 15; ++i)
            {
                graph.AddChildVertex(vertices[i % 5], vertices[i]);
            }

            graph.AddChildVertex(vertices[5], vertices[4]);
            graph.AddChildVertex(vertices[5], vertices[2]);
            graph.AddChildVertex(vertices[16], vertices[0]);
            graph.AddChildVertex(vertices[16], vertices[1]);
            graph.AddChildVertex(vertices[16], vertices[3]);

            graph.AddEdge(new Edge<object>(vertices[0], vertices[1]));
            graph.AddEdge(new Edge<object>(vertices[0], vertices[2]));
            graph.AddEdge(new Edge<object>(vertices[2], vertices[4]));
            graph.AddEdge(new Edge<object>(vertices[0], vertices[7]));
            graph.AddEdge(new Edge<object>(vertices[8], vertices[7]));

            graphs[BigGraph] = graph;

            #endregion

            #region Small graph

            graph = new CompoundGraph<object, IEdge<object>>();
            vertices = InitVertices(graph, 10);

            // Add the containements
            graph.AddChildVertex(vertices[0], vertices[1]);
            graph.AddChildVertex(vertices[0], vertices[2]);
            graph.AddChildVertex(vertices[3], vertices[4]);
            graph.AddChildVertex(vertices[3], vertices[5]);
            graph.AddChildVertex(vertices[3], vertices[6]);
            graph.AddChildVertex(vertices[3], vertices[7]);
            graph.AddChildVertex(vertices[3], vertices[8]);
            graph.AddChildVertex(vertices[3], vertices[9]);

            // Add the edges
            graph.AddEdge(new Edge<object>(vertices[2], vertices[4]));
            graph.AddEdge(new Edge<object>(vertices[1], vertices[5]));
            //g.AddEdge(new Edge<object>(vertices[0], vertices[1]));

            graphs[SmallGraph] = graph;

            #endregion

            #region Flat graph

            graph = new CompoundGraph<object, IEdge<object>>();
            vertices = InitVertices(graph, 10);

            graph.AddEdge(new Edge<object>(vertices[0], vertices[1]));
            graph.AddEdge(new Edge<object>(vertices[1], vertices[2]));
            graph.AddEdge(new Edge<object>(vertices[2], vertices[3]));
            graph.AddEdge(new Edge<object>(vertices[3], vertices[0]));
            graph.AddEdge(new Edge<object>(vertices[1], vertices[3]));
            graph.AddEdge(new Edge<object>(vertices[2], vertices[0]));

            graphs[FlatGraph] = graph;

            #endregion

            #region Repulsion graph

            graph = new CompoundGraph<object, IEdge<object>>();
            InitVertices(graph, 50);

            graphs[RepGraph] = graph;

            #endregion

            #region Star

            graph = new CompoundGraph<object, IEdge<object>>();
            vertices = InitVertices(graph, 13);

            for (int i = 1; i < 13; ++i)
                graph.AddEdge(new Edge<object>(vertices[0], vertices[i]));

            for (int i = 0; i < 4; ++i)
            {
                graph.AddEdge(new Edge<object>(vertices[i * 3 + 1], vertices[i * 3 + 2]));
                graph.AddEdge(new Edge<object>(vertices[i * 3 + 1], vertices[i * 3 + 3]));
                graph.AddEdge(new Edge<object>(vertices[i * 3 + 2], vertices[i * 3 + 3]));
            }

            graphs[StarGraph] = graph;

            #endregion

            #region Combined

            graph = new CompoundGraph<object, IEdge<object>>();
            vertices = InitVertices(graph, 51);

            // Add the containements
            graph.AddChildVertex(vertices[0], vertices[1]);
            graph.AddChildVertex(vertices[0], vertices[2]);

            // Add the edges
            graph.AddEdge(new Edge<object>(vertices[2], vertices[3]));
            graph.AddEdge(new Edge<object>(vertices[3], vertices[4]));

            graph.AddEdge(new Edge<object>(vertices[10], vertices[11]));
            graph.AddEdge(new Edge<object>(vertices[11], vertices[12]));
            graph.AddEdge(new Edge<object>(vertices[12], vertices[13]));
            graph.AddEdge(new Edge<object>(vertices[13], vertices[10]));

            for (int i = 6; i < 15; ++i)
            {
                graph.AddChildVertex(vertices[i % 5 + 20], vertices[i + 20]);
            }

            graph.AddChildVertex(vertices[25], vertices[24]);
            graph.AddChildVertex(vertices[25], vertices[22]);
            graph.AddChildVertex(vertices[36], vertices[20]);
            graph.AddChildVertex(vertices[36], vertices[21]);
            graph.AddChildVertex(vertices[36], vertices[23]);

            graph.AddEdge(new Edge<object>(vertices[20], vertices[21]));
            graph.AddEdge(new Edge<object>(vertices[20], vertices[22]));
            graph.AddEdge(new Edge<object>(vertices[22], vertices[24]));
            graph.AddEdge(new Edge<object>(vertices[20], vertices[27]));
            graph.AddEdge(new Edge<object>(vertices[28], vertices[27]));

            graph.AddEdge(new Edge<object>(vertices[4], vertices[39]));
            graph.AddEdge(new Edge<object>(vertices[39], vertices[40]));
            graph.AddEdge(new Edge<object>(vertices[39], vertices[41]));
            graph.AddEdge(new Edge<object>(vertices[39], vertices[42]));
            graph.AddEdge(new Edge<object>(vertices[42], vertices[43]));
            graph.AddEdge(new Edge<object>(vertices[42], vertices[44]));

            graph.AddEdge(new Edge<object>(vertices[1], vertices[45]));
            graph.AddEdge(new Edge<object>(vertices[45], vertices[46]));
            graph.AddEdge(new Edge<object>(vertices[45], vertices[47]));
            graph.AddEdge(new Edge<object>(vertices[45], vertices[48]));
            graph.AddEdge(new Edge<object>(vertices[48], vertices[49]));
            graph.AddEdge(new Edge<object>(vertices[48], vertices[50]));

            graphs[CombinedGraph] = graph;

            #endregion

            return graphs;
        }

        private Duration _animationDuration = new Duration(TimeSpan.FromMilliseconds(100));

        private void AddLine([NotNull] object lineKey, Point pos1, Point pos2, bool b)
        {
            Debug.Assert(lineKey != null);

            Line line;
            if (_lines.ContainsKey(lineKey))
            {
                line = _lines[lineKey];
            }
            else
            {
                line = new Line();
                _lines[lineKey] = line;
                Layout.Children.Add(line);
                line.StrokeThickness = 2;
                line.Stroke = b ? System.Windows.Media.Brushes.Black : System.Windows.Media.Brushes.Silver;
            }

            Animate(line, Line.X1Property, pos1.X, _animationDuration);
            Animate(line, Line.Y1Property, pos1.Y, _animationDuration);
            Animate(line, Line.X2Property, pos2.X, _animationDuration);
            Animate(line, Line.Y2Property, pos2.Y, _animationDuration);
        }

        [NotNull, ItemNotNull]
        private static readonly Brush[] Brushes =
        {
            System.Windows.Media.Brushes.Red,
            System.Windows.Media.Brushes.Yellow,
            System.Windows.Media.Brushes.Green,
            System.Windows.Media.Brushes.BlueViolet,
            System.Windows.Media.Brushes.Violet,
            System.Windows.Media.Brushes.Teal,
            System.Windows.Media.Brushes.Blue
        };

        private static int _brushIndex;

        private void AddRectangle([NotNull] object rectKey, Point point, Size size)
        {
            Debug.Assert(rectKey != null);

            point.X -= size.Width / 2;
            point.Y -= size.Height / 2;
            Rectangle rect;
            if (_rectangles.ContainsKey(rectKey))
            {
                rect = _rectangles[rectKey];
            }
            else
            {
                rect = new Rectangle();
                _rectangles[rectKey] = rect;
                Layout.Children.Add(rect);
                rect.Fill = Brushes[_brushIndex];
                rect.Stroke = System.Windows.Media.Brushes.Black;
                rect.StrokeThickness = 1;
                _brushIndex = (_brushIndex + 1) % Brushes.Length;
                rect.Opacity = 0.7;
            }

            Animate(rect, WidthProperty, size.Width, _animationDuration);
            Animate(rect, HeightProperty, size.Height, _animationDuration);
            Animate(rect, Canvas.LeftProperty, point.X, _animationDuration);
            Animate(rect, Canvas.TopProperty, point.Y, _animationDuration);
        }

        private static void Animate(
            [NotNull] UIElement obj,
            [NotNull] DependencyProperty property,
            double toValue,
            Duration duration)
        {
            Debug.Assert(obj != null);
            Debug.Assert(property != null);

            double fromValue = (double)obj.GetValue(property);
            if (double.IsNaN(fromValue))
                fromValue = 0;

            var animation = new DoubleAnimation(fromValue, toValue, duration, FillBehavior.HoldEnd)
            {
                AccelerationRatio = 0.3,
                DecelerationRatio = 0.3
            };

            obj.BeginAnimation(property, animation);
        }

        private void OnPauseClick(object sender, RoutedEventArgs args)
        {
            _paused = !_paused;
        }

        private void OnRelayoutClick(object sender, RoutedEventArgs args)
        {
            ShowGraph(GraphComboBox.SelectedIndex);
        }
    }
}
