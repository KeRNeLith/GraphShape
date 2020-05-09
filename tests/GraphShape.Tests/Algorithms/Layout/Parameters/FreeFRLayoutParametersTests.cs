using System;
using System.Collections.Generic;
using GraphShape.Algorithms.Layout;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="FreeFRLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class FreeFRLayoutParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            HashSet<string> expectedPropertyNames = null;

            var parameters = new FreeFRLayoutParameters();
            parameters.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable AccessToModifiedClosure
                if (expectedPropertyNames is null)
                    Assert.Fail("Must not raise.");
                else
                    Assert.IsTrue(expectedPropertyNames.Remove(args.PropertyName));
                // ReSharper restore AccessToModifiedClosure
            };

            parameters.VertexCount = 0;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(FreeFRLayoutParameters.VertexCount),
                nameof(FreeFRLayoutParameters.ConstantOfRepulsion),
                nameof(FreeFRLayoutParameters.ConstantOfAttraction),
                nameof(FreeFRLayoutParameters.InitialTemperature)
            };
            parameters.VertexCount = 10;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.AttractionMultiplier = 1.2;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(FreeFRLayoutParameters.AttractionMultiplier),
                nameof(FreeFRLayoutParameters.ConstantOfAttraction)
            };
            parameters.AttractionMultiplier = 1.4;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.RepulsiveMultiplier = 0.6;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(FreeFRLayoutParameters.RepulsiveMultiplier),
                nameof(FreeFRLayoutParameters.ConstantOfRepulsion)
            };
            parameters.RepulsiveMultiplier = 0.7;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.MaxIterations = 200;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(FreeFRLayoutParameters.MaxIterations)
            };
            parameters.MaxIterations = 150;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.Lambda = 0.95;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(FreeFRLayoutParameters.Lambda)
            };
            parameters.Lambda = 0.99;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.CoolingFunction = FRCoolingFunction.Exponential;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(FreeFRLayoutParameters.CoolingFunction)
            };
            parameters.CoolingFunction = FRCoolingFunction.Linear;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.IdealEdgeLength = 10;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(FreeFRLayoutParameters.IdealEdgeLength),
                nameof(FreeFRLayoutParameters.ConstantOfRepulsion),
                nameof(FreeFRLayoutParameters.ConstantOfAttraction),
                nameof(FreeFRLayoutParameters.K),
                nameof(FreeFRLayoutParameters.InitialTemperature)
            };
            parameters.IdealEdgeLength = 12;
            CollectionAssert.IsEmpty(expectedPropertyNames);
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new FreeFRLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.VertexCount = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.MaxIterations = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.IdealEdgeLength = -1);
        }

        [Test]
        public void Clone()
        {
            var parameters = new FreeFRLayoutParameters();
            var clonedParameters = (FreeFRLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new FreeFRLayoutParameters();
            parameters.VertexCount = 20;
            parameters.AttractionMultiplier = 1.5;
            parameters.RepulsiveMultiplier = 0.7;
            parameters.MaxIterations = 150;
            parameters.Lambda = 0.9;
            parameters.CoolingFunction = FRCoolingFunction.Linear;
            parameters.IdealEdgeLength = 15;
            clonedParameters = (FreeFRLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}