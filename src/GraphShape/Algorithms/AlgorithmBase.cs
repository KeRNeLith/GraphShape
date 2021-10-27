using System;
using System.Diagnostics;
#if SUPPORTS_AGGRESSIVE_INLINING
using System.Runtime.CompilerServices;
#endif
using JetBrains.Annotations;
using QuikGraph.Algorithms;

namespace GraphShape.Algorithms
{
    /// <summary>
    /// Base class for all algorithm.
    /// </summary>
    public abstract class AlgorithmBase : IAlgorithm
    {
        #region IAlgorithm

        private int _cancelling;

        /// <summary>
        /// Gets a value indicating if a cancellation request is pending.
        /// </summary>
        protected bool IsCancelling => _cancelling > 0;

        /// <summary>
        /// Throws if a cancellation of the algorithm was requested.
        /// </summary>
        /// <exception cref="T:System.OperationCanceledException">If the algorithm <see cref="IsCancelling"/>.</exception>
#if SUPPORTS_AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        protected void ThrowIfCancellationRequested()
        {
            if (IsCancelling)
                throw new OperationCanceledException("Algorithm aborted.");
        }

        /// <inheritdoc />
        public object SyncRoot { get; } = new object();

        private volatile ComputationState _state = ComputationState.NotRunning;

        /// <inheritdoc />
        public ComputationState State
        {
            get
            {
                lock (SyncRoot)
                {
                    return _state;
                }
            }
        }

        /// <inheritdoc />
        public void Compute()
        {
            BeginComputation();

            try
            {
                Initialize();

                InternalCompute();
            }
            catch (OperationCanceledException)
            {
                // Just catch it to clean and end computing.
            }
            finally
            {
                Clean();

                EndComputation();
            }
        }

        /// <inheritdoc />
        public void Abort()
        {
            bool raise = false;
            lock (SyncRoot)
            {
                if (_state == ComputationState.Running)
                {
                    _state = ComputationState.PendingAbortion;
                    System.Threading.Interlocked.Increment(ref _cancelling);
                    raise = true;
                }
            }

            if (raise)
            {
                OnStateChanged(EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public event EventHandler StateChanged;

        /// <summary>
        /// Called on algorithm state changed.
        /// </summary>
        /// <param name="args"><see cref="F:EventArgs.Empty"/>.</param>
        protected virtual void OnStateChanged([NotNull] EventArgs args)
        {
            Debug.Assert(args != null);

            StateChanged?.Invoke(this, args);
        }

        /// <inheritdoc />
        public event EventHandler Started;

        /// <summary>
        /// Called on algorithm start.
        /// </summary>
        /// <param name="args"><see cref="F:EventArgs.Empty"/>.</param>
        protected virtual void OnStarted([NotNull] EventArgs args)
        {
            Debug.Assert(args != null);

            Started?.Invoke(this, args);
        }

        /// <inheritdoc />
        public event EventHandler Finished;

        /// <summary>
        /// Called on algorithm finished.
        /// </summary>
        /// <param name="args"><see cref="F:EventArgs.Empty"/>.</param>
        protected virtual void OnFinished([NotNull] EventArgs args)
        {
            Debug.Assert(args != null);

            Finished?.Invoke(this, args);
        }

        /// <inheritdoc />
        public event EventHandler Aborted;

        /// <summary>
        /// Called on algorithm abort.
        /// </summary>
        /// <param name="args"><see cref="F:EventArgs.Empty"/>.</param>
        protected virtual void OnAborted([NotNull] EventArgs args)
        {
            Debug.Assert(args != null);

            Aborted?.Invoke(this, args);
        }

        #endregion

        private void BeginComputation()
        {
            Debug.Assert(
                State == ComputationState.NotRunning
                || State == ComputationState.Finished
                || State == ComputationState.Aborted);

            lock (SyncRoot)
            {
                _state = ComputationState.Running;
                _cancelling = 0;
                OnStarted(EventArgs.Empty);
                OnStateChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called on algorithm initialization step.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Algorithm compute step.
        /// </summary>
        protected abstract void InternalCompute();

        /// <summary>
        /// Called on algorithm cleanup step.
        /// </summary>
        protected virtual void Clean()
        {
        }

        private void EndComputation()
        {
            Debug.Assert(
                State == ComputationState.Running
                ||
                State == ComputationState.PendingAbortion);

            lock (SyncRoot)
            {
                switch (_state)
                {
                    case ComputationState.Running:
                        _state = ComputationState.Finished;
                        OnFinished(EventArgs.Empty);
                        break;
                    case ComputationState.PendingAbortion:
                        _state = ComputationState.Aborted;
                        OnAborted(EventArgs.Empty);
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                _cancelling = 0;
                OnStateChanged(EventArgs.Empty);
            }
        }
    }
}