using System;
using GraphShape.Algorithms.Layout.Simple.FDP;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="KKLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class KKLayoutParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            string expectedPropertyName = null;

            var parameters = new KKLayoutParameters();
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

            expectedPropertyName = nameof(KKLayoutParameters.Width);
            parameters.Width = 400;

            expectedPropertyName = null;
            parameters.Height = 300;

            expectedPropertyName = nameof(KKLayoutParameters.Height);
            parameters.Height = 400;

            expectedPropertyName = null;
            parameters.MaxIterations = 200;

            expectedPropertyName = nameof(KKLayoutParameters.MaxIterations);
            parameters.MaxIterations = 400;

            expectedPropertyName = null;
            parameters.K = 1;

            expectedPropertyName = nameof(KKLayoutParameters.K);
            parameters.K = 2;

            expectedPropertyName = null;
            parameters.ExchangeVertices = false;

            expectedPropertyName = nameof(KKLayoutParameters.ExchangeVertices);
            parameters.ExchangeVertices = true;

            expectedPropertyName = null;
            parameters.LengthFactor = 1;

            expectedPropertyName = nameof(KKLayoutParameters.LengthFactor);
            parameters.LengthFactor = 2;

            expectedPropertyName = null;
            parameters.DisconnectedMultiplier = 0.5;

            expectedPropertyName = nameof(KKLayoutParameters.DisconnectedMultiplier);
            parameters.DisconnectedMultiplier = 0.6;
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new KKLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Width = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Height = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.MaxIterations = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.LengthFactor = -1);
        }

        [Test]
        public void Clone()
        {
            var parameters = new KKLayoutParameters();
            var clonedParameters = (KKLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new KKLayoutParameters();
            parameters.Width= 400;
            parameters.Height = 400;
            parameters.MaxIterations = 250;
            parameters.K = 2;
            parameters.ExchangeVertices = true;
            parameters.LengthFactor = 5;
            parameters.DisconnectedMultiplier = 0.6;
            clonedParameters = (KKLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}