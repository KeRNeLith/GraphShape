using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Compound
{
    public class TestingCompoundLayoutIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>
        : CompoundLayoutIterationEventArgs<TVertex, TEdge>, ILayoutInfoIterationEventArgs<TVertex, TEdge, TVertexInfo, TEdgeInfo>
        where TVertex : class 
        where TEdge : IEdge<TVertex>
    {
        private IDictionary<TVertex, TVertexInfo> vertexInfos;

        public Point GravitationCenter { get; private set; }

        public TestingCompoundLayoutIterationEventArgs(
            int iteration, 
            double statusInPercent, 
            string message, 
            IDictionary<TVertex, Point> verticesPositions, 
            IDictionary<TVertex, Size> innerCanvasSizes,
            IDictionary<TVertex, TVertexInfo> vertexInfos,
            Point gravitationCenter) 
            : base(iteration, statusInPercent, message, verticesPositions, innerCanvasSizes)
        {
            this.vertexInfos = vertexInfos;
            this.GravitationCenter = gravitationCenter;
        }

        public override object GetVertexInfo(TVertex vertex)
        {
            TVertexInfo info = default(TVertexInfo);
            if (vertexInfos.TryGetValue(vertex, out info))
                return info;

            return null;
        }

        public IDictionary<TVertex, TVertexInfo> VerticesInfos
        {
            get { return this.vertexInfos; }
        }

        public IDictionary<TEdge, TEdgeInfo> EdgesInfos
        {
            get { return null; }
        }
    }
}
