using System;
using System.Diagnostics.CodeAnalysis;
using GraphShape.Algorithms.EdgeRouting;
using JetBrains.Annotations;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.EdgeRouting
{
    /// <summary>
    /// Tests for <see cref="EdgeRoutingParameters"/>.
    /// </summary>
    [TestFixture]
    internal class EdgeRoutingParametersTests
    {
        #region Test classes

        private sealed class TestEdgeRoutingParameters : EdgeRoutingParameters, IEquatable<TestEdgeRoutingParameters>
        {
            public int TestParam { get; }

            private int _testParam2 = 12;

            public int TestParam2
            {
                [UsedImplicitly] get => _testParam2;
                set
                {
                    _testParam2 = value;
                    OnPropertyChanged();
                }
            }

            public int TestParam3 { get; set; }

            public TestEdgeRoutingParameters()
                : this(12)
            {
            }

            public TestEdgeRoutingParameters(int value)
            {
                TestParam = value;
                TestParam2 = 25;
                TestParam3 = 50;
            }

            public bool Equals(TestEdgeRoutingParameters other)
            {
                if (other is null)
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return _testParam2 == other._testParam2
                       && TestParam == other.TestParam
                       && TestParam3 == other.TestParam3;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return Equals(obj as TestEdgeRoutingParameters);
            }

            /// <inheritdoc />
            [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
            public override int GetHashCode()
            {
                int hashCode = _testParam2;
                hashCode = (hashCode * 397) ^ TestParam;
                hashCode = (hashCode * 397) ^ TestParam3;
                return hashCode;
            }
        }

        #endregion

        [Test]
        public void ParameterRaise()
        {
            string expectedPropertyName = null;

            var parameters = new TestEdgeRoutingParameters();
            parameters.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable AccessToModifiedClosure
                if (expectedPropertyName is null)
                    Assert.Fail("Must not raise.");
                else
                    Assert.AreEqual(expectedPropertyName, args.PropertyName);
                // ReSharper restore AccessToModifiedClosure
            };

            parameters.TestParam3 = 42;

            expectedPropertyName = nameof(TestEdgeRoutingParameters.TestParam2);
            parameters.TestParam2 = 72;
        }

        [Test]
        public void Clone()
        {
            var parameters = new TestEdgeRoutingParameters();
            var clonedParameters = (TestEdgeRoutingParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new TestEdgeRoutingParameters(12);
            clonedParameters = (TestEdgeRoutingParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new TestEdgeRoutingParameters(88);
            parameters.TestParam2 = 66;
            parameters.TestParam3 = 33;
            clonedParameters = (TestEdgeRoutingParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}