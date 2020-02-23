using System;
using NUnit.Framework;

namespace GraphShape.Tests.Structures
{
    /// <summary>
    /// Tests for <see cref="WrappedVertex{TVertex}"/>.
    /// </summary>
    [TestFixture]
    internal class WrappedVertexTests
    {
        [Test]
        public void Constructor()
        {
            var wrapper1 = new WrappedVertex<int>(1);
            Assert.AreEqual(1, wrapper1.Original);

            var vertex = new TestVertex();
            var wrapper2 = new WrappedVertex<TestVertex>(vertex);
            Assert.AreSame(vertex, wrapper2.Original);
        }

        [Test]
        public void Constructor_Throws()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(() => new WrappedVertex<TestVertex>(null));
        }
    }
}