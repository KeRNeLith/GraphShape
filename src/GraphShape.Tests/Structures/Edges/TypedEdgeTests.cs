using System;
using NUnit.Framework;

namespace GraphShape.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="TypedEdge{TVertex}"/>.
    /// </summary>
    [TestFixture]
    internal class TypedEdgeTests
    {
        [Test]
        public void Constructor()
        {
            var edge1 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            Assert.AreEqual(1, edge1.Source);
            Assert.AreEqual(2, edge1.Target);
            Assert.AreEqual(EdgeTypes.General, edge1.Type);

            var edge2 = new TypedEdge<int>(2, 1, EdgeTypes.Hierarchical);
            Assert.AreEqual(2, edge2.Source);
            Assert.AreEqual(1, edge2.Target);
            Assert.AreEqual(EdgeTypes.Hierarchical, edge2.Type);

            var vertex1 = new TestVertex("1");
            var vertex2 = new TestVertex("2");
            var edge3 = new TypedEdge<TestVertex>(vertex1, vertex2, EdgeTypes.General);
            Assert.AreSame(vertex1, edge3.Source);
            Assert.AreSame(vertex2, edge3.Target);
            Assert.AreEqual(EdgeTypes.General, edge3.Type);

            var edge4 = new TypedEdge<TestVertex>(vertex2, vertex1, EdgeTypes.Hierarchical);
            Assert.AreSame(vertex2, edge4.Source);
            Assert.AreSame(vertex1, edge4.Target);
            Assert.AreEqual(EdgeTypes.Hierarchical, edge4.Type);
        }

        [Test]
        public void Constructor_Throws()
        {
            var vertex = new TestVertex();
            // ReSharper disable ObjectCreationAsStatement
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new TypedEdge<TestVertex>(vertex, null, EdgeTypes.General));
            Assert.Throws<ArgumentNullException>(() => new TypedEdge<TestVertex>(null, vertex, EdgeTypes.General));
            Assert.Throws<ArgumentNullException>(() => new TypedEdge<TestVertex>(null, null, EdgeTypes.General));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ObjectCreationAsStatement
        }

        [Test]
        public void ObjectToString()
        {
            var edge1 = new TypedEdge<int>(1, 2, EdgeTypes.General);
            StringAssert.AreEqualIgnoringCase($"{nameof(EdgeTypes.General)}: 1->2", edge1.ToString());

            var edge2 = new TypedEdge<int>(2, 1, EdgeTypes.Hierarchical);
            StringAssert.AreEqualIgnoringCase($"{nameof(EdgeTypes.Hierarchical)}: 2->1", edge2.ToString());
        }
    }
}