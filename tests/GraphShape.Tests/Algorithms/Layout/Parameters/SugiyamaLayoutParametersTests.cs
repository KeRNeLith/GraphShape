using System;
using GraphShape.Algorithms.Layout;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="SugiyamaLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class SugiyamaLayoutParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            string expectedPropertyName = null;

            var parameters = new SugiyamaLayoutParameters();
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

            expectedPropertyName = nameof(SugiyamaLayoutParameters.Direction);
            parameters.Direction = LayoutDirection.BottomToTop;

            expectedPropertyName = null;
            parameters.LayerGap = 15;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.LayerGap);
            parameters.LayerGap = 42;

            expectedPropertyName = null;
            parameters.SliceGap = 15;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.SliceGap);
            parameters.SliceGap = 42;

            expectedPropertyName = null;
            parameters.PositionMode = -1;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.PositionMode);
            parameters.PositionMode = 3;

            expectedPropertyName = null;
            parameters.OptimizeWidth = false;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.OptimizeWidth);
            parameters.OptimizeWidth = true;

            expectedPropertyName = null;
            parameters.WidthPerHeight = 1.0;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.WidthPerHeight);
            parameters.WidthPerHeight = 1.2;

            expectedPropertyName = null;
            parameters.MinimizeEdgeLength = true;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.MinimizeEdgeLength);
            parameters.MinimizeEdgeLength = false;

            expectedPropertyName = null;
            parameters.EdgeRouting = SugiyamaEdgeRouting.Traditional;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.EdgeRouting);
            parameters.EdgeRouting = SugiyamaEdgeRouting.Orthogonal;
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new SugiyamaLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.LayerGap = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.SliceGap = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.PositionMode = 4);
        }

        [Test]
        public void Clone()
        {
            var parameters = new SugiyamaLayoutParameters();
            var clonedParameters = (SugiyamaLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new SugiyamaLayoutParameters();
            parameters.Direction = LayoutDirection.LeftToRight;
            parameters.LayerGap = 50;
            parameters.SliceGap = 50;
            parameters.PositionMode = 2;
            parameters.OptimizeWidth = true;
            parameters.WidthPerHeight = 1.1;
            parameters.MinimizeEdgeLength = false;
            parameters.EdgeRouting = SugiyamaEdgeRouting.Orthogonal;
            clonedParameters = (SugiyamaLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}