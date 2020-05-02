using System;
using System.Collections.Generic;
using System.Windows;
using GraphShape.Algorithms.Layout;
using GraphShape.Algorithms.Layout.Contextual;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="ContextualLayoutContext{TVertex, TEdge, TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class ContextualLayoutContextTests
    {
        [Test]
        public void Constructor()
        {
            const int vertex = 10;
            var graph = new AdjacencyGraph<int, Edge<int>>();
            var verticesPositions = new Dictionary<int, Point>();
            var verticesSizes = new Dictionary<int, Size>();
            var context = new ContextualLayoutContext<int, Edge<int>, AdjacencyGraph<int, Edge<int>>>(
                graph,
                vertex,
                verticesPositions,
                verticesSizes);
            
            Assert.AreEqual(vertex, context.SelectedVertex);
            Assert.AreSame(verticesPositions, context.Positions);
            Assert.AreSame(verticesSizes, context.Sizes);
            Assert.AreSame(graph, context.Graph);
            Assert.AreEqual(LayoutMode.Simple, context.Mode);
        }

        [Test]
        public void Constructor_Throws()
        {
            const string vertex = "10";
            var verticesPositions = new Dictionary<string, Point>();
            var verticesSizes = new Dictionary<string, Size>();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.DoesNotThrow(
                () => new ContextualLayoutContext<string, Edge<string>, AdjacencyGraph<string, Edge<string>>>(null, vertex, verticesPositions, verticesSizes));
            Assert.DoesNotThrow(
                () => new ContextualLayoutContext<string, Edge<string>, AdjacencyGraph<string, Edge<string>>>(null, vertex, null, verticesSizes));
            Assert.Throws<ArgumentNullException>(
                () => new ContextualLayoutContext<string, Edge<string>, AdjacencyGraph<string, Edge<string>>>(null, vertex, verticesPositions, null));
            Assert.Throws<ArgumentNullException>(
                () => new ContextualLayoutContext<string, Edge<string>, AdjacencyGraph<string, Edge<string>>>(null, vertex, null, null));

            Assert.Throws<ArgumentNullException>(
                () => new ContextualLayoutContext<string, Edge<string>, AdjacencyGraph<string, Edge<string>>>(null, null, verticesPositions, verticesSizes));
            Assert.Throws<ArgumentNullException>(
                () => new ContextualLayoutContext<string, Edge<string>, AdjacencyGraph<string, Edge<string>>>(null, null, null, verticesSizes));
            Assert.Throws<ArgumentNullException>(
                () => new ContextualLayoutContext<string, Edge<string>, AdjacencyGraph<string, Edge<string>>>(null, null, verticesPositions, null));
            Assert.Throws<ArgumentNullException>(
                () => new ContextualLayoutContext<string, Edge<string>, AdjacencyGraph<string, Edge<string>>>(null, null, null, null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}