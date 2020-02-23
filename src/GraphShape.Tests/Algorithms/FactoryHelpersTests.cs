using System.ComponentModel;
using GraphShape.Algorithms;
using GraphShape.Algorithms.Layout;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms
{
    /// <summary>
    /// Tests related to <see cref="FactoryHelpers"/>.
    /// </summary>
    [TestFixture]
    internal class FactoryHelpersTests
    {
        #region Test classes

#pragma warning disable 67

        private class TestLayoutParameters : ILayoutParameters
        {
            public int Param { get; }

            public TestLayoutParameters()
            {
            }

            public TestLayoutParameters(int value)
            {
                Param = value;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public object Clone()
            {
                return new TestLayoutParameters(Param);
            }
        }

        private class TestOtherLayoutParameters : ILayoutParameters
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public object Clone()
            {
                return new TestOtherLayoutParameters();
            }
        }

#pragma warning restore 67

        #endregion

        [Test]
        public void CreateNewParameters()
        {
            var parameters = new TestLayoutParameters(12);

            var newParameters = parameters.CreateNewParameters<TestLayoutParameters>();
            Assert.AreNotSame(parameters, newParameters);
            Assert.IsInstanceOf<TestLayoutParameters>(newParameters);
            Assert.AreEqual(parameters.Param, newParameters.Param);

            newParameters = FactoryHelpers.CreateNewParameters<TestLayoutParameters>(null);
            Assert.AreNotSame(parameters, newParameters);
            Assert.IsInstanceOf<TestLayoutParameters>(newParameters);
            Assert.AreEqual(0, newParameters.Param);

            var newOtherParameters = parameters.CreateNewParameters<TestOtherLayoutParameters>();
            Assert.AreNotSame(parameters, newOtherParameters);
            Assert.IsInstanceOf<TestOtherLayoutParameters>(newOtherParameters);
        }
    }
}