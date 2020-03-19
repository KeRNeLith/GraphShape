using System;
using GraphShape.Algorithms.OverlapRemoval;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Tests for <see cref="OverlapRemovalParameters"/> and <see cref="OneWayFSAParameters"/>.
    /// </summary>
    [TestFixture]
    internal class OverlapRemovalParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            string expectedPropertyName = null;

            var parameters = new OverlapRemovalParameters();
            parameters.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable AccessToModifiedClosure
                if (expectedPropertyName is null)
                    Assert.Fail("Must not raise.");
                else
                    Assert.AreEqual(expectedPropertyName, args.PropertyName);
                // ReSharper restore AccessToModifiedClosure
            };

            expectedPropertyName = nameof(OverlapRemovalParameters.VerticalGap);
            parameters.VerticalGap = 42;

            expectedPropertyName = null;
            parameters.VerticalGap = 42;

            expectedPropertyName = nameof(OverlapRemovalParameters.HorizontalGap);
            parameters.HorizontalGap = 72;

            expectedPropertyName = null;
            parameters.HorizontalGap = 72;

            var oneWayFSAParameters = new OneWayFSAParameters();
            oneWayFSAParameters.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable AccessToModifiedClosure
                if (expectedPropertyName is null)
                    Assert.Fail("Must not raise.");
                else
                    Assert.AreEqual(expectedPropertyName, args.PropertyName);
                // ReSharper restore AccessToModifiedClosure
            };

            expectedPropertyName = nameof(OverlapRemovalParameters.VerticalGap);
            oneWayFSAParameters.VerticalGap = 42;

            expectedPropertyName = null;
            oneWayFSAParameters.VerticalGap = 42;

            expectedPropertyName = nameof(OverlapRemovalParameters.HorizontalGap);
            oneWayFSAParameters.HorizontalGap = 72;

            expectedPropertyName = null;
            oneWayFSAParameters.HorizontalGap = 72;

            expectedPropertyName = nameof(OneWayFSAParameters.Way);
            oneWayFSAParameters.Way = OneWayFSAWay.Vertical;

            expectedPropertyName = null;
            oneWayFSAParameters.Way = OneWayFSAWay.Vertical;

            expectedPropertyName = nameof(OneWayFSAParameters.Way);
            oneWayFSAParameters.Way = OneWayFSAWay.Horizontal;
        }

        [Test]
        public void Parameter_Throws()
        {
            var parameters = new OverlapRemovalParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.VerticalGap = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.HorizontalGap = -1);

            var oneWayFSAParameters = new OneWayFSAParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => oneWayFSAParameters.VerticalGap = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => oneWayFSAParameters.HorizontalGap = -1);
        }

        [Test]
        public void Clone()
        {
            var parameters = new OverlapRemovalParameters();
            var clonedParameters = (OverlapRemovalParameters)parameters.Clone();
            AssertAreEqual(parameters, clonedParameters);

            parameters = new OverlapRemovalParameters { HorizontalGap = 25, VerticalGap = 50 };
            clonedParameters = (OverlapRemovalParameters)parameters.Clone();
            AssertAreEqual(parameters, clonedParameters);

            var oneWayFSAParameters = new OneWayFSAParameters();
            var clonedOneWayFSAParameters = (OneWayFSAParameters)oneWayFSAParameters.Clone();
            AssertOneWayParamsAreEqual(oneWayFSAParameters, clonedOneWayFSAParameters);

            oneWayFSAParameters = new OneWayFSAParameters { HorizontalGap = 25, VerticalGap = 50 };
            clonedOneWayFSAParameters = (OneWayFSAParameters)oneWayFSAParameters.Clone();
            AssertOneWayParamsAreEqual(oneWayFSAParameters, clonedOneWayFSAParameters);

            #region Local function

            void AssertAreEqual(IOverlapRemovalParameters p1, IOverlapRemovalParameters p2)
            {
                Assert.AreEqual(p1.HorizontalGap, p2.HorizontalGap);
                Assert.AreEqual(p1.VerticalGap, p2.VerticalGap);
            }

            void AssertOneWayParamsAreEqual(OneWayFSAParameters p1, OneWayFSAParameters p2)
            {
                AssertAreEqual(p1, p2);
                Assert.AreEqual(p1.Way, p2.Way);
            }

            #endregion
        }
    }
}