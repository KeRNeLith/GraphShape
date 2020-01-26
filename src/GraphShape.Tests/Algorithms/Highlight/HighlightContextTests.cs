using System;
using GraphShape.Algorithms.Highlight;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests.Algorithms.Highlight
{
    /// <summary>
    /// Tests for <see cref="HighlightContext{TVertex,TEdge,TGraph}"/>.
    /// </summary>
    [TestFixture]
    internal class HighlightContextTests
    {
        [Test]
        public void Constructor()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var context = new HighlightContext<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(graph);
            Assert.AreSame(graph, context.Graph);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => new HighlightContext<int, Edge<int>, BidirectionalGraph<int, Edge<int>>>(null));
        }
    }
}