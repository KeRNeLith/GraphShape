using System;
using System.Collections.Generic;
using System.Windows;
using GraphShape.Algorithms.Layout;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="LayoutContext{TVertex, TEdge, TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class LayoutContextTests
    {
        [Test]
        public void Constructor()
        {
            var graph = new AdjacencyGraph<int, Edge<int>>();
            var verticesPositions = new Dictionary<int, Point>();
            var verticesSizes = new Dictionary<int, Size>();
            var context = new LayoutContext<int, Edge<int>, AdjacencyGraph<int, Edge<int>>>(
                graph,
                verticesPositions,
                verticesSizes,
                LayoutMode.Simple);
            
            Assert.AreSame(verticesPositions, context.Positions);
            Assert.AreSame(verticesSizes, context.Sizes);
            Assert.AreSame(graph, context.Graph);
            Assert.AreEqual(LayoutMode.Simple, context.Mode);
        }

        [Test]
        public void Constructor_Throws()
        {
            var verticesPositions = new Dictionary<int, Point>();
            var verticesSizes = new Dictionary<int, Size>();

            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.DoesNotThrow(
                () => new LayoutContext<int, Edge<int>, AdjacencyGraph<int, Edge<int>>>(null, verticesPositions, verticesSizes, LayoutMode.Simple));
            Assert.DoesNotThrow(
                () => new LayoutContext<int, Edge<int>, AdjacencyGraph<int, Edge<int>>>(null, null, verticesSizes, LayoutMode.Simple));
            Assert.Throws<ArgumentNullException>(
                () => new LayoutContext<int, Edge<int>, AdjacencyGraph<int, Edge<int>>>(null, verticesPositions, null, LayoutMode.Simple));
            Assert.Throws<ArgumentNullException>(
                () => new LayoutContext<int, Edge<int>, AdjacencyGraph<int, Edge<int>>>(null, null, null, LayoutMode.Simple));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}