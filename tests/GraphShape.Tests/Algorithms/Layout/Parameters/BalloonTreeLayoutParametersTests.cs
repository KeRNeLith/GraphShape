using System;
using GraphShape.Algorithms.Layout.Simple.Tree;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="BalloonTreeLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class BalloonTreeLayoutParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            string expectedPropertyName = null;

            var parameters = new BalloonTreeLayoutParameters();
            parameters.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable AccessToModifiedClosure
                if (expectedPropertyName is null)
                    Assert.Fail("Must not raise.");
                else
                    Assert.AreEqual(expectedPropertyName, args.PropertyName);
                // ReSharper restore AccessToModifiedClosure
            };

            parameters.MinRadius = 2;

            expectedPropertyName = nameof(BalloonTreeLayoutParameters.MinRadius);
            parameters.MinRadius = 4;

            expectedPropertyName = null;
            parameters.Border = 20;

            expectedPropertyName = nameof(BalloonTreeLayoutParameters.Border);
            parameters.Border = 42;
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new BalloonTreeLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.MinRadius = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Border = -1);
        }

        [Test]
        public void Clone()
        {
            var parameters = new BalloonTreeLayoutParameters();
            var clonedParameters = (BalloonTreeLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new BalloonTreeLayoutParameters();
            parameters.MinRadius = 10;
            parameters.Border = 50;
            clonedParameters = (BalloonTreeLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}