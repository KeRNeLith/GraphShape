using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Simple.Tree
{
    public partial class SimpleTreeLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        private class Layer
        {
            public double Size { get; set; }

            public double NextPosition { get; set; }

            [NotNull, ItemNotNull]
            public IList<TVertex> Vertices { get; } = new List<TVertex>();

            public double LastTranslate { get; set; }

            public Layer()
            {
                LastTranslate = 0;
            }

            // Width and Height Optimization

        }

        private class VertexData
        {
            [CanBeNull]
            public TVertex Parent { get; set; }

            public double Translate { get; set; }

            public double Position { get; set; }

            // Width and Height Optimization

        }
    }
}
