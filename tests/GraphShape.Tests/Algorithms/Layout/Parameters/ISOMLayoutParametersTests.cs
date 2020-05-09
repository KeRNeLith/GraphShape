using System;
using GraphShape.Algorithms.Layout;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="ISOMLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class ISOMLayoutParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            string expectedPropertyName = null;

            var parameters = new ISOMLayoutParameters();
            parameters.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable AccessToModifiedClosure
                if (expectedPropertyName is null)
                    Assert.Fail("Must not raise.");
                else
                    Assert.AreEqual(expectedPropertyName, args.PropertyName);
                // ReSharper restore AccessToModifiedClosure
            };

            parameters.Width = 300;

            expectedPropertyName = nameof(ISOMLayoutParameters.Width);
            parameters.Width = 400;

            expectedPropertyName = null;
            parameters.Height = 300;

            expectedPropertyName = nameof(ISOMLayoutParameters.Height);
            parameters.Height = 400;

            expectedPropertyName = null;
            parameters.MaxEpochs = 2000;

            expectedPropertyName = nameof(ISOMLayoutParameters.MaxEpochs);
            parameters.MaxEpochs = 400;

            expectedPropertyName = null;
            parameters.RadiusConstantTime = 100;

            expectedPropertyName = nameof(ISOMLayoutParameters.RadiusConstantTime);
            parameters.RadiusConstantTime = 200;

            expectedPropertyName = null;
            parameters.InitialRadius = 5;

            expectedPropertyName = nameof(ISOMLayoutParameters.InitialRadius);
            parameters.InitialRadius = 10;

            expectedPropertyName = null;
            parameters.MinRadius = 1;

            expectedPropertyName = nameof(ISOMLayoutParameters.MinRadius);
            parameters.MinRadius = 2;

            expectedPropertyName = null;
            parameters.InitialAdaptation = 0.9;

            expectedPropertyName = nameof(ISOMLayoutParameters.InitialAdaptation);
            parameters.InitialAdaptation = 2;

            expectedPropertyName = null;
            parameters.MinAdaptation = 0;

            expectedPropertyName = nameof(ISOMLayoutParameters.MinAdaptation);
            parameters.MinAdaptation = 1;

            expectedPropertyName = null;
            parameters.CoolingFactor = 2;

            expectedPropertyName = nameof(ISOMLayoutParameters.CoolingFactor);
            parameters.CoolingFactor = 4;
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new ISOMLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Width = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Height = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.MaxEpochs = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.RadiusConstantTime = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.InitialRadius = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.MinRadius = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.InitialAdaptation = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.MinAdaptation = -1);
        }

        [Test]
        public void Clone()
        {
            var parameters = new ISOMLayoutParameters();
            var clonedParameters = (ISOMLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new ISOMLayoutParameters();
            parameters.Width= 400;
            parameters.Height = 400;
            parameters.MaxEpochs = 250;
            parameters.RadiusConstantTime = 150;
            parameters.InitialRadius = 10;
            parameters.MinRadius = 5;
            parameters.InitialAdaptation = 0.9;
            parameters.MinAdaptation = 0.5;
            parameters.CoolingFactor = 5;
            clonedParameters = (ISOMLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}