using System;
using GraphShape.Algorithms.Layout;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="RandomLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class RandomLayoutParametersTests
    {
        [Test]
        public void ParameterRaise()
        {
            string expectedPropertyName = null;

            var parameters = new RandomLayoutParameters();
            parameters.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable AccessToModifiedClosure
                if (expectedPropertyName is null)
                    Assert.Fail("Must not raise.");
                else
                    Assert.AreEqual(expectedPropertyName, args.PropertyName);
                // ReSharper restore AccessToModifiedClosure
            };

            parameters.XOffset = 0.0;

            expectedPropertyName = nameof(RandomLayoutParameters.XOffset);
            parameters.XOffset = -12.0;

            expectedPropertyName = null;
            parameters.YOffset = 0.0;

            expectedPropertyName = nameof(RandomLayoutParameters.YOffset);
            parameters.YOffset = 5.0;
            
            expectedPropertyName = null;
            parameters.Width = 100.0;

            expectedPropertyName = nameof(RandomLayoutParameters.Width);
            parameters.Width = 350.0;

            expectedPropertyName = null;
            parameters.Height = 100.0;

            expectedPropertyName = nameof(RandomLayoutParameters.Height);
            parameters.Height = 325.0;
        }

        [Test]
        public void InvalidParameters()
        {
            var parameters = new RandomLayoutParameters();
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Width = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => parameters.Height = -1);
        }

        [Test]
        public void Clone()
        {
            var parameters = new RandomLayoutParameters();
            var clonedParameters = (RandomLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new RandomLayoutParameters();
            parameters.XOffset = 12.0;
            parameters.YOffset = -5.0;
            parameters.Width = 50.0;
            parameters.Height = 111.0;
            clonedParameters = (RandomLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}