using System;
using GraphShape.Algorithms.Layout;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="CompoundFDPLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class CompoundFDPLayoutParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            string expectedPropertyName = null;

            var parameters = new CompoundFDPLayoutParameters();
            parameters.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable AccessToModifiedClosure
                if (expectedPropertyName is null)
                    Assert.Fail("Must not raise.");
                else
                    Assert.AreEqual(expectedPropertyName, args.PropertyName);
                // ReSharper restore AccessToModifiedClosure
            };

            parameters.IdealEdgeLength = 25;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.IdealEdgeLength);
            parameters.IdealEdgeLength = 30;

            expectedPropertyName = null;
            parameters.ElasticConstant = 0.005;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.ElasticConstant);
            parameters.ElasticConstant = 0.08;

            expectedPropertyName = null;
            parameters.RepulsionConstant = 150;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.RepulsionConstant);
            parameters.RepulsionConstant = 42;

            expectedPropertyName = null;
            parameters.NestingFactor = 0.2;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.NestingFactor);
            parameters.NestingFactor = 0.3;

            expectedPropertyName = null;
            parameters.GravitationFactor = 8;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.GravitationFactor);
            parameters.GravitationFactor = 10;

            expectedPropertyName = null;
            parameters.Phase1Iterations = 50;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.Phase1Iterations);
            parameters.Phase1Iterations = 60;

            expectedPropertyName = null;
            parameters.Phase2Iterations = 70;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.Phase2Iterations);
            parameters.Phase2Iterations = 80;

            expectedPropertyName = null;
            parameters.Phase3Iterations = 30;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.Phase3Iterations);
            parameters.Phase3Iterations = 40;

            expectedPropertyName = null;
            parameters.Phase2TemperatureInitialMultiplier = 0.5;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.Phase2TemperatureInitialMultiplier);
            parameters.Phase2TemperatureInitialMultiplier = 0.6;

            expectedPropertyName = null;
            parameters.Phase3TemperatureInitialMultiplier = 0.2;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.Phase3TemperatureInitialMultiplier);
            parameters.Phase3TemperatureInitialMultiplier = 0.3;

            expectedPropertyName = null;
            parameters.TemperatureDecreasing = 0.5;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.TemperatureDecreasing);
            parameters.TemperatureDecreasing = 0.4;

            expectedPropertyName = null;
            parameters.DisplacementLimitMultiplier = 0.5;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.DisplacementLimitMultiplier);
            parameters.DisplacementLimitMultiplier = 0.6;

            expectedPropertyName = null;
            parameters.SeparationMultiplier = 15;

            expectedPropertyName = nameof(CompoundFDPLayoutParameters.SeparationMultiplier);
            parameters.SeparationMultiplier = 16;
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new CompoundFDPLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.IdealEdgeLength = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.ElasticConstant = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Phase1Iterations = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Phase2Iterations = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Phase3Iterations = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Phase2TemperatureInitialMultiplier = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Phase3TemperatureInitialMultiplier = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.TemperatureDecreasing = -1);
        }

        [Test]
        public void Clone()
        {
            var parameters = new CompoundFDPLayoutParameters();
            var clonedParameters = (CompoundFDPLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new CompoundFDPLayoutParameters();
            parameters.IdealEdgeLength = 15;
            parameters.ElasticConstant = 0.5;
            parameters.RepulsionConstant = 120;
            parameters.NestingFactor = 0.3;
            parameters.GravitationFactor = 9;
            parameters.Phase1Iterations = 40;
            parameters.Phase2Iterations = 30;
            parameters.Phase3Iterations = 40;
            parameters.Phase2TemperatureInitialMultiplier = 0.6;
            parameters.Phase3TemperatureInitialMultiplier = 0.4;
            parameters.TemperatureDecreasing = 0.4;
            parameters.DisplacementLimitMultiplier = 0.4;
            parameters.SeparationMultiplier = 10;
            clonedParameters = (CompoundFDPLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}