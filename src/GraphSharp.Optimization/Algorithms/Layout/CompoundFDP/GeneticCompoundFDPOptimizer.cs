using System;
using System.Collections.Generic;
using System.Windows;
using GraphSharp.Algorithms.Layout;
using GraphSharp.Optimization.GeneticAlgorithm;
using QuickGraph;
using GraphSharp.Algorithms;
using GraphSharp.Algorithms.Layout.Compound.FDP;
using GraphSharp.Algorithms.Layout.Compound;

namespace GraphSharp.Optimization.Algorithms.Layout.CompoundFDP
{
    public class GeneticCompoundFDPOptimizer : GeneticAlgorithmBase<CompoundFDPLayoutParameters, GeneticAlgorithmParameters>
    {
        private CompoundGraph<object, IEdge<object>>[] graphs;
        private static readonly int BIG_GRAPH = 0;
        private static readonly int SMALL_GRAPH = 1;
        private static readonly int FLAT_GRAPH = 2;
        private static readonly int REP_GRAPH = 3;
        private static readonly int STAR_GRAPH = 4;
        private static readonly int COMBINED_GRAPH = 5;

        public GeneticCompoundFDPOptimizer(GeneticAlgorithmParameters parameters)
            : base(parameters)
        {
            InitGraphs();
        }

        private void InitGraphs()
        {
            graphs = new CompoundGraph<object, IEdge<object>>[6];

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
            vertices = InitVertices(g, 2);

            //add the containments
            /*g.AddChildVertex(vertices[0], vertices[1]);
            g.AddChildVertex(vertices[0], vertices[2]);*/

            //add the edges
            /*g.AddEdge(new Edge<object>(vertices[2], vertices[3]));
            g.AddEdge(new Edge<object>(vertices[3], vertices[4]));*/

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
            vertices = InitVertices(g, 20);

            graphs[REP_GRAPH] = g;
            #endregion

            #region Star

            g = new CompoundGraph<object, IEdge<object>>();
            vertices = InitVertices(g, 13);

            for (int i = 1; i < 13; i++)
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
            vertices = InitVertices(g, 50);

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

        protected override CompoundFDPLayoutParameters Mutate(CompoundFDPLayoutParameters chromosome)
        {
            bool mutate = rnd.NextDouble() < Parameters.MutationRate;
            bool lowering = rnd.NextDouble() < 0.5;
            if (mutate)
                chromosome.ElasticConstant *= lowering ? rnd.NextDouble() : Math.Max(1.0, rnd.NextDouble() * 3);

            mutate = rnd.NextDouble() < Parameters.MutationRate;
            lowering = rnd.NextDouble() < 0.5;
            if (mutate)
                chromosome.GravitationFactor *= lowering ? rnd.NextDouble() : Math.Max(1.0, rnd.NextDouble() * 3);

            mutate = rnd.NextDouble() < Parameters.MutationRate;
            lowering = rnd.NextDouble() < 0.5;
            if (mutate)
                chromosome.NestingFactor *= lowering ? rnd.NextDouble() : Math.Max(1.0, rnd.NextDouble() * 3);

            mutate = rnd.NextDouble() < Parameters.MutationRate;
            lowering = rnd.NextDouble() < 0.5;
            if (mutate)
                chromosome.RepulsionConstant *= lowering ? rnd.NextDouble() : Math.Max(1.0, rnd.NextDouble() * 3);

            mutate = rnd.NextDouble() < Parameters.MutationRate;
            if (mutate)
                chromosome.Phase1Iterations = rnd.Next(190) + 10;

            mutate = rnd.NextDouble() < Parameters.MutationRate;
            if (mutate)
                chromosome.Phase2Iterations = rnd.Next(190) + 10;

            mutate = rnd.NextDouble() < Parameters.MutationRate;
            if (mutate)
                chromosome.Phase3Iterations = rnd.Next(190) + 10;

            mutate = rnd.NextDouble() < Parameters.MutationRate;
            lowering = rnd.NextDouble() < 0.5;
            if (mutate)
                chromosome.DisplacementLimitMultiplier *= lowering ? rnd.NextDouble() : Math.Max(1.0, rnd.NextDouble() * 3);

            mutate = rnd.NextDouble() < Parameters.MutationRate;
            lowering = rnd.NextDouble() < 0.5;
            if (mutate)
                chromosome.SeparationMultiplier *= lowering ? rnd.NextDouble() : Math.Max(1.0, rnd.NextDouble() * 3);

            mutate = rnd.NextDouble() < Parameters.MutationRate;
            lowering = rnd.NextDouble() < 0.5;
            if (mutate)
                chromosome.TemperatureDecreasing *= lowering ? rnd.NextDouble() : Math.Max(1.0, rnd.NextDouble() * 2);

            mutate = rnd.NextDouble() < Parameters.MutationRate;
            lowering = rnd.NextDouble() < 0.5;
            if (mutate)
                chromosome.TemperatureFactor *= lowering ? rnd.NextDouble() : Math.Max(1.0, rnd.NextDouble() * 2);

            return CoalesceParameters(chromosome);
        }

        protected override CompoundFDPLayoutParameters Crossover(CompoundFDPLayoutParameters parent1, CompoundFDPLayoutParameters parent2)
        {
            var crossoverChance = rnd.NextDouble();
            var offspring = new CompoundFDPLayoutParameters();
            var crossoverIndex = rnd.Next(12) + 1;
            if (crossoverChance < Parameters.CrossoverRate)
            {
                offspring.ElasticConstant = crossoverIndex >= 1 ? parent1.ElasticConstant : parent2.ElasticConstant;
                offspring.GravitationFactor = crossoverIndex >= 2 ? parent1.GravitationFactor : parent2.GravitationFactor;
                offspring.NestingFactor = crossoverIndex >= 3 ? parent1.NestingFactor : parent2.NestingFactor;
                offspring.RepulsionConstant = crossoverIndex >= 4 ? parent1.RepulsionConstant : parent2.RepulsionConstant;
                offspring.Phase1Iterations = crossoverIndex >= 5 ? parent1.Phase1Iterations : parent2.Phase1Iterations;
                offspring.Phase2Iterations = crossoverIndex >= 6 ? parent1.Phase2Iterations : parent2.Phase2Iterations;
                offspring.Phase3Iterations = crossoverIndex >= 7 ? parent1.Phase3Iterations : parent2.Phase3Iterations;
                offspring.DisplacementLimitMultiplier = crossoverIndex >= 8 ? parent1.DisplacementLimitMultiplier : parent2.DisplacementLimitMultiplier;
                offspring.SeparationMultiplier = crossoverIndex >= 9 ? parent1.SeparationMultiplier : parent2.SeparationMultiplier;
                offspring.TemperatureDecreasing = crossoverIndex >= 10 ? parent1.TemperatureDecreasing : parent2.TemperatureDecreasing;
                offspring.TemperatureFactor = crossoverIndex >= 11 ? parent1.TemperatureFactor : parent2.TemperatureFactor;


                offspring = CoalesceParameters(offspring);
            }
            return offspring;
        }

        private CompoundFDPLayoutParameters CoalesceParameters(CompoundFDPLayoutParameters offspring)
        {
            offspring.ElasticConstant = Math.Max(0, offspring.ElasticConstant);
            offspring.GravitationFactor = Math.Max(0, offspring.GravitationFactor);
            offspring.NestingFactor = Math.Min(1.0, Math.Max(0.0, offspring.NestingFactor));
            offspring.RepulsionConstant = Math.Max(0, offspring.RepulsionConstant);
            offspring.Phase1Iterations = Math.Min(200, Math.Max(10, offspring.Phase1Iterations));
            offspring.Phase2Iterations = Math.Min(200, Math.Max(10, offspring.Phase2Iterations));
            offspring.Phase3Iterations = Math.Min(200, Math.Max(10, offspring.Phase3Iterations));
            offspring.DisplacementLimitMultiplier = Math.Max(0, offspring.DisplacementLimitMultiplier);
            offspring.SeparationMultiplier = Math.Max(0, offspring.SeparationMultiplier);
            offspring.TemperatureDecreasing = Math.Min(1.0, Math.Max(0.0, offspring.TemperatureDecreasing));
            offspring.TemperatureFactor = Math.Min(0.999, Math.Max(0.1, offspring.TemperatureFactor));

            return offspring;
        }

        protected override double EvaluateFitness(CompoundFDPLayoutParameters individual)
        {
            double fitness = 0.0;
            //Console.WriteLine("Evaluating Fitness");
            for (int c = 0; c < 2; c++)
            {
                //Console.WriteLine("EF:Round {0}", c);
                for (int i = 0; i < graphs.Length; i++)
                {
                    if (i != BIG_GRAPH /*&& i!= SMALL_GRAPH && i!= STAR_GRAPH*/)
                        continue;
                    double f = EvaluateFitnessForGraph(individual, graphs[i]);
                    //Console.WriteLine("EF:Round {0}:Graph{1}", c, i);
                    if (double.IsNaN(f))
                        return double.PositiveInfinity;
                    fitness += f;
                }
            }

            //edge crossing
            //edge length
            //node overlap
            //size of canvas
            //canvas ratio
            return fitness;
        }

        private const double NODE_OVERLAP_MULTIPLIER = 1000000000000;
        private const double NODE_DISTANCE_MULTIPLIER = 50000;
        private const double CANVAS_SIZE_MULTIPLIER = 1000;
        private const double CANVAS_RATIO_MULTIPLIER = 3000;
        private const double EDGE_LENGTH_MULTIPLIER = 5000000;
        private const double EDGE_CROSSING_MULTIPLIER = 1000000000;
        private const double PHASE_LENGTH_MULTIPLIER = 30000;

        private double EvaluateFitnessForGraph(CompoundFDPLayoutParameters chromosome, CompoundGraph<object, IEdge<object>> compoundGraph)
        {
            var sizes = new Dictionary<object, Size>();
            var borders = new Dictionary<object, Thickness>();
            var layoutType = new Dictionary<object, CompoundVertexInnerLayoutType>();

            var s = new Size(20, 20);
            foreach (var v in compoundGraph.SimpleVertices)
            {
                sizes[v] = s;
            }

            var b = new Thickness(5, 10, 5, 5);
            foreach (var v in compoundGraph.CompoundVertices)
            {
                sizes[v] = new Size();
                borders[v] = b;
                layoutType[v] = CompoundVertexInnerLayoutType.Automatic;
            }

            //run the compound FDP algorithm
            var algorithm =
                new CompoundFDPLayoutAlgorithm<object, IEdge<object>, CompoundGraph<object, IEdge<object>>>(
                    compoundGraph, sizes, borders, layoutType, null, chromosome);
            algorithm.Compute();

            double fitness = 0.0;

            //refresh the sizes of the compound vertices
            foreach (var v in compoundGraph.CompoundVertices)
            {
                var border = borders[v];
                var innerCanvasSize = algorithm.InnerCanvasSizes[v];
                var size = new Size(
                    border.Left + innerCanvasSize.Width + border.Right,
                    border.Top + innerCanvasSize.Height + border.Bottom
                    );
                sizes[v] = size;
            }

            //NODE OVERLAP
            double overlaps = EvaluateNodeOverlaps(compoundGraph, algorithm, sizes);
            /*if (overlaps > 0.0)
                return double.NaN;*/
            fitness += overlaps * NODE_OVERLAP_MULTIPLIER;

            //CANVAS SIZE
            Size canvasSize = EvaluateCanvasSize(compoundGraph, algorithm, sizes);
            fitness += canvasSize.Width * canvasSize.Height * CANVAS_SIZE_MULTIPLIER;

            //CANVAS RATIO
            double canvasRatio = canvasSize.Width / canvasSize.Height;
            canvasRatio = canvasRatio < 1 ? 1 / canvasRatio : canvasRatio;
            fitness += canvasRatio * CANVAS_RATIO_MULTIPLIER;

            //NODE DISTANCES
            double minimalMinDistance, averageMinDistance, maximalMinDistance;
            double idealDistance = chromosome.IdealEdgeLength;
            EvaluateNodeDistances(compoundGraph, algorithm, sizes, out minimalMinDistance, out averageMinDistance, out maximalMinDistance);
            fitness += (averageMinDistance - idealDistance) * NODE_DISTANCE_MULTIPLIER;
            fitness += (minimalMinDistance - idealDistance) * NODE_DISTANCE_MULTIPLIER;
            fitness += (maximalMinDistance - idealDistance) * NODE_DISTANCE_MULTIPLIER;

            //EDGE LENGTH
            double edgeLength = EvaluateEdgeLength(compoundGraph, algorithm, sizes, chromosome.IdealEdgeLength, chromosome.NestingFactor);
            fitness += edgeLength * EDGE_LENGTH_MULTIPLIER;

            //EDGE CROSSING
            double edgeCrossing = EvaluateEdgeCrossing(compoundGraph, algorithm, sizes);
            fitness += edgeCrossing * EDGE_CROSSING_MULTIPLIER;

            //PHASE_LENGTH
            double phaseLength = (chromosome.Phase1Iterations + chromosome.Phase2Iterations +
                                  chromosome.Phase3Iterations)*PHASE_LENGTH_MULTIPLIER;
            fitness += phaseLength;

            return fitness;
        }

        private int EvaluateEdgeCrossing(
            CompoundGraph<object, IEdge<object>> compoundGraph,
            CompoundFDPLayoutAlgorithm<object, IEdge<object>, CompoundGraph<object, IEdge<object>>> algorithm,
            Dictionary<object, Size> sizes)
        {
            int crossings = 0;
            foreach (var edge in compoundGraph.Edges)
            {
                var uPos1 = algorithm.VertexPositions[edge.Source];
                var vPos1 = algorithm.VertexPositions[edge.Target];
                var uSize1 = sizes[edge.Source];
                var vSize1 = sizes[edge.Target];

                var uPoint1 = LayoutUtil.GetClippingPoint(uSize1, uPos1, vPos1);
                var vPoint1 = LayoutUtil.GetClippingPoint(vSize1, vPos1, uPos1);
                foreach (var edge2 in compoundGraph.Edges)
                {
                    if (edge == edge2)
                        continue;

                    var uPos2 = algorithm.VertexPositions[edge.Source];
                    var vPos2 = algorithm.VertexPositions[edge.Target];
                    var uSize2 = sizes[edge.Source];
                    var vSize2 = sizes[edge.Target];

                    var uPoint2 = LayoutUtil.GetClippingPoint(uSize2, uPos2, vPos2);
                    var vPoint2 = LayoutUtil.GetClippingPoint(vSize2, vPos2, uPos2);

                    Vector v1 = (vPoint1 - uPoint1);
                    Vector v2 = (vPoint2 - uPoint2);

                    if (v1 == v2 || v1 == -v2)
                        continue; //parallel edges

                    var t2 = (uPoint1.Y - uPoint2.Y + (uPoint2.X - uPoint1.X) * v1.Y / v1.X) / (v2.Y - v2.X * v1.Y / v1.X);
                    var t1 = (uPoint2.X - uPoint1.X + t2 * v2.X) / v1.X;

                    var p = uPoint1 + t1 * v1;
                    var b1 = t1 > 0 && (p - uPoint1).Length < (vPoint1 - uPoint1).Length;
                    var b2 = t2 > 0 && (p - uPoint2).Length < (vPoint2 - uPoint2).Length;

                    if (b1 && b2)
                        crossings++;
                }
            }
            return crossings;
        }

        private double EvaluateEdgeLength(
            CompoundGraph<object, IEdge<object>> compoundGraph,
            CompoundFDPLayoutAlgorithm<object, IEdge<object>, CompoundGraph<object, IEdge<object>>> algorithm,
            Dictionary<object, Size> sizes,
            double idealEdgeLength,
            double nestingFactor)
        {
            double edgeLengthError = 0.0;
            foreach (var edge in compoundGraph.Edges)
            {
                var uPos = algorithm.VertexPositions[edge.Source];
                var vPos = algorithm.VertexPositions[edge.Target];
                var uSize = sizes[edge.Source];
                var vSize = sizes[edge.Target];

                var uPoint = LayoutUtil.GetClippingPoint(uSize, uPos, vPos);
                var vPoint = LayoutUtil.GetClippingPoint(vSize, vPos, uPos);

                double length = (uPoint - vPoint).Length;
                bool isInterEdge = compoundGraph.GetParent(edge.Source) != compoundGraph.GetParent(edge.Target);
                var iel = isInterEdge
                              ? idealEdgeLength *
                                (algorithm.LevelOfVertex(edge.Source) + algorithm.LevelOfVertex(edge.Target) + 1) *
                                nestingFactor
                              : idealEdgeLength;
                double err = Math.Pow(length - iel, 2);
                edgeLengthError += err;
            }
            return edgeLengthError;
        }

        private double EvaluateNodeOverlaps(
            CompoundGraph<object, IEdge<object>> compoundGraph,
            CompoundFDPLayoutAlgorithm<object, IEdge<object>, CompoundGraph<object, IEdge<object>>> algorithm,
            IDictionary<object, Size> vertexSizes)
        {
            double overlapArea = 0.0;
            foreach (var level in algorithm.Levels)
            {
                foreach (var u in level)
                {
                    foreach (var v in level)
                    {
                        if (u == v || compoundGraph.GetParent(u) != compoundGraph.GetParent(v))
                            continue;

                        var uSize = vertexSizes[u];
                        var vSize = vertexSizes[v];
                        var uPosition = algorithm.VertexPositions[u];
                        var vPosition = algorithm.VertexPositions[v];

                        var uRect = new Rect(uPosition, uSize);
                        var vRect = new Rect(vPosition, vSize);

                        //get the overlap size
                        uRect.Intersect(vRect);
                        if (double.IsNegativeInfinity(uRect.Width) || double.IsNegativeInfinity(uRect.Height))
                            continue;

                        overlapArea += uRect.Width * uRect.Height;
                    }
                }
            }
            return overlapArea;
        }


        private Size EvaluateCanvasSize(
            CompoundGraph<object, IEdge<object>> compoundGraph,
            CompoundFDPLayoutAlgorithm<object, IEdge<object>, CompoundGraph<object, IEdge<object>>> algorithm,
            Dictionary<object, Size> sizes)
        {
            Point topLeft = new Point(double.PositiveInfinity, double.PositiveInfinity);
            Point bottomRight = new Point(double.NegativeInfinity, double.NegativeInfinity);
            foreach (var v in compoundGraph.Vertices)
            {
                var pos = algorithm.VertexPositions[v];
                var size = sizes[v];

                topLeft.X = Math.Min(topLeft.X, pos.X - size.Width / 2.0);
                topLeft.Y = Math.Min(topLeft.Y, pos.Y - size.Height / 2.0);

                bottomRight.X = Math.Max(bottomRight.X, pos.X + size.Width / 2.0);
                bottomRight.Y = Math.Max(bottomRight.Y, pos.Y + size.Height / 2.0);
            }
            var sizeVector = bottomRight - topLeft;
            return new Size(sizeVector.X, sizeVector.Y);
        }


        private void EvaluateNodeDistances(
            CompoundGraph<object, IEdge<object>> compoundGraph,
            CompoundFDPLayoutAlgorithm<object, IEdge<object>, CompoundGraph<object, IEdge<object>>> algorithm,
            Dictionary<object, Size> sizes,
            out double minimalMinDistance,
            out double averageMinDistance,
            out double maximalMinDistance)
        {
            minimalMinDistance = 0.0;
            averageMinDistance = 0.0;
            maximalMinDistance = 0.0;
            double count = 0;
            foreach (var level in algorithm.Levels)
            {
                foreach (var u in level)
                {
                    double m = double.PositiveInfinity;
                    foreach (var v in level)
                    {
                        if (u == v || compoundGraph.GetParent(u) != compoundGraph.GetParent(v))
                            continue;

                        var uPoint = LayoutUtil.GetClippingPoint(sizes[u], algorithm.VertexPositions[u],
                                                                   algorithm.VertexPositions[v]);
                        var vPoint = LayoutUtil.GetClippingPoint(sizes[v], algorithm.VertexPositions[v],
                                                                   algorithm.VertexPositions[u]);
                        double distance = (uPoint - vPoint).Length;
                        m = Math.Min(m, distance);
                    }
                    if (double.IsPositiveInfinity(m))
                        continue;
                    minimalMinDistance = Math.Min(minimalMinDistance, m);
                    averageMinDistance += m;
                    count++;
                    maximalMinDistance = Math.Max(maximalMinDistance, m);
                }
            }
        }


        protected override CompoundFDPLayoutParameters CreateIndividual()
        {
            CompoundFDPLayoutParameters parameters = new CompoundFDPLayoutParameters();
            parameters.ElasticConstant = rnd.NextDouble() * 300 + 1.0;
            parameters.GravitationFactor = rnd.NextDouble() * 30 + 1.0;
            parameters.NestingFactor = rnd.NextDouble() / 2.0 + 0.5;
            parameters.RepulsionConstant = rnd.NextDouble() * 1000 + 1.0;
            parameters.Phase1Iterations = rnd.Next(190) + 10;
            parameters.Phase2Iterations = rnd.Next(190) + 10;
            parameters.Phase3Iterations = rnd.Next(190) + 10;
            parameters.DisplacementLimitMultiplier = rnd.NextDouble();
            parameters.SeparationMultiplier = rnd.NextDouble() * 5.0 + 1;
            parameters.TemperatureDecreasing = rnd.NextDouble() / 2.0 + 0.3;
            parameters.TemperatureDecreasing = rnd.NextDouble() * 0.9 + 0.1;

            return CoalesceParameters(parameters);
        }
    }
}
