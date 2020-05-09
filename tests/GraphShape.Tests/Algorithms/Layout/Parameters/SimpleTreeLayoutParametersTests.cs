using System;
using GraphShape.Algorithms.Layout;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="SimpleTreeLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class SimpleTreeLayoutParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            string expectedPropertyName = null;

            var parameters = new SimpleTreeLayoutParameters();
            parameters.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable AccessToModifiedClosure
                if (expectedPropertyName is null)
                    Assert.Fail("Must not raise.");
                else
                    Assert.AreEqual(expectedPropertyName, args.PropertyName);
                // ReSharper restore AccessToModifiedClosure
            };

            parameters.Direction = LayoutDirection.TopToBottom;

            expectedPropertyName = nameof(SimpleTreeLayoutParameters.Direction);
            parameters.Direction = LayoutDirection.BottomToTop;

            expectedPropertyName = null;
            parameters.VertexGap = 10;

            expectedPropertyName = nameof(SimpleTreeLayoutParameters.VertexGap);
            parameters.VertexGap = 42;

            expectedPropertyName = null;
            parameters.LayerGap = 10;

            expectedPropertyName = nameof(SimpleTreeLayoutParameters.LayerGap);
            parameters.LayerGap = 42;

            expectedPropertyName = null;
            parameters.SpanningTreeGeneration = SpanningTreeGeneration.DFS;

            expectedPropertyName = nameof(SimpleTreeLayoutParameters.SpanningTreeGeneration);
            parameters.SpanningTreeGeneration = SpanningTreeGeneration.BFS;
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new SimpleTreeLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.VertexGap = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.LayerGap = -1);
        }

        [Test]
        public void Clone()
        {
            var parameters = new SimpleTreeLayoutParameters();
            var clonedParameters = (SimpleTreeLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new SimpleTreeLayoutParameters();
            parameters.Direction = LayoutDirection.LeftToRight;
            parameters.VertexGap = 50;
            parameters.LayerGap = 50;
            parameters.SpanningTreeGeneration = SpanningTreeGeneration.BFS;
            clonedParameters = (SimpleTreeLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}