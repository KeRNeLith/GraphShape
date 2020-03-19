using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
    public partial class SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        private class SugiEdge : TypedEdge<SugiVertex>
        {
            [NotNull]
            public TEdge Original { get; }

            public bool IsReverted => !Original.Equals(default(TEdge))
                                      && Original.Source == Target.Original
                                      && Original.Target == Source.Original;

            public bool IsLongEdge
            {
                get => DummyVertices != null;
                set
                {
                    if (IsLongEdge == value)
                        return;
                    DummyVertices = value ? new List<SugiVertex>() : null;
                }
            }

            [ItemNotNull]
            public IList<SugiVertex> DummyVertices { get; private set; }

            public SugiEdge(
                [NotNull] TEdge original,
                [NotNull] SugiVertex source,
                [NotNull] SugiVertex target,
                EdgeTypes type)
                : base(source, target, type)
            {
                Original = original;
            }
        }
    }
}