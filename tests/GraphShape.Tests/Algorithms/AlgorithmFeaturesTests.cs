#if SUPPORTS_TASKS
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphShape.Algorithms;
using JetBrains.Annotations;
using QuikGraph.Algorithms;
using NUnit.Framework;

namespace GraphShape.Tests.Algorithms
{
    /// <summary>
    /// Tests related to algorithm features (state).
    /// </summary>
    [TestFixture]
    internal class AlgorithmFeaturesTests
    {
        #region Test classes

        private static readonly TimeSpan TimeoutDelay = TimeSpan.FromSeconds(5);

        private class ManageableTestAlgorithm : AlgorithmBase
        {
            [NotNull]
            public ManualResetEvent InitializeEvent { get; }
            [NotNull]
            public ManualResetEvent InitializedEvent { get; } = new ManualResetEvent(false);

            [NotNull]
            public ManualResetEvent ComputeEvent { get; }
            [NotNull]
            public ManualResetEvent ComputedEvent { get; } = new ManualResetEvent(false);

            [NotNull]
            public ManualResetEvent CleanEvent { get; }
            [NotNull]
            public ManualResetEvent CleanedEvent { get; } = new ManualResetEvent(false);

            public ManageableTestAlgorithm(
                [NotNull] ManualResetEvent initialize,
                [NotNull] ManualResetEvent compute,
                [NotNull] ManualResetEvent clean)
            {
                InitializeEvent = initialize;
                ComputeEvent = compute;
                CleanEvent = clean;
            }

            protected override void Initialize()
            {
                InitializeEvent.Set();
                InitializedEvent.WaitOne(TimeoutDelay);
                ThrowIfCancellationRequested();
            }

            protected override void InternalCompute()
            {
                ComputeEvent.Set();
                ComputedEvent.WaitOne(TimeoutDelay);
                ThrowIfCancellationRequested();
            }

            protected override void Clean()
            {
                CleanEvent.Set();
                CleanedEvent.WaitOne(TimeoutDelay);
            }
        }

        #endregion

        [Test]
        public void AlgorithmNormalStates()
        {
            var initialize = new ManualResetEvent(false);
            var compute = new ManualResetEvent(false);
            var clean = new ManualResetEvent(false);
            var finished = new ManualResetEvent(false);

            var algorithm = new ManageableTestAlgorithm(initialize, compute, clean);

            bool hasStarted = false;
            bool hasFinished = false;
            var expectedStates = new Queue<ComputationState>();
            expectedStates.Enqueue(ComputationState.Running);
            expectedStates.Enqueue(ComputationState.Finished);

            algorithm.Started += (sender, args) =>
            {
                if (hasStarted)
                    Assert.Fail($"{nameof(AlgorithmBase<object>.Started)} event called twice.");
                hasStarted = true;
            };
            algorithm.Finished += (sender, args) =>
            {
                if (hasFinished)
                    Assert.Fail($"{nameof(AlgorithmBase<object>.Finished)} event called twice.");
                hasFinished = true;
                finished.Set();
            };
            algorithm.StateChanged += (sender, args) =>
            {
                Assert.AreEqual(expectedStates.Peek(), algorithm.State);
                expectedStates.Dequeue();
            };
            algorithm.Aborted += (sender, args) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.Aborted)} event called.");
            };

            Assert.AreEqual(ComputationState.NotRunning, algorithm.State);

            // Run the algorithm
            Task.Run(() =>
            {
                Assert.DoesNotThrow(algorithm.Compute);
            });

            initialize.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Running, algorithm.State);
            Assert.IsTrue(hasStarted);

            algorithm.InitializedEvent.Set();

            compute.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Running, algorithm.State);

            algorithm.ComputedEvent.Set();

            clean.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Running, algorithm.State);

            algorithm.CleanedEvent.Set();

            finished.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Finished, algorithm.State);
            Assert.IsTrue(hasFinished);
            Assert.IsTrue(expectedStates.Count == 0);
        }

        [Test]
        public void AlgorithmStates_AbortBeforeStart()
        {
            var initialize = new ManualResetEvent(true);
            var compute = new ManualResetEvent(true);
            var clean = new ManualResetEvent(true);
            var end = new ManualResetEvent(false);

            var algorithm = new ManageableTestAlgorithm(initialize, compute, clean);

            algorithm.Started += (sender, args) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.Started)} event called.");
            };
            algorithm.Finished += (sender, args) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.Finished)} event called.");
            };
            algorithm.StateChanged += (sender, args) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.StateChanged)} event called.");
            };
            algorithm.Aborted += (sender, args) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.Aborted)} event called.");
            };

            Assert.AreEqual(ComputationState.NotRunning, algorithm.State);

            // Abort the algorithm
            Task.Run(() =>
            {
                Assert.DoesNotThrow(() =>
                {
                    algorithm.Abort();
                    Assert.AreEqual(ComputationState.NotRunning, algorithm.State);
                    end.Set();
                });
            });

            end.WaitOne(TimeoutDelay);
        }

        [Test]
        public void AlgorithmStates_AbortDuringRun()
        {
            var initialize = new ManualResetEvent(false);
            var compute = new ManualResetEvent(false);
            var clean = new ManualResetEvent(false);
            var aborted = new ManualResetEvent(false);

            var algorithm = new ManageableTestAlgorithm(initialize, compute, clean);

            bool hasStarted = false;
            bool hasAborted = false;
            var expectedStates = new Queue<ComputationState>();
            expectedStates.Enqueue(ComputationState.Running);
            expectedStates.Enqueue(ComputationState.PendingAbortion);
            expectedStates.Enqueue(ComputationState.Aborted);

            algorithm.Started += (sender, args) =>
            {
                if (hasStarted)
                    Assert.Fail($"{nameof(AlgorithmBase<object>.Started)} event called twice.");
                hasStarted = true;
            };
            algorithm.Finished += (sender, args) =>
            {
                Assert.Fail($"{nameof(AlgorithmBase<object>.Finished)} event called.");
            };
            algorithm.StateChanged += (sender, args) =>
            {
                Assert.AreEqual(expectedStates.Peek(), algorithm.State);
                expectedStates.Dequeue();
            };
            algorithm.Aborted += (sender, args) =>
            {
                if (hasAborted)
                    Assert.Fail($"{nameof(AlgorithmBase<object>.Aborted)} event called twice.");
                hasAborted = true;
                aborted.Set();
            };

            Assert.AreEqual(ComputationState.NotRunning, algorithm.State);

            // Run the algorithm
            Task.Run(() =>
            {
                Assert.DoesNotThrow(algorithm.Compute);
            });

            initialize.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Running, algorithm.State);
            Assert.IsTrue(hasStarted);

            algorithm.InitializedEvent.Set();

            compute.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Running, algorithm.State);

            algorithm.Abort();
            algorithm.ComputedEvent.Set();

            Assert.AreEqual(ComputationState.PendingAbortion, algorithm.State);

            algorithm.CleanedEvent.Set();

            aborted.WaitOne(TimeoutDelay);
            Assert.AreEqual(ComputationState.Aborted, algorithm.State);
            Assert.IsTrue(hasAborted);
            Assert.IsTrue(expectedStates.Count == 0);
        }
    }
}
#endif