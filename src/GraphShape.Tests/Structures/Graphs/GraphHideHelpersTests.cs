using System;
using NUnit.Framework;
using QuikGraph;

namespace GraphShape.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="GraphHideHelpers{TVertex,TEdge}"/>.
    /// </summary>
    [TestFixture]
    internal class GraphHideHelpersTests
    {
        // Check other graph feature are not supported

        [Test]
        public void GraphHideHelperOnly()
        {
            var graph = new BidirectionalGraph<int, Edge<int>>();
            var helper = new GraphHideHelpers<int, Edge<int>>(graph);

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<NotSupportedException>(() => { var _ = helper.IsDirected; });
            Assert.Throws<NotSupportedException>(() => { var _ = helper.AllowParallelEdges; });
            Assert.Throws<NotSupportedException>(() => { var _ = helper.VertexCount; });
            Assert.Throws<NotSupportedException>(() => { var _ = helper.Vertices; });
            Assert.Throws<NotSupportedException>(() => { var _ = helper.IsVerticesEmpty; });
            Assert.Throws<NotSupportedException>(() => helper.ContainsVertex(1));
            Assert.Throws<NotSupportedException>(() => { var _ = helper.EdgeCount; });
            Assert.Throws<NotSupportedException>(() => { var _ = helper.Edges; });
            Assert.Throws<NotSupportedException>(() => { var _ = helper.IsEdgesEmpty; });
            Assert.Throws<NotSupportedException>(() => helper.ContainsEdge(new Edge<int>(1, 2)));
            Assert.Throws<NotSupportedException>(() => helper.ContainsEdge(1, 2));
            Assert.Throws<NotSupportedException>(() => helper.OutDegree(1));
            Assert.Throws<NotSupportedException>(() => helper.OutEdges(1));
            Assert.Throws<NotSupportedException>(() => helper.OutEdge(1, 0));
            Assert.Throws<NotSupportedException>(() => helper.IsOutEdgesEmpty(1));
            Assert.Throws<NotSupportedException>(() => helper.TryGetOutEdges(1, out _));
            Assert.Throws<NotSupportedException>(() => helper.InDegree(1));
            Assert.Throws<NotSupportedException>(() => helper.InEdges(1));
            Assert.Throws<NotSupportedException>(() => helper.InEdge(1, 0));
            Assert.Throws<NotSupportedException>(() => helper.IsInEdgesEmpty(1));
            Assert.Throws<NotSupportedException>(() => helper.Degree(1));
            Assert.Throws<NotSupportedException>(() => helper.TryGetInEdges(1, out _));
            Assert.Throws<NotSupportedException>(() => helper.TryGetEdge(1, 2, out _));
            Assert.Throws<NotSupportedException>(() => helper.TryGetEdges(1, 2, out _));
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}