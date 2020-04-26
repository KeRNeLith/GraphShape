using System;
using GraphShape.Algorithms.Layout;
using GraphShape.Algorithms.Layout.Simple.Hierarchical;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="EfficientSugiyamaLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class EfficientSugiyamaLayoutParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            string expectedPropertyName = null;

            var parameters = new EfficientSugiyamaLayoutParameters();
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

            expectedPropertyName = nameof(EfficientSugiyamaLayoutParameters.Direction);
            parameters.Direction = LayoutDirection.BottomToTop;

            expectedPropertyName = null;
            parameters.LayerGap = 15;

            expectedPropertyName = nameof(EfficientSugiyamaLayoutParameters.LayerGap);
            parameters.LayerGap = 42;

            expectedPropertyName = null;
            parameters.SliceGap = 15;

            expectedPropertyName = nameof(EfficientSugiyamaLayoutParameters.SliceGap);
            parameters.SliceGap = 42;

            expectedPropertyName = null;
            parameters.PositionMode = -1;

            expectedPropertyName = nameof(EfficientSugiyamaLayoutParameters.PositionMode);
            parameters.PositionMode = 3;

            expectedPropertyName = null;
            parameters.OptimizeWidth = false;

            expectedPropertyName = nameof(EfficientSugiyamaLayoutParameters.OptimizeWidth);
            parameters.OptimizeWidth = true;

            expectedPropertyName = null;
            parameters.WidthPerHeight = 1.0;

            expectedPropertyName = nameof(EfficientSugiyamaLayoutParameters.WidthPerHeight);
            parameters.WidthPerHeight = 1.2;

            expectedPropertyName = null;
            parameters.MinimizeEdgeLength = true;

            expectedPropertyName = nameof(EfficientSugiyamaLayoutParameters.MinimizeEdgeLength);
            parameters.MinimizeEdgeLength = false;

            expectedPropertyName = null;
            parameters.EdgeRouting = SugiyamaEdgeRouting.Traditional;

            expectedPropertyName = nameof(EfficientSugiyamaLayoutParameters.EdgeRouting);
            parameters.EdgeRouting = SugiyamaEdgeRouting.Orthogonal;
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new EfficientSugiyamaLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.LayerGap = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.SliceGap = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.PositionMode = 4);
        }

        [Test]
        public void Clone()
        {
            var parameters = new EfficientSugiyamaLayoutParameters();
            var clonedParameters = (EfficientSugiyamaLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new EfficientSugiyamaLayoutParameters();
            parameters.Direction = LayoutDirection.LeftToRight;
            parameters.LayerGap = 50;
            parameters.SliceGap = 50;
            parameters.PositionMode = 2;
            parameters.OptimizeWidth = true;
            parameters.WidthPerHeight = 1.1;
            parameters.MinimizeEdgeLength = false;
            parameters.EdgeRouting = SugiyamaEdgeRouting.Orthogonal;
            clonedParameters = (EfficientSugiyamaLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}