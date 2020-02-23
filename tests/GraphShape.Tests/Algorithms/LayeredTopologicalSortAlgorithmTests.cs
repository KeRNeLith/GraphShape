using System;
using System.Collections.Generic;
using System.Linq;
using GraphShape.Algorithms;
using JetBrains.Annotations;
using QuikGraph;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms
{
    /// <summary>
    /// Tests for <see cref="LayeredTopologicalSortAlgorithm{TVertex,TEdge}"/>.
    ///</summary>
    [TestFixture]
    internal class LayeredTopologicalSortAlgorithmTests : AlgorithmTestsBase
    {
        #region Test helpers

        private static void CheckAlgorithmResults<TVertex, TEdge>(
            [NotNull] LayeredTopologicalSortAlgorithm<TVertex, TEdge> algorithm,
            [NotNull, ItemNotNull] TVertex[][] verticesPerLayer)
            where TEdge : IEdge<TVertex>
        {
            Assert.AreEqual(verticesPerLayer.Length, algorithm.LayerCount);

            for (var i = 0; i < verticesPerLayer.Length; ++i)
            {
                foreach (TVertex vertex in verticesPerLayer[i])
                {
                    Assert.AreEqual(i, algorithm.LayerIndices[vertex]);
                }

                CollectionAssert.AreEquivalent(
                    verticesPerLayer[i],
                    algorithm.Layers[i]);
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            var algorithm = new LayeredTopologicalSortAlgorithm<int, Edge<int>>(graph);
            AssertAlgorithmProperties(algorithm, graph);

            graph = new AdjacencyGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(1, 3),
                new Edge<int>(2, 3)
            });
            algorithm = new LayeredTopologicalSortAlgorithm<int, Edge<int>>(graph);
            AssertAlgorithmProperties(algorithm, graph);

            #region Local function

            void AssertAlgorithmProperties<TVertex, TEdge>(
                LayeredTopologicalSortAlgorithm<TVertex, TEdge> algo,
                IVertexAndEdgeListGraph<TVertex, TEdge> g)
                where TEdge : IEdge<TVertex>
            {
                AssertAlgorithmState(algo, g);
                Assert.Zero(algo.LayerCount);
                CollectionAssert.IsEmpty(algo.LayerIndices);
                CollectionAssert.IsEmpty(algo.Layers);
            }

            #endregion
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new LayeredTopologicalSortAlgorithm<int, Edge<int>>(null));
        }

        [Test]
        public void SimpleGraph()
        {
            var graph = new BidirectionalGraph<string, Edge<string>>();

            const int nbVertices = 3;
            var vertices = new string[nbVertices];
            for (int i = 1; i <= nbVertices; ++i)
            {
                vertices[i - 1] = i.ToString();
                graph.AddVertex(i.ToString());
            }
            graph.AddEdge(new Edge<string>(vertices[0], vertices[1]));
            graph.AddEdge(new Edge<string>(vertices[1], vertices[2]));

            var algorithm = new LayeredTopologicalSortAlgorithm<string, Edge<string>>(graph);
            int layerFinished = 0;
            var verticesPerLayer = new[] { new[] { vertices[0] }, new[] { vertices[1] }, new[] { vertices[2] } };
            var verticesPerLayerStack = new Stack<string[]>(verticesPerLayer.Reverse());
            algorithm.LayerFinished += (sender, args) =>
            {
                Assert.AreEqual(layerFinished, args.LayerIndex);
                CollectionAssert.AreEquivalent(verticesPerLayerStack.Pop(), args.Vertices);
                ++layerFinished;
            };
            algorithm.Compute();

            Assert.AreEqual(3, layerFinished);
            CheckAlgorithmResults(algorithm, verticesPerLayer);
        }

        [Test]
        public void MultipleSourceGraph()
        {
            var graph = new BidirectionalGraph<string, Edge<string>>();

            const int nbVertices = 5;
            var vertices = new string[nbVertices];
            for (int i = 1; i <= nbVertices; ++i)
            {
                vertices[i - 1] = i.ToString();
                graph.AddVertex(i.ToString());
            }
            graph.AddEdge(new Edge<string>(vertices[0], vertices[1]));
            graph.AddEdge(new Edge<string>(vertices[1], vertices[2]));
            graph.AddEdge(new Edge<string>(vertices[3], vertices[1]));
            graph.AddEdge(new Edge<string>(vertices[4], vertices[2]));

            var algorithm = new LayeredTopologicalSortAlgorithm<string, Edge<string>>(graph);
            int layerFinished = 0;
            var verticesPerLayer = new[]
            {
                new[] { vertices[0], vertices[3], vertices[4] },
                new[] { vertices[1] },
                new[] { vertices[2] }
            };
            var verticesPerLayerStack = new Stack<string[]>(verticesPerLayer.Reverse());
            algorithm.LayerFinished += (sender, args) =>
            {
                Assert.AreEqual(layerFinished, args.LayerIndex);
                CollectionAssert.AreEquivalent(verticesPerLayerStack.Pop(), args.Vertices);
                ++layerFinished;
            };
            algorithm.Compute();

            Assert.AreEqual(3, layerFinished);
            CheckAlgorithmResults(algorithm, verticesPerLayer);
        }

        [Test]
        public void NonAcyclic()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            graph.AddVerticesAndEdgeRange(new[]
            {
                new Edge<int>(1, 2),
                new Edge<int>(2, 3),
                new Edge<int>(3, 1),
                new Edge<int>(4, 1)
            });

            var algorithm = new LayeredTopologicalSortAlgorithm<int, Edge<int>>(graph);
            Assert.Throws<NonAcyclicGraphException>(() => algorithm.Compute());
        }
    }
}