using System;
using NUnit.Framework;

namespace GraphShape.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="WeightedEdge{TVertex}"/>.
    /// </summary>
    [TestFixture]
    internal class WeightedEdgeTests
    {
        [Test]
        public void Constructor()
        {
            var edge1 = new WeightedEdge<int>(1, 2);
            Assert.AreEqual(1, edge1.Source);
            Assert.AreEqual(2, edge1.Target);
            Assert.AreEqual(1.0, edge1.Weight);

            var edge2 = new WeightedEdge<int>(2, 1, 10.0);
            Assert.AreEqual(2, edge2.Source);
            Assert.AreEqual(1, edge2.Target);
            Assert.AreEqual(10.0, edge2.Weight);

            var edge3 = new WeightedEdge<int>(1, 1, -10.0);
            Assert.AreEqual(1, edge3.Source);
            Assert.AreEqual(1, edge3.Target);
            Assert.AreEqual(-10.0, edge3.Weight);

            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            var edge4 = new WeightedEdge<TestVertex>(vertex1, vertex2);
            Assert.AreSame(vertex1, edge4.Source);
            Assert.AreSame(vertex2, edge4.Target);
            Assert.AreEqual(1.0, edge4.Weight);

            var edge5 = new WeightedEdge<TestVertex>(vertex2, vertex1, 10.0);
            Assert.AreSame(vertex2, edge5.Source);
            Assert.AreSame(vertex1, edge5.Target);
            Assert.AreEqual(10.0, edge5.Weight);

            var edge6 = new WeightedEdge<TestVertex>(vertex1, vertex1, -10.0);
            Assert.AreSame(vertex1, edge6.Source);
            Assert.AreSame(vertex1, edge6.Target);
            Assert.AreEqual(-10.0, edge6.Weight);
        }

        [Test]
        public void Constructor_Throws()
        {
            var vertex = new TestVertex();
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new WeightedEdge<TestVertex>(vertex, null));
            Assert.Throws<ArgumentNullException>(() => new WeightedEdge<TestVertex>(null, vertex));
            Assert.Throws<ArgumentNullException>(() => new WeightedEdge<TestVertex>(null, null));

            Assert.Throws<ArgumentNullException>(() => new WeightedEdge<TestVertex>(vertex, null, 10.0));
            Assert.Throws<ArgumentNullException>(() => new WeightedEdge<TestVertex>(null, vertex, 10.0));
            Assert.Throws<ArgumentNullException>(() => new WeightedEdge<TestVertex>(null, null, 10.0));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}