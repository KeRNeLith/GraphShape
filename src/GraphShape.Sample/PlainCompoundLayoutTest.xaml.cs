using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphSharp.Algorithms.Layout;
using QuickGraph;
using System.ComponentModel;
using System.Threading;
using System.Windows.Media.Animation;
using GraphSharp.Algorithms.Layout.Compound.FDP;
using GraphSharp.Algorithms.Layout.Compound;

namespace GraphSharp.Sample
{
    /// <summary>
    /// Interaction logic for PlainCompoundLayoutTest.xaml
    /// </summary>
    public partial class PlainCompoundLayoutTest
    {
        private static readonly int BIG_GRAPH = 0;
        private static readonly int SMALL_GRAPH = 1;
        private static readonly int FLAT_GRAPH = 2;
        private static readonly int REP_GRAPH = 3;
        private static readonly int STAR_GRAPH = 4;
        private static readonly int COMBINED_GRAPH = 5;

        private int inspectedGraph = STAR_GRAPH;
        private bool _paused = false;

        private CompoundGraph<object, IEdge<object>>[] graphs;
        private CompoundFDPLayoutParameters parameters = new CompoundFDPLayoutParameters();

        public PlainCompoundLayoutTest()
        {
            InitializeComponent();
            InitGraphs();
            //ShowGraph(inspectedGraph);
            DataContext = parameters;
        }

        private void Relayout_Click(object sender, RoutedEventArgs e)
        {
            ShowGraph(cbGraph.SelectedIndex);
        }

        private readonly IDictionary<object, Rectangle> _rectDict = new Dictionary<object, Rectangle>();
        private readonly IDictionary<object, Line> _lineDict = new Dictionary<object, Line>();
        private readonly IList<Line> _forceLines = new List<Line>();

        private void ShowGraph(int graphIndex)
        {
            CompoundGraph<object, IEdge<object>> g = graphs[graphIndex];
            _rectDict.Clear();
            _lineDict.Clear();
            lc.Children.Clear();
            var origo = new Ellipse();
            origo.Width = 100;
            origo.Height = 100;
            origo.Fill = Brushes.Black;
            origo.OpacityMask = new RadialGradientBrush(Colors.Black, Colors.Transparent);
            Canvas.SetLeft(origo, -5);
            Canvas.SetTop(origo, -5);
            lc.Children.Add(origo);
            var sizes = new Dictionary<object, Size>();
            var borders = new Dictionary<object, Thickness>();
            var layoutType = new Dictionary<object, CompoundVertexInnerLayoutType>();

            var s = new Size(20, 20);
            foreach (var v in g.SimpleVertices)
            {
                sizes[v] = s;
            }

            var b = new Thickness(5, 10, 5, 5);
            foreach (var v in g.CompoundVertices)
            {
                sizes[v] = new Size();
                borders[v] = b;
                layoutType[v] = CompoundVertexInnerLayoutType.Automatic;
            }

            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sender, e) =>
                                 {
                                     var layoutAlgorithm =
                                         new CompoundFDPLayoutAlgorithm
                                             <object, IEdge<object>, ICompoundGraph<object, IEdge<object>>>(
                                             g, sizes, borders, layoutType, null,
                                             parameters);
                                     layoutAlgorithm.IterationEnded += (o, evt) =>
                                     {
                                         var args = evt as TestingCompoundLayoutIterationEventArgs<object, IEdge<object>, TestingCompoundVertexInfo, object>;
                                         var positions = args.VertexPositions;
                                         var innerSizes = (args as ICompoundLayoutIterationEventArgs<object>).InnerCanvasSizes;
                                         var vertexInfos = args.VertexInfos;

                                         Dispatcher.Invoke(new Action(delegate
                                                                          {
                                                                              var pDict = new Dictionary<object, Point>();

                                                                              var compoundVerticesToCheck =
                                                                                  new Queue<object>();
                                                                              var compoundVertices = new List<object>();
                                                                              var root =
                                                                                  g.CompoundVertices.Where(
                                                                                      cv => g.GetParent(cv) == null);
                                                                              foreach (var r in root)
                                                                                  compoundVerticesToCheck.Enqueue(r);
                                                                              while (compoundVerticesToCheck.Count > 0)
                                                                              {
                                                                                  var next = compoundVerticesToCheck.Dequeue();
                                                                                  if (!g.IsCompoundVertex(next))
                                                                                      continue;
                                                                                  compoundVertices.Add(next);
                                                                                  foreach (
                                                                                      var childrenVertex in
                                                                                          g.GetChildrenVertices(next))
                                                                                  {
                                                                                      compoundVerticesToCheck.Enqueue(
                                                                                          childrenVertex);
                                                                                  }
                                                                              }

                                                                              //draw the compound vertices
                                                                              foreach (var v in compoundVertices)
                                                                              {
                                                                                  var size = innerSizes[v];
                                                                                  size.Width += b.Left + b.Right;
                                                                                  size.Height += b.Top + b.Bottom;

                                                                                  var pos = positions[v];
                                                                                  pDict[v] = pos;
                                                                                  AddRect(v, pos, size);
                                                                              }

                                                                              //draw the simple vertices
                                                                              foreach (var v in g.SimpleVertices)
                                                                              {
                                                                                  var pos = positions[v];
                                                                                  pDict[v] = pos;
                                                                                  AddRect(v, pos, s);
                                                                              }

                                                                              //draw the simple edges
                                                                              foreach (var edge in g.Edges)
                                                                              {
                                                                                  var pos1 = pDict[edge.Source];
                                                                                  var pos2 = pDict[edge.Target];
                                                                                  AddLine(edge, pos1, pos2, true);
                                                                              }

                                                                              //draw the containment edges
                                                                              /*foreach (var v in g.CompoundVertices)
                                                                              {
                                                                                  var pos1 = pDict[v];
                                                                                  foreach (var c in g.GetChildrenVertices(v))
                                                                                  {
                                                                                      var pos2 = pDict[c];
                                                                                      AddLine(c, pos1, pos2, false);
                                                                                  }
                                                                              }*/

                                                                              //draw the lines of the forces
                                                                              foreach (var forceLine in _forceLines)
                                                                                  lc.Children.Remove(forceLine);

                                                                              var springColor = Brushes.Orange;
                                                                              var repulsionColor = Brushes.Red;
                                                                              var gravityColor = Brushes.Green;
                                                                              var applicationColor = Brushes.Yellow;

                                                                              foreach (var kvp in vertexInfos)
                                                                              {
                                                                                  var line = CreateLine(pDict[kvp.Key],
                                                                                                        kvp.Value.SpringForce,
                                                                                                        springColor);
                                                                                  lc.Children.Add(line);
                                                                                  _forceLines.Add(line);
                                                                                  line = CreateLine(pDict[kvp.Key],
                                                                                             kvp.Value.RepulsionForce,
                                                                                             repulsionColor);
                                                                                  lc.Children.Add(line);
                                                                                  _forceLines.Add(line);
                                                                                  line = CreateLine(pDict[kvp.Key],
                                                                                             kvp.Value.GravityForce,
                                                                                             gravityColor);
                                                                                  lc.Children.Add(line);
                                                                                  _forceLines.Add(line);
                                                                                  line = CreateLine(pDict[kvp.Key],
                                                                                             kvp.Value.ApplicationForce,
                                                                                             applicationColor);
                                                                                  lc.Children.Add(line);
                                                                                  _forceLines.Add(line);
                                                                              }


                                                                              //set the position of the gravity center
                                                                              Animate(origo, Canvas.LeftProperty, args.GravitationCenter.X - origo.Width / 2.0, animationDuration);
                                                                              Animate(origo, Canvas.TopProperty, args.GravitationCenter.Y - origo.Height / 2.0, animationDuration);
                                                                              txtMessage.Text = args.Message;
                                                                          }));
                                         do
                                         {
                                             Thread.Sleep((int) animationDuration.TimeSpan.TotalMilliseconds);
                                         } while (_paused);
                                     };
                                     layoutAlgorithm.Compute();
                                 };
            worker.RunWorkerAsync();
        }

        private static Line CreateLine(Point startPoint, Vector vector, Brush color)
        {
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

        private void InitGraphs()
        {
            graphs = new CompoundGraph<object, IEdge<object>>[10];

            #region Big graph
            var g = new CompoundGraph<object, IEdge<object>>();

            string[] vertices = InitVertices(g, 20);

            for (int i = 6; i < 15; i++)
            {
                g.AddChildVertex(vertices[i % 5], vertices[i]);
            }
            g.AddChildVertex(vertices[5], vertices[4]);
            g.AddChildVertex(vertices[5], vertices[2]);
            g.AddChildVertex(vertices[16], vertices[0]);
            g.AddChildVertex(vertices[16], vertices[1]);
            g.AddChildVertex(vertices[16], vertices[3]);

            g.AddEdge(new Edge<object>(vertices[0], vertices[1]));
            g.AddEdge(new Edge<object>(vertices[0], vertices[2]));
            g.AddEdge(new Edge<object>(vertices[2], vertices[4]));
            g.AddEdge(new Edge<object>(vertices[0], vertices[7]));
            g.AddEdge(new Edge<object>(vertices[8], vertices[7]));
            graphs[BIG_GRAPH] = g;
            #endregion

            #region Small graph

            g = new CompoundGraph<object, IEdge<object>>();
            vertices = InitVertices(g, 10);

            //add the containments
            g.AddChildVertex(vertices[0], vertices[1]);
            g.AddChildVertex(vertices[0], vertices[2]);
            g.AddChildVertex(vertices[3], vertices[4]);
            g.AddChildVertex(vertices[3], vertices[5]);
            g.AddChildVertex(vertices[3], vertices[6]);
            g.AddChildVertex(vertices[3], vertices[7]);
            g.AddChildVertex(vertices[3], vertices[8]);
            g.AddChildVertex(vertices[3], vertices[9]);

            //add the edges
            g.AddEdge(new Edge<object>(vertices[2], vertices[4]));
            g.AddEdge(new Edge<object>(vertices[1], vertices[5]));
            //g.AddEdge(new Edge<object>(vertices[0], vertices[1]));

            graphs[SMALL_GRAPH] = g;
            #endregion

            #region Flat graph

            g = new CompoundGraph<object, IEdge<object>>();
            vertices = InitVertices(g, 10);

            g.AddEdge(new Edge<object>(vertices[0], vertices[1]));
            g.AddEdge(new Edge<object>(vertices[1], vertices[2]));
            g.AddEdge(new Edge<object>(vertices[2], vertices[3]));
            g.AddEdge(new Edge<object>(vertices[3], vertices[0]));
            g.AddEdge(new Edge<object>(vertices[1], vertices[3]));
            g.AddEdge(new Edge<object>(vertices[2], vertices[0]));

            graphs[FLAT_GRAPH] = g;
            #endregion

            #region Repulsion graph
            g = new CompoundGraph<object, IEdge<object>>();
            vertices = InitVertices(g, 50);

            graphs[REP_GRAPH] = g;
            #endregion

            #region Star

            g = new CompoundGraph<object, IEdge<object>>();
            vertices = InitVertices(g, 13);

            for (int i = 1; i < 13; i++ )
                g.AddEdge(new Edge<object>(vertices[0], vertices[i]));

            for (int i = 0; i < 4; i++)
            {
                g.AddEdge(new Edge<object>(vertices[i * 3 + 1], vertices[i * 3 + 2]));
                g.AddEdge(new Edge<object>(vertices[i * 3 + 1], vertices[i * 3 + 3]));
                g.AddEdge(new Edge<object>(vertices[i * 3 + 2], vertices[i * 3 + 3]));
            }
            graphs[STAR_GRAPH] = g;
            #endregion

            #region Combined

            g = new CompoundGraph<object, IEdge<object>>();
            vertices = InitVertices(g, 51);

            //add the containments
            g.AddChildVertex(vertices[0], vertices[1]);
            g.AddChildVertex(vertices[0], vertices[2]);

            //add the edges
            g.AddEdge(new Edge<object>(vertices[2], vertices[3]));
            g.AddEdge(new Edge<object>(vertices[3], vertices[4]));

            g.AddEdge(new Edge<object>(vertices[10], vertices[11]));
            g.AddEdge(new Edge<object>(vertices[11], vertices[12]));
            g.AddEdge(new Edge<object>(vertices[12], vertices[13]));
            g.AddEdge(new Edge<object>(vertices[13], vertices[10]));

            for (int i = 6; i < 15; i++)
            {
                g.AddChildVertex(vertices[i % 5 + 20], vertices[i + 20]);
            }
            g.AddChildVertex(vertices[25], vertices[24]);
            g.AddChildVertex(vertices[25], vertices[22]);
            g.AddChildVertex(vertices[36], vertices[20]);
            g.AddChildVertex(vertices[36], vertices[21]);
            g.AddChildVertex(vertices[36], vertices[23]);

            g.AddEdge(new Edge<object>(vertices[20], vertices[21]));
            g.AddEdge(new Edge<object>(vertices[20], vertices[22]));
            g.AddEdge(new Edge<object>(vertices[22], vertices[24]));
            g.AddEdge(new Edge<object>(vertices[20], vertices[27]));
            g.AddEdge(new Edge<object>(vertices[28], vertices[27]));
            
            g.AddEdge(new Edge<object>(vertices[4], vertices[39]));
            g.AddEdge(new Edge<object>(vertices[39], vertices[40]));
            g.AddEdge(new Edge<object>(vertices[39], vertices[41]));
            g.AddEdge(new Edge<object>(vertices[39], vertices[42]));
            g.AddEdge(new Edge<object>(vertices[42], vertices[43]));
            g.AddEdge(new Edge<object>(vertices[42], vertices[44]));

            g.AddEdge(new Edge<object>(vertices[1], vertices[45]));
            g.AddEdge(new Edge<object>(vertices[45], vertices[46]));
            g.AddEdge(new Edge<object>(vertices[45], vertices[47]));
            g.AddEdge(new Edge<object>(vertices[45], vertices[48]));
            g.AddEdge(new Edge<object>(vertices[48], vertices[49]));
            g.AddEdge(new Edge<object>(vertices[48], vertices[50]));

            graphs[COMBINED_GRAPH] = g;
            #endregion

        }

        private string[] InitVertices(CompoundGraph<object, IEdge<object>> g, int vertexCount)
        {
            var vertices = new string[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                vertices[i] = i.ToString();
                g.AddVertex(vertices[i]);
            }
            return vertices;
        }

        private Duration animationDuration = new Duration(TimeSpan.FromMilliseconds(100));

        private void AddLine(object lineKey, Point pos1, Point pos2, bool b)
        {
            Line line = null;
            if (_lineDict.ContainsKey(lineKey))
                line = _lineDict[lineKey];
            else
            {
                line = new Line();
                _lineDict[lineKey] = line;
                lc.Children.Add(line);
                line.StrokeThickness = 2;
                if (b)
                    line.Stroke = Brushes.Black;
                else line.Stroke = Brushes.Silver;
            }
            Animate(line, Line.X1Property, pos1.X, animationDuration);
            Animate(line, Line.Y1Property, pos1.Y, animationDuration);
            Animate(line, Line.X2Property, pos2.X, animationDuration);
            Animate(line, Line.Y2Property, pos2.Y, animationDuration);
        }

        private static Brush[] brushes = new Brush[] { Brushes.Red, Brushes.Yellow, Brushes.Green, Brushes.BlueViolet, Brushes.Violet, Brushes.Teal, Brushes.Blue };
        private static int bIndex = 0;

        private void AddRect(object rectKey, Point point, Size size)
        {
            point.X -= size.Width / 2;
            point.Y -= size.Height / 2;
            Rectangle rect = null;
            if (_rectDict.ContainsKey(rectKey))
                rect = _rectDict[rectKey];
            else
            {
                rect = new Rectangle();
                _rectDict[rectKey] = rect;
                lc.Children.Add(rect);
                rect.Fill = brushes[bIndex];
                rect.Stroke = Brushes.Black;
                rect.StrokeThickness = 1;
                bIndex = (bIndex + 1) % brushes.Length;
                rect.Opacity = 0.7;
            }
            Animate(rect, FrameworkElement.WidthProperty, size.Width, animationDuration);
            Animate(rect, FrameworkElement.HeightProperty, size.Height, animationDuration);
            Animate(rect, Canvas.LeftProperty, point.X, animationDuration);
            Animate(rect, Canvas.TopProperty, point.Y, animationDuration);
        }

        private void Animate(FrameworkElement obj, DependencyProperty property, double toValue, Duration duration)
        {
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

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            this._paused = !this._paused;
        }
    }
}
