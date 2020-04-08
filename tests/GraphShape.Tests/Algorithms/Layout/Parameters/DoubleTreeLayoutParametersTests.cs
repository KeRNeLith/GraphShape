using System;
using GraphShape.Algorithms.Layout;
using GraphShape.Algorithms.Layout.Contextual;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="DoubleTreeLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class DoubleTreeLayoutParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            string expectedPropertyName = null;

            var parameters = new DoubleTreeLayoutParameters();
            parameters.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable AccessToModifiedClosure
                if (expectedPropertyName is null)
                    Assert.Fail("Must not raise.");
                else
                    Assert.AreEqual(expectedPropertyName, args.PropertyName);
                // ReSharper restore AccessToModifiedClosure
            };

            parameters.Direction = LayoutDirection.LeftToRight;

            expectedPropertyName = nameof(DoubleTreeLayoutParameters.Direction);
            parameters.Direction = LayoutDirection.BottomToTop;

            expectedPropertyName = null;
            parameters.VertexGap = 10;

            expectedPropertyName = nameof(DoubleTreeLayoutParameters.VertexGap);
            parameters.VertexGap = 42;

            expectedPropertyName = null;
            parameters.LayerGap = 10;

            expectedPropertyName = nameof(DoubleTreeLayoutParameters.LayerGap);
            parameters.LayerGap = 42;
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new DoubleTreeLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.VertexGap = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.LayerGap = -1);
        }

        [Test]
        public void Clone()
        {
            var parameters = new DoubleTreeLayoutParameters();
            var clonedParameters = (DoubleTreeLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new DoubleTreeLayoutParameters();
            parameters.Direction = LayoutDirection.LeftToRight;
            parameters.VertexGap = 50;
            parameters.LayerGap = 50;
            clonedParameters = (DoubleTreeLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}