using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphShape.Algorithms.OverlapRemoval
{
    /// <summary>
    /// Simple implementation of an overlap removal algorithm factory.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    public class StandardOverlapRemovalAlgorithmFactory<TVertex> : IOverlapRemovalAlgorithmFactory<TVertex>
    {
        [NotNull]
        private const string FSAAlgorithm = "FSA";
        [NotNull]
        private const string OneWayFSAAlgorithm = "OneWayFSA";

        /// <inheritdoc />
        public IEnumerable<string> AlgorithmTypes { get; } = new[] { FSAAlgorithm, OneWayFSAAlgorithm };

        /// <inheritdoc />
        public IOverlapRemovalAlgorithm<TVertex> CreateAlgorithm(
            string algorithmType,
            IOverlapRemovalContext<TVertex> context,
            IOverlapRemovalParameters parameters)
        {
            if (algorithmType is null)
                throw new ArgumentNullException(nameof(algorithmType));
            if (context is null)
                throw new ArgumentNullException(nameof(context));
            if (parameters is null)
                throw new ArgumentNullException(nameof(parameters));

            switch (algorithmType)
            {
                case FSAAlgorithm:
                    return new FSAAlgorithm<TVertex>(context.Rectangles, parameters);
                case OneWayFSAAlgorithm:
                    {
                        if (parameters is OneWayFSAParameters oneWayFSAParams)
                            return new OneWayFSAAlgorithm<TVertex>(context.Rectangles, oneWayFSAParams);

                        throw new ArgumentException(
                            $"Must use {nameof(OneWayFSAParameters)} to create a {nameof(OneWayFSAAlgorithm<object>)} algorithm.",
                            nameof(parameters));
                    }
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        public IOverlapRemovalParameters CreateParameters(string algorithmType, IOverlapRemovalParameters oldParameters)
        {
            if (algorithmType is null)
                throw new ArgumentNullException(nameof(algorithmType));

            switch (algorithmType)
            {
                case FSAAlgorithm:
                    return oldParameters is OverlapRemovalParameters overlapRemovalParams
                        ? (IOverlapRemovalParameters)(overlapRemovalParams).Clone()
                        : new OverlapRemovalParameters();
                case OneWayFSAAlgorithm:
                    return oldParameters is OneWayFSAParameters oneWayFSAParams
                        ? (IOverlapRemovalParameters)oneWayFSAParams.Clone()
                        : new OneWayFSAParameters();
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        public bool IsValidAlgorithm(string algorithmType)
        {
            return AlgorithmTypes.Contains(algorithmType);
        }

        /// <inheritdoc />
        public string GetAlgorithmType(IOverlapRemovalAlgorithm<TVertex> algorithm)
        {
            if (algorithm is null)
                throw new ArgumentNullException(nameof(algorithm));
            if (algorithm is FSAAlgorithm<TVertex>)
                return FSAAlgorithm;
            if (algorithm is OneWayFSAAlgorithm<TVertex>)
                return OneWayFSAAlgorithm;
            return string.Empty;
        }
    }
}