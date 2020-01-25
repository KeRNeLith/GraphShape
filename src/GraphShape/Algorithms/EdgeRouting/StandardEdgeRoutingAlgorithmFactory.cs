using System;
using System.Collections.Generic;
using System.Linq;
using GraphShape.Algorithms.Layout;
using QuikGraph;

namespace GraphShape.Algorithms.EdgeRouting
{
    /// <summary>
    /// Dummy implementation of edge routing algorithm factory.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    /// <typeparam name="TGraph">Graph type.</typeparam>
    public class StandardEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph> : IEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        /// <inheritdoc />
        public IEnumerable<string> AlgorithmTypes { get; } = Enumerable.Empty<string>();

        /// <inheritdoc />
        public IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph> CreateAlgorithm(string algorithmType, ILayoutContext<TVertex, TEdge, TGraph> context, IEdgeRoutingParameters parameters)
        {
            if (algorithmType is null)
                throw new ArgumentNullException(nameof(algorithmType));
            if (context is null)
                throw new ArgumentNullException(nameof(context));
            return null;
        }

        /// <inheritdoc />
        public IEdgeRoutingParameters CreateParameters(string algorithmType, IEdgeRoutingParameters oldParameters)
        {
            if (algorithmType is null)
                throw new ArgumentNullException(nameof(algorithmType));
            return new EdgeRoutingParameters();
        }

        /// <inheritdoc />
        public bool IsValidAlgorithm(string algorithmType)
        {
            return AlgorithmTypes.Any(type => type == algorithmType);
        }

        /// <inheritdoc />
        public string GetAlgorithmType(IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph> algorithm)
        {
            if (algorithm is null)
                throw new ArgumentNullException(nameof(algorithm));
            return string.Empty;
        }
    }
}