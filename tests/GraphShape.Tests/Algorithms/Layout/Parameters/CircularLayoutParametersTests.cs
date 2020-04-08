using GraphShape.Algorithms.Layout.Simple.Circular;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Layout
{
    /// <summary>
    /// Tests for <see cref="CircularLayoutParameters"/>.
    /// </summary>
    [TestFixture]
    internal class CircularLayoutParametersTests
    {
        [Test]
        public void Clone()
        {
            var parameters = new CircularLayoutParameters();
            var clonedParameters = (CircularLayoutParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}