using System;
using System.Collections.Generic;
using GraphShape.Algorithms.Layout;
using GraphShape.Optimization.GeneticAlgorithm;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Optimization.Algorithms
{
    /// <summary>
    /// Optimizer of parameters for Compound FDP algorithm.
    /// </summary>
    internal class GeneticCompoundFDPOptimizer : GeneticAlgorithmBase<CompoundFDPLayoutParameters, GeneticAlgorithmParameters>
    {
        private const int BigGraph = 0;
        private const int SmallGraph = 1;
        private const int FlatGraph = 2;
        private const int RepGraph = 3;
        private const int StarGraph = 4;
        private const int CombinedGraph = 5;

        [NotNull, ItemNotNull]
        private readonly CompoundGraph<object, IEdge<object>>[] _graphs;

        public GeneticCompoundFDPOptimizer([NotNull] GeneticAlgorithmParameters parameters)
            : base(parameters)
        {
            _graphs = InitGraphs();
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

        #region Helpers

        [NotNull, ItemNotNull]
        private static string[] InitVertices(CompoundGraph<object, IEdge<object>> graph, int vertexCount)
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
        private static CompoundFDPLayoutParameters CoalesceParameters([NotNull] CompoundFDPLayoutParameters offspring)
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
            return offspring;
        }

        #endregion

        /// <inheritdoc />
        protected override CompoundFDPLayoutParameters Mutate(CompoundFDPLayoutParameters chromosome)
        {
            bool mutate = Rand.NextDouble() < Parameters.MutationRate;
            bool lowering = Rand.NextDouble() < 0.5;
            if (mutate)
                chromosome.ElasticConstant *= lowering ? Rand.NextDouble() : Math.Max(1.0, Rand.NextDouble() * 3);

            mutate = Rand.NextDouble() < Parameters.MutationRate;
            lowering = Rand.NextDouble() < 0.5;
            if (mutate)
                chromosome.GravitationFactor *= lowering ? Rand.NextDouble() : Math.Max(1.0, Rand.NextDouble() * 3);

            mutate = Rand.NextDouble() < Parameters.MutationRate;
            lowering = Rand.NextDouble() < 0.5;
            if (mutate)
                chromosome.NestingFactor *= lowering ? Rand.NextDouble() : Math.Max(1.0, Rand.NextDouble() * 3);

            mutate = Rand.NextDouble() < Parameters.MutationRate;
            lowering = Rand.NextDouble() < 0.5;
            if (mutate)
                chromosome.RepulsionConstant *= lowering ? Rand.NextDouble() : Math.Max(1.0, Rand.NextDouble() * 3);

            mutate = Rand.NextDouble() < Parameters.MutationRate;
            if (mutate)
                chromosome.Phase1Iterations = Rand.Next(190) + 10;

            mutate = Rand.NextDouble() < Parameters.MutationRate;
            if (mutate)
                chromosome.Phase2Iterations = Rand.Next(190) + 10;

            mutate = Rand.NextDouble() < Parameters.MutationRate;
            if (mutate)
                chromosome.Phase3Iterations = Rand.Next(190) + 10;

            mutate = Rand.NextDouble() < Parameters.MutationRate;
            lowering = Rand.NextDouble() < 0.5;
            if (mutate)
                chromosome.DisplacementLimitMultiplier *= lowering ? Rand.NextDouble() : Math.Max(1.0, Rand.NextDouble() * 3);

            mutate = Rand.NextDouble() < Parameters.MutationRate;
            lowering = Rand.NextDouble() < 0.5;
            if (mutate)
                chromosome.SeparationMultiplier *= lowering ? Rand.NextDouble() : Math.Max(1.0, Rand.NextDouble() * 3);

            mutate = Rand.NextDouble() < Parameters.MutationRate;
            lowering = Rand.NextDouble() < 0.5;
            if (mutate)
                chromosome.TemperatureDecreasing *= lowering ? Rand.NextDouble() : Math.Max(1.0, Rand.NextDouble() * 2);

            return CoalesceParameters(chromosome);
        }

        /// <inheritdoc />
        protected override CompoundFDPLayoutParameters Crossover(CompoundFDPLayoutParameters parent1, CompoundFDPLayoutParameters parent2)
        {
            double crossoverChance = Rand.NextDouble();
            var offspring = new CompoundFDPLayoutParameters();
            int crossoverIndex = Rand.Next(12) + 1;
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

                offspring = CoalesceParameters(offspring);
            }
            return offspring;
        }

        /// <inheritdoc />
        protected override double EvaluateFitness(CompoundFDPLayoutParameters individual)
        {
            double fitness = 0.0;
            for (int c = 0; c < 2; ++c)
            {
                for (int i = 0; i < _graphs.Length; ++i)
                {
                    if (i != BigGraph)
                        continue;
                    double f = EvaluateFitnessForGraph(individual, _graphs[i]);
                    if (double.IsNaN(f))
                        return double.PositiveInfinity;
                    fitness += f;
                }
            }

            // edge crossing
            // edge length
            // node overlap
            // size of canvas
            // canvas ratio
            return fitness;
        }

        private const double NodeOverlapMultiplier = 1000000000000;
        private const double NodeDistanceMultiplier = 50000;
        private const double CanvasSizeMultiplier = 1000;
        private const double CanvasRatioMultiplier = 3000;
        private const double EdgeLengthMultiplier = 5000000;
        private const double EdgeCrossingMultiplier = 1000000000;
        private const double PhaseLengthMultiplier = 30000;

        [Pure]
        private static double EvaluateFitnessForGraph(
            [NotNull] CompoundFDPLayoutParameters chromosome,
            [NotNull] CompoundGraph<object, IEdge<object>> compoundGraph)
        {
            var verticesSizes = new Dictionary<object, Size>();
            var verticesBorders = new Dictionary<object, Thickness>();
            var layoutTypes = new Dictionary<object, CompoundVertexInnerLayoutType>();

            var size = new Size(20, 20);
            foreach (object v in compoundGraph.SimpleVertices)
            {
                verticesSizes[v] = size;
            }

            var border = new Thickness(5, 10, 5, 5);
            foreach (object v in compoundGraph.CompoundVertices)
            {
                verticesSizes[v] = new Size();
                verticesBorders[v] = border;
                layoutTypes[v] = CompoundVertexInnerLayoutType.Automatic;
            }

            // Run the compound FDP algorithm
            var algorithm = new CompoundFDPLayoutAlgorithm<object, IEdge<object>, CompoundGraph<object, IEdge<object>>>(
                compoundGraph,
                verticesSizes,
                verticesBorders,
                layoutTypes,
                chromosome);
            algorithm.Compute();

            double fitness = 0.0;

            // Refresh the sizes of the compound vertices
            foreach (object v in compoundGraph.CompoundVertices)
            {
                Thickness b = verticesBorders[v];
                Size innerCanvasSize = algorithm.InnerCanvasSizes[v];
                var s = new Size(
                    b.Left + innerCanvasSize.Width + b.Right,
                    b.Top + innerCanvasSize.Height + b.Bottom);
                verticesSizes[v] = s;
            }

            // NODE OVERLAP
            double overlaps = EvaluateNodeOverlaps(compoundGraph, algorithm, verticesSizes);
            fitness += overlaps * NodeOverlapMultiplier;

            // CANVAS SIZE
            Size canvasSize = EvaluateCanvasSize(compoundGraph, algorithm, verticesSizes);
            fitness += canvasSize.Width * canvasSize.Height * CanvasSizeMultiplier;

            // CANVAS RATIO
            double canvasRatio = canvasSize.Width / canvasSize.Height;
            canvasRatio = canvasRatio < 1 ? 1 / canvasRatio : canvasRatio;
            fitness += canvasRatio * CanvasRatioMultiplier;

            // NODE DISTANCES
            double idealDistance = chromosome.IdealEdgeLength;
            EvaluateNodeDistances(
                compoundGraph,
                algorithm,
                verticesSizes,
                out double minimalMinDistance,
                out double averageMinDistance,
                out double maximalMinDistance);
            fitness += (averageMinDistance - idealDistance) * NodeDistanceMultiplier;
            fitness += (minimalMinDistance - idealDistance) * NodeDistanceMultiplier;
            fitness += (maximalMinDistance - idealDistance) * NodeDistanceMultiplier;

            // EDGE LENGTH
            double edgeLength = EvaluateEdgeLength(
                compoundGraph,
                algorithm,
                verticesSizes,
                chromosome.IdealEdgeLength,
                chromosome.NestingFactor);
            fitness += edgeLength * EdgeLengthMultiplier;

            // EDGE CROSSING
            double edgeCrossing = EvaluateEdgeCrossing(compoundGraph, algorithm, verticesSizes);
            fitness += edgeCrossing * EdgeCrossingMultiplier;

            // PHASE_LENGTH
            double phaseLength = chromosome.Phase1Iterations + chromosome.Phase2Iterations + chromosome.Phase3Iterations;
            phaseLength *= PhaseLengthMultiplier;
            fitness += phaseLength;

            return fitness;
        }

        [Pure]
        private static double EvaluateNodeOverlaps(
            [NotNull] ICompoundGraph<object, IEdge<object>> compoundGraph,
            [NotNull] CompoundFDPLayoutAlgorithm<object, IEdge<object>, CompoundGraph<object, IEdge<object>>> algorithm,
            [NotNull] IDictionary<object, Size> verticesSizes)
        {
            double overlapArea = 0.0;
            foreach (HashSet<object> level in algorithm.Levels)
            {
                foreach (object u in level)
                {
                    foreach (object v in level)
                    {
                        if (ReferenceEquals(u, v) || compoundGraph.GetParent(u) != compoundGraph.GetParent(v))
                            continue;

                        Point uPosition = algorithm.VerticesPositions[u];
                        Point vPosition = algorithm.VerticesPositions[v];
                        Size uSize = verticesSizes[u];
                        Size vSize = verticesSizes[v];
                        
                        var uRect = new Rect(uPosition, uSize);
                        var vRect = new Rect(vPosition, vSize);

                        // Get the overlap size
                        uRect.Intersect(vRect);
                        if (double.IsNegativeInfinity(uRect.Width) || double.IsNegativeInfinity(uRect.Height))
                            continue;

                        overlapArea += uRect.Width * uRect.Height;
                    }
                }
            }

            return overlapArea;
        }

        [Pure]
        private static Size EvaluateCanvasSize(
            [NotNull] IVertexSet<object> compoundGraph,
            [NotNull] ILayoutAlgorithm<object, IEdge<object>, CompoundGraph<object, IEdge<object>>> algorithm,
            [NotNull] IReadOnlyDictionary<object, Size> verticesSizes)
        {
            var topLeft = new Point(double.PositiveInfinity, double.PositiveInfinity);
            var bottomRight = new Point(double.NegativeInfinity, double.NegativeInfinity);
            foreach (object v in compoundGraph.Vertices)
            {
                Point pos = algorithm.VerticesPositions[v];
                Size size = verticesSizes[v];

                topLeft.X = Math.Min(topLeft.X, pos.X - size.Width / 2.0);
                topLeft.Y = Math.Min(topLeft.Y, pos.Y - size.Height / 2.0);

                bottomRight.X = Math.Max(bottomRight.X, pos.X + size.Width / 2.0);
                bottomRight.Y = Math.Max(bottomRight.Y, pos.Y + size.Height / 2.0);
            }

            Vector sizeVector = bottomRight - topLeft;
            return new Size(sizeVector.X, sizeVector.Y);
        }

        private static void EvaluateNodeDistances(
            [NotNull] ICompoundGraph<object, IEdge<object>> compoundGraph,
            [NotNull] CompoundFDPLayoutAlgorithm<object, IEdge<object>, CompoundGraph<object, IEdge<object>>> algorithm,
            [NotNull] IReadOnlyDictionary<object, Size> verticesSizes,
            out double minimalMinDistance,
            out double averageMinDistance,
            out double maximalMinDistance)
        {
            minimalMinDistance = 0.0;
            averageMinDistance = 0.0;
            maximalMinDistance = 0.0;
            foreach (HashSet<object> level in algorithm.Levels)
            {
                foreach (object u in level)
                {
                    double m = double.PositiveInfinity;
                    foreach (object v in level)
                    {
                        if (ReferenceEquals(u, v) || compoundGraph.GetParent(u) != compoundGraph.GetParent(v))
                            continue;

                        Point uPoint = LayoutUtils.GetClippingPoint(
                            verticesSizes[u],
                            algorithm.VerticesPositions[u],
                            algorithm.VerticesPositions[v]);

                        Point vPoint = LayoutUtils.GetClippingPoint(
                            verticesSizes[v],
                            algorithm.VerticesPositions[v],
                            algorithm.VerticesPositions[u]);
                        double distance = (uPoint - vPoint).Length;
                        m = Math.Min(m, distance);
                    }

                    if (double.IsPositiveInfinity(m))
                        continue;

                    minimalMinDistance = Math.Min(minimalMinDistance, m);
                    averageMinDistance += m;
                    maximalMinDistance = Math.Max(maximalMinDistance, m);
                }
            }
        }

        [Pure]
        private static double EvaluateEdgeLength(
            [NotNull] ICompoundGraph<object, IEdge<object>> compoundGraph,
            [NotNull] CompoundFDPLayoutAlgorithm<object, IEdge<object>, CompoundGraph<object, IEdge<object>>> algorithm,
            [NotNull] IReadOnlyDictionary<object, Size> verticesSizes,
            double idealEdgeLength,
            double nestingFactor)
        {
            double edgeLengthError = 0.0;
            
            foreach (IEdge<object> edge in compoundGraph.Edges)
            {
                Point uPos = algorithm.VerticesPositions[edge.Source];
                Point vPos = algorithm.VerticesPositions[edge.Target];
                Size uSize = verticesSizes[edge.Source];
                Size vSize = verticesSizes[edge.Target];

                Point uPoint = LayoutUtils.GetClippingPoint(uSize, uPos, vPos);
                Point vPoint = LayoutUtils.GetClippingPoint(vSize, vPos, uPos);

                double length = (uPoint - vPoint).Length;
                bool isInterEdge = compoundGraph.GetParent(edge.Source) != compoundGraph.GetParent(edge.Target);
                double iel = isInterEdge
                              ? idealEdgeLength *
                                (algorithm.LevelOfVertex(edge.Source) + algorithm.LevelOfVertex(edge.Target) + 1) *
                                nestingFactor
                              : idealEdgeLength;
                double error = Math.Pow(length - iel, 2);
                edgeLengthError += error;
            }

            return edgeLengthError;
        }

        [Pure]
        private static int EvaluateEdgeCrossing(
            [NotNull] IEdgeSet<object, IEdge<object>> compoundGraph,
            [NotNull] ILayoutAlgorithm<object, IEdge<object>, CompoundGraph<object, IEdge<object>>> algorithm,
            [NotNull] IReadOnlyDictionary<object, Size> verticesSizes)
        {
            int crossings = 0;
            foreach (IEdge<object> edge in compoundGraph.Edges)
            {
                Point uPos1 = algorithm.VerticesPositions[edge.Source];
                Point vPos1 = algorithm.VerticesPositions[edge.Target];
                Size uSize1 = verticesSizes[edge.Source];
                Size vSize1 = verticesSizes[edge.Target];

                Point uPoint1 = LayoutUtils.GetClippingPoint(uSize1, uPos1, vPos1);
                Point vPoint1 = LayoutUtils.GetClippingPoint(vSize1, vPos1, uPos1);
                foreach (IEdge<object> edge2 in compoundGraph.Edges)
                {
                    if (ReferenceEquals(edge, edge2))
                        continue;

                    Point uPos2 = algorithm.VerticesPositions[edge.Source];
                    Point vPos2 = algorithm.VerticesPositions[edge.Target];
                    Size uSize2 = verticesSizes[edge.Source];
                    Size vSize2 = verticesSizes[edge.Target];

                    Point uPoint2 = LayoutUtils.GetClippingPoint(uSize2, uPos2, vPos2);
                    Point vPoint2 = LayoutUtils.GetClippingPoint(vSize2, vPos2, uPos2);

                    Vector v1 = vPoint1 - uPoint1;
                    Vector v2 = vPoint2 - uPoint2;

                    if (v1 == v2 || v1 == -v2)
                        continue; // Parallel edges

                    double t2 = (uPoint1.Y - uPoint2.Y + (uPoint2.X - uPoint1.X) * v1.Y / v1.X) / (v2.Y - v2.X * v1.Y / v1.X);
                    double t1 = (uPoint2.X - uPoint1.X + t2 * v2.X) / v1.X;

                    Point p = uPoint1 + t1 * v1;
                    bool b1 = t1 > 0 && (p - uPoint1).Length < (vPoint1 - uPoint1).Length;
                    bool b2 = t2 > 0 && (p - uPoint2).Length < (vPoint2 - uPoint2).Length;

                    if (b1 && b2)
                    {
                        ++crossings;
                    }
                }
            }
            return crossings;
        }

        /// <inheritdoc />
        protected override CompoundFDPLayoutParameters CreateIndividual()
        {
            var parameters = new CompoundFDPLayoutParameters
            {
                ElasticConstant = Rand.NextDouble() * 300 + 1.0,
                GravitationFactor = Rand.NextDouble() * 30 + 1.0,
                NestingFactor = Rand.NextDouble() / 2.0 + 0.5,
                RepulsionConstant = Rand.NextDouble() * 1000 + 1.0,
                Phase1Iterations = Rand.Next(190) + 10,
                Phase2Iterations = Rand.Next(190) + 10,
                Phase3Iterations = Rand.Next(190) + 10,
                DisplacementLimitMultiplier = Rand.NextDouble(),
                SeparationMultiplier = Rand.NextDouble() * 5.0 + 1,
                TemperatureDecreasing = Rand.NextDouble() / 2.0 + 0.3
            };

            return CoalesceParameters(parameters);
        }
    }
}
