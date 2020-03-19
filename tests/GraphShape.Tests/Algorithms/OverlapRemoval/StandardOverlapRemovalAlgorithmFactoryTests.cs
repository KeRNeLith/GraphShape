using System;
using System.Collections.Generic;
using System.Windows;
using GraphShape.Algorithms.OverlapRemoval;
using JetBrains.Annotations;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Tests for <see cref="StandardOverlapRemovalAlgorithmFactory{TVertex}"/>.
    /// </summary>
    [TestFixture]
    internal class StandardOverlapRemovalAlgorithmFactoryTests
    {
        #region Tests classes

        private class TestOverlapRemovalContext : OverlapRemovalContext<int>
        {
            public TestOverlapRemovalContext([NotNull] IDictionary<int, Rect> rectangles)
                : base(rectangles)
            {
            }
        }

        #endregion

        [Test]
        public void StandardFactory()
        {
            var rectangles = new Dictionary<int, Rect>();
            var context = new TestOverlapRemovalContext(rectangles);

            var factory = new StandardOverlapRemovalAlgorithmFactory<int>();
            CollectionAssert.AreEqual(new[] { "FSA", "OneWayFSA" }, factory.AlgorithmTypes);


            Assert.IsNull(
                factory.CreateAlgorithm(
                    string.Empty,
                    context,
                    new OverlapRemovalParameters()));

            Assert.IsInstanceOf<FSAAlgorithm<int>>(
                factory.CreateAlgorithm(
                    "FSA",
                    context,
                    new OverlapRemovalParameters()));

            Assert.IsInstanceOf<OneWayFSAAlgorithm<int>>(
                factory.CreateAlgorithm(
                    "OneWayFSA",
                    context,
                    new OneWayFSAParameters()));


            var fsaParameters = new OverlapRemovalParameters();
            IOverlapRemovalParameters createdParameters = factory.CreateParameters(string.Empty, fsaParameters);
            Assert.IsNull(createdParameters);

            createdParameters = factory.CreateParameters("fsa", fsaParameters);
            Assert.IsNull(createdParameters);

            createdParameters = factory.CreateParameters("FSA", fsaParameters);
            Assert.IsInstanceOf<OverlapRemovalParameters>(createdParameters);
            Assert.AreNotSame(fsaParameters, createdParameters);

            createdParameters = factory.CreateParameters("FSA", null);
            Assert.IsInstanceOf<OverlapRemovalParameters>(createdParameters);
            Assert.AreNotSame(fsaParameters, createdParameters);

            var oneWayFSAParameters = new OneWayFSAParameters();
            createdParameters = factory.CreateParameters("OneWayFSA", oneWayFSAParameters);
            Assert.IsInstanceOf<OneWayFSAParameters>(createdParameters);
            Assert.AreNotSame(oneWayFSAParameters, createdParameters);

            createdParameters = factory.CreateParameters("OneWayFSA", null);
            Assert.IsInstanceOf<OneWayFSAParameters>(createdParameters);
            Assert.AreNotSame(oneWayFSAParameters, createdParameters);


            createdParameters = factory.CreateParameters("OneWayFSA", fsaParameters);
            Assert.IsInstanceOf<OneWayFSAParameters>(createdParameters);
            Assert.AreNotSame(fsaParameters, createdParameters);
            Assert.AreNotSame(oneWayFSAParameters, createdParameters);


            Assert.IsFalse(factory.IsValidAlgorithm(null));
            Assert.IsFalse(factory.IsValidAlgorithm(string.Empty));
            Assert.IsTrue(factory.IsValidAlgorithm("FSA"));
            Assert.IsTrue(factory.IsValidAlgorithm("OneWayFSA"));
            Assert.IsFalse(factory.IsValidAlgorithm("fsa"));


            var algorithm1 = new FSAAlgorithm<int, OverlapRemovalParameters>(rectangles, new OverlapRemovalParameters());
            Assert.IsEmpty(factory.GetAlgorithmType(algorithm1));

            var algorithm2 = new FSAAlgorithm<int>(rectangles, new OverlapRemovalParameters());
            Assert.AreEqual("FSA", factory.GetAlgorithmType(algorithm2));

            var algorithm3 = new OneWayFSAAlgorithm<int>(rectangles, new OneWayFSAParameters());
            Assert.AreEqual("OneWayFSA", factory.GetAlgorithmType(algorithm3));
        }

        [Test]
        public void StandardFactory_Throws()
        {
            var rectangles = new Dictionary<int, Rect>();
            var context = new TestOverlapRemovalContext(rectangles);

            var factory = new StandardOverlapRemovalAlgorithmFactory<int>();

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            // ReSharper disable AssignNullToNotNullAttribute
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, context, new OverlapRemovalParameters()));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, null, new OverlapRemovalParameters()));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, context, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(string.Empty, context, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(string.Empty, null, new OverlapRemovalParameters()));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(string.Empty, null, null));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateAlgorithm(null, null, null));
            Assert.Throws<ArgumentException>(
                () => factory.CreateAlgorithm("OneWayFSA", context, new OverlapRemovalParameters()));

            Assert.Throws<ArgumentNullException>(
                () => factory.CreateParameters(null, new OverlapRemovalParameters()));
            Assert.Throws<ArgumentNullException>(
                () => factory.CreateParameters(null, null));

            Assert.Throws<ArgumentNullException>(() => factory.GetAlgorithmType(null));
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}