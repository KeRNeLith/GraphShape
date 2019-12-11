using QuikGraph;

namespace GraphShape.Algorithms.Highlight
{
	public interface IHighlightContext<TVertex, TEdge, TGraph>
		where TEdge : IEdge<TVertex>
		where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
	{
		TGraph Graph { get; }
	}
}