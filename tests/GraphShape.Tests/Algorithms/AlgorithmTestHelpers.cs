using System.Collections.Generic;
using System.Windows;
using GraphShape.Algorithms;
using GraphShape.Algorithms.Layout;
using JetBrains.Annotations;
using NUnit.Framework;
using QuikGraph;
using QuikGraph.Algorithms;

namespace GraphShape.Tests.Algorithms
{
    /// <summary>
    /// Test helpers for algorithms.
    /// </summary>
    internal static class AlgorithmTestHelpers
    {
        public static void AssertAlgorithmState(
            [NotNull] AlgorithmBase algorithm,
            ComputationState state = ComputationState.NotRunning)
        {
            Assert.IsNotNull(algorithm.SyncRoot);
            Assert.AreEqual(state, algorithm.State);
        }

        public static void AssertAlgorithmState<TGraph>(
            [NotNull] AlgorithmBase<TGraph> algorithm,
            [NotNull] TGraph treatedGraph,
            ComputationState state = ComputationState.NotRunning)
        {
            Assert.IsNotNull(treatedGraph);
            Assert.AreSame(treatedGraph, algorithm.VisitedGraph);
            Assert.IsNotNull(algorithm.Services);
            Assert.IsNotNull(algorithm.SyncRoot);
            Assert.AreEqual(state, algorithm.State);
        }

        public static void AssertAlgorithmProperties<TVertex, TEdge, TGraph, TParameters>(
            [NotNull] ParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, TParameters> algorithm,
            [NotNull] TGraph treatedGraph,
            [CanBeNull] IDictionary<TVertex, Point> positions = null,
            bool expectedReportIterationEnd = false,
            bool expectedReportProgress = false,
            [CanBeNull] TParameters parameters = null)
            where TEdge : IEdge<TVertex>
            where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
            where TParameters : class, ILayoutParameters
        {
            AssertAlgorithmState(algorithm);
            Assert.AreSame(treatedGraph, algorithm.VisitedGraph);
            if (positions is null)
                Assert.IsNotNull(algorithm.VerticesPositions);
            else
                CollectionAssert.AreEqual(positions, algorithm.VerticesPositions);
            Assert.AreEqual(expectedReportIterationEnd, algorithm.ReportOnIterationEndNeeded);
            Assert.AreEqual(expectedReportProgress, algorithm.ReportOnProgressChangedNeeded);
            if (parameters is null)
            {
                Assert.IsNotNull(algorithm.Parameters);
                Assert.AreSame(algorithm.Parameters, algorithm.GetParameters());
            }
            else
            {
                Assert.AreEqual(parameters, algorithm.Parameters);
                Assert.AreSame(algorithm.Parameters, algorithm.GetParameters());
            }
        }

        public static void AssertAlgorithmProperties<TVertex, TEdge, TGraph, TParameters>(
            [NotNull] DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, TParameters> algorithm,
            [NotNull] TGraph treatedGraph,
            [CanBeNull] IDictionary<TVertex, Point> positions = null,
            bool expectedReportIterationEnd = false,
            bool expectedReportProgress = false,
            [CanBeNull] TParameters parameters = null)
            where TEdge : IEdge<TVertex>
            where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
            where TParameters : class, ILayoutParameters, new()
        {
            AssertAlgorithmState(algorithm);
            Assert.AreSame(treatedGraph, algorithm.VisitedGraph);
            if (positions is null)
                Assert.IsNotNull(algorithm.VerticesPositions);
            else
                CollectionAssert.AreEqual(positions, algorithm.VerticesPositions);
            Assert.AreEqual(expectedReportIterationEnd, algorithm.ReportOnIterationEndNeeded);
            Assert.AreEqual(expectedReportProgress, algorithm.ReportOnProgressChangedNeeded);
            if (parameters is null)
            {
                Assert.IsNotNull(algorithm.Parameters);
                Assert.AreSame(algorithm.Parameters, algorithm.GetParameters());
            }
            else
            {
                Assert.AreEqual(parameters, algorithm.Parameters);
                Assert.AreSame(algorithm.Parameters, algorithm.GetParameters());
            }
        }

        public static void AssertAlgorithmProperties<TVertex, TEdge, TGraph, TVertexInfo, TEdgeInfo, TParameters>(
            [NotNull] ParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, TVertexInfo, TEdgeInfo, TParameters> algorithm,
            [NotNull] TGraph treatedGraph,
            [CanBeNull] IDictionary<TVertex, Point> positions = null,
            bool expectedReportIterationEnd = false,
            bool expectedReportProgress = false,
            [CanBeNull] TParameters parameters = null)
            where TEdge : IEdge<TVertex>
            where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
            where TParameters : class, ILayoutParameters
        {
            AssertAlgorithmState(algorithm);
            Assert.AreSame(treatedGraph, algorithm.VisitedGraph);
            if (positions is null)
                Assert.IsNotNull(algorithm.VerticesPositions);
            else
                CollectionAssert.AreEqual(positions, algorithm.VerticesPositions);
            Assert.AreEqual(expectedReportIterationEnd, algorithm.ReportOnIterationEndNeeded);
            Assert.AreEqual(expectedReportProgress, algorithm.ReportOnProgressChangedNeeded);
            if (parameters is null)
            {
                Assert.IsNotNull(algorithm.Parameters);
                Assert.AreSame(algorithm.Parameters, algorithm.GetParameters());
            }
            else
            {
                Assert.AreEqual(parameters, algorithm.Parameters);
                Assert.AreSame(algorithm.Parameters, algorithm.GetParameters());
            }
            CollectionAssert.IsEmpty(algorithm.VerticesInfos);
            CollectionAssert.IsEmpty(algorithm.EdgesInfos);
        }
    }
}
