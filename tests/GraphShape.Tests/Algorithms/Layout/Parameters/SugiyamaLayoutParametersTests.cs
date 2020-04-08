using System;
using GraphShape.Algorithms.Layout.Simple.Hierarchical;
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

            parameters.VerticalGap = 10;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.VerticalGap);
            parameters.VerticalGap = 42;

            expectedPropertyName = null;
            parameters.HorizontalGap = 10;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.HorizontalGap);
            parameters.HorizontalGap = 42;

            expectedPropertyName = null;
            parameters.DirtyRound = true;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.DirtyRound);
            parameters.DirtyRound = false;

            expectedPropertyName = null;
            parameters.Phase1IterationCount = 8;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.Phase1IterationCount);
            parameters.Phase1IterationCount = 10;

            expectedPropertyName = null;
            parameters.Phase2IterationCount = 5;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.Phase2IterationCount);
            parameters.Phase2IterationCount = 6;

            expectedPropertyName = null;
            parameters.MinimizeHierarchicalEdgeLong = true;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.MinimizeHierarchicalEdgeLong);
            parameters.MinimizeHierarchicalEdgeLong = false;

            expectedPropertyName = null;
            parameters.PositionCalculationMethod = PositionCalculationMethodTypes.PositionBased;

            expectedPropertyName = nameof(SugiyamaLayoutParameters.PositionCalculationMethod);
            parameters.PositionCalculationMethod = PositionCalculationMethodTypes.IndexBased;
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new SugiyamaLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.VerticalGap = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.HorizontalGap = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Phase1IterationCount = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Phase2IterationCount = -1);
        }

        [Test]
        public void Clone()
        {
            var parameters = new SugiyamaLayoutParameters();
            var clonedParameters = (SugiyamaLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new SugiyamaLayoutParameters();
            parameters.VerticalGap = 50;
            parameters.HorizontalGap = 50;
            parameters.DirtyRound = false;
            parameters.Phase1IterationCount = 10;
            parameters.Phase2IterationCount = 15;
            parameters.MinimizeHierarchicalEdgeLong = false;
            parameters.PositionCalculationMethod = PositionCalculationMethodTypes.IndexBased;
            clonedParameters = (SugiyamaLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}