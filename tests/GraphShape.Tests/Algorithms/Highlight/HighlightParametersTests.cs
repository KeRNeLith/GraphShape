using System;
using System.Diagnostics.CodeAnalysis;
using GraphShape.Algorithms.Highlight;
using JetBrains.Annotations;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.Highlight
{
    /// <summary>
    /// Tests for <see cref="HighlightParameters"/>.
    /// </summary>
    [TestFixture]
    internal class HighlightParametersTests
    {
        #region Test classes

        private sealed class TestHighlightParameters : HighlightParameters, IEquatable<TestHighlightParameters>
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

            public TestHighlightParameters()
                : this(12)
            {
            }

            public TestHighlightParameters(int value)
            {
                TestParam = value;
                TestParam2 = 25;
                TestParam3 = 50;
            }

            public bool Equals(TestHighlightParameters other)
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
                return Equals(obj as TestHighlightParameters);
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

            var parameters = new TestHighlightParameters();
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

            expectedPropertyName = nameof(TestHighlightParameters.TestParam2);
            parameters.TestParam2 = 72;
        }

        [Test]
        public void Clone()
        {
            var parameters = new TestHighlightParameters();
            var clonedParameters = (TestHighlightParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new TestHighlightParameters(12);
            clonedParameters = (TestHighlightParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);

            parameters = new TestHighlightParameters(88);
            parameters.TestParam2 = 66;
            parameters.TestParam3 = 33;
            clonedParameters = (TestHighlightParameters)parameters.Clone();

            Assert.AreEqual(parameters, clonedParameters);
        }
    }
}