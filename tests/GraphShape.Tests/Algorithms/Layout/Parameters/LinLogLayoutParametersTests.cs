using System;
using GraphShape.Algorithms.Layout.Simple.FDP;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="LinLogLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class LinLogLayoutParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            string expectedPropertyName = null;

            var parameters = new LinLogLayoutParameters();
            parameters.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable AccessToModifiedClosure
                if (expectedPropertyName is null)
                    Assert.Fail("Must not raise.");
                else
                    Assert.AreEqual(expectedPropertyName, args.PropertyName);
                // ReSharper restore AccessToModifiedClosure
            };

            parameters.AttractionExponent = 1;

            expectedPropertyName = nameof(LinLogLayoutParameters.AttractionExponent);
            parameters.AttractionExponent = 1.2;

            expectedPropertyName = null;
            parameters.RepulsiveExponent = 0;

            expectedPropertyName = nameof(LinLogLayoutParameters.RepulsiveExponent);
            parameters.RepulsiveExponent = 1;

            expectedPropertyName = null;
            parameters.GravitationMultiplier = 0.1;

            expectedPropertyName = nameof(LinLogLayoutParameters.GravitationMultiplier);
            parameters.GravitationMultiplier = 0.2;

            expectedPropertyName = null;
            parameters.MaxIterations = 100;

            expectedPropertyName = nameof(LinLogLayoutParameters.MaxIterations);
            parameters.MaxIterations = 200;
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new LinLogLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.MaxIterations = -1);
        }

        [Test]
        public void Clone()
        {
            var parameters = new LinLogLayoutParameters();
            var clonedParameters = (LinLogLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new LinLogLayoutParameters();
            parameters.AttractionExponent= 1.2;
            parameters.RepulsiveExponent = 1;
            parameters.GravitationMultiplier = 0.2;
            parameters.MaxIterations = 150;
            clonedParameters = (LinLogLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}