using System;
using System.Collections.Generic;
using GraphShape.Algorithms.Layout;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="BoundedFRLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class BoundedFRLayoutParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            HashSet<string> expectedPropertyNames = null;

            var parameters = new BoundedFRLayoutParameters();
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
                nameof(BoundedFRLayoutParameters.VertexCount),
                nameof(BoundedFRLayoutParameters.ConstantOfRepulsion),
                nameof(BoundedFRLayoutParameters.ConstantOfAttraction),
                nameof(BoundedFRLayoutParameters.K),
                nameof(BoundedFRLayoutParameters.InitialTemperature)
            };
            parameters.VertexCount = 10;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.AttractionMultiplier = 1.2;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(BoundedFRLayoutParameters.AttractionMultiplier),
                nameof(BoundedFRLayoutParameters.ConstantOfAttraction)
            };
            parameters.AttractionMultiplier = 1.4;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.RepulsiveMultiplier = 0.6;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(BoundedFRLayoutParameters.RepulsiveMultiplier),
                nameof(BoundedFRLayoutParameters.ConstantOfRepulsion)
            };
            parameters.RepulsiveMultiplier = 0.7;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.MaxIterations = 200;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(BoundedFRLayoutParameters.MaxIterations)
            };
            parameters.MaxIterations = 150;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.Lambda = 0.95;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(BoundedFRLayoutParameters.Lambda)
            };
            parameters.Lambda = 0.99;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.CoolingFunction = FRCoolingFunction.Exponential;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(BoundedFRLayoutParameters.CoolingFunction)
            };
            parameters.CoolingFunction = FRCoolingFunction.Linear;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.Width = 100;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(BoundedFRLayoutParameters.ConstantOfRepulsion),
                nameof(BoundedFRLayoutParameters.ConstantOfAttraction),
                nameof(BoundedFRLayoutParameters.K),
                nameof(BoundedFRLayoutParameters.Width)
            };
            parameters.Width = 120;
            CollectionAssert.IsEmpty(expectedPropertyNames);

            expectedPropertyNames = null;
            parameters.Height = 100;

            expectedPropertyNames = new HashSet<string>
            {
                nameof(BoundedFRLayoutParameters.ConstantOfRepulsion),
                nameof(BoundedFRLayoutParameters.ConstantOfAttraction),
                nameof(BoundedFRLayoutParameters.K),
                nameof(BoundedFRLayoutParameters.InitialTemperature),
                nameof(BoundedFRLayoutParameters.Height)
            };
            parameters.Height = 120;
            CollectionAssert.IsEmpty(expectedPropertyNames);
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new BoundedFRLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.VertexCount = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.MaxIterations = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Height = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Width = -1);
        }

        [Test]
        public void Clone()
        {
            var parameters = new BoundedFRLayoutParameters();
            var clonedParameters = (BoundedFRLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new BoundedFRLayoutParameters();
            parameters.VertexCount = 20;
            parameters.AttractionMultiplier = 1.5;
            parameters.RepulsiveMultiplier = 0.7;
            parameters.MaxIterations = 150;
            parameters.Lambda = 0.9;
            parameters.CoolingFunction = FRCoolingFunction.Linear;
            parameters.Height = 150;
            parameters.Width = 150;
            clonedParameters = (BoundedFRLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}