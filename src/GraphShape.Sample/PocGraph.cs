using QuickGraph;

namespace GraphSharp.Sample
{
	public class PocGraph : BidirectionalGraph<PocVertex, PocEdge>
	{
		public PocGraph() { }

		public PocGraph(bool allowParallelEdges)
			: base(allowParallelEdges) { }

		public PocGraph(bool allowParallelEdges, int vertexCapacity)
			: base(allowParallelEdges, vertexCapacity) { }
	}
}