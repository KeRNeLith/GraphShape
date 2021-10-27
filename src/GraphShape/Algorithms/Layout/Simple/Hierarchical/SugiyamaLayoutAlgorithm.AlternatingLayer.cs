using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    public partial class SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// List of alternating <see cref="SegmentContainer"/> and <see cref="SugiVertex"/>.
        /// </summary>
        protected class AlternatingLayer : List<IData>, ICloneable
        {
            /// <summary>
            /// This method ensures that the layer is a real alternating
            /// layer: starts with a <see cref="SegmentContainer"/> followed by a Vertex,
            /// another <see cref="SegmentContainer"/>, another Vertex, ... ending with 
            /// a <see cref="SegmentContainer"/>.
            /// </summary>
            public void EnsureAlternatingAndPositions()
            {
                bool shouldBeAContainer = true;
                for (int i = 0; i < Count; ++i, shouldBeAContainer = !shouldBeAContainer)
                {
                    if (shouldBeAContainer && this[i] is SugiVertex)
                    {
                        Insert(i, new SegmentContainer());
                    }
                    else
                    {
                        while (i < Count
                               && !shouldBeAContainer
                               && this[i] is SegmentContainer actualContainer)
                        {
                            // The previous one must be a container too
                            var prevContainer = (SegmentContainer)this[i - 1];
                            prevContainer.Join(actualContainer);
                            RemoveAt(i);
                        }

                        if (i >= Count)
                            break;
                    }
                }

                if (shouldBeAContainer)
                {
                    // The last element in the alternating layer 
                    // should be a container, but it's not
                    // So add an empty one
                    Add(new SegmentContainer());
                }
            }

            /// <summary>
            /// Ensures items positions.
            /// </summary>
            protected void EnsurePositions()
            {
                // Assign positions to vertices on the actualLayer (L_i)
                for (int i = 1; i < Count; i += 2)
                {
                    var precedingContainer = (SegmentContainer)this[i - 1];
                    var vertex = (SugiVertex)this[i];
                    if (i == 1)
                    {
                        vertex.Position = precedingContainer.Count;
                    }
                    else
                    {
                        var previousVertex = (SugiVertex)this[i - 2];
                        vertex.Position = previousVertex.Position + precedingContainer.Count + 1;
                    }
                }

                // Assign positions to containers on the actualLayer (L_i+1)
                for (int i = 0; i < Count; i += 2)
                {
                    var container = (SegmentContainer)this[i];
                    if (i == 0)
                    {
                        container.Position = 0;
                    }
                    else
                    {
                        var precedingVertex = (SugiVertex)this[i - 1];
                        container.Position = precedingVertex.Position + 1;
                    }
                }
            }

            /// <summary>
            /// Sets positions to list items.
            /// </summary>
            public void SetPositions()
            {
                int nextPosition = 0;
                for (int i = 0; i < Count; ++i)
                {
                    var segmentContainer = this[i] as SegmentContainer;
                    var vertex = this[i] as SugiVertex;
                    if (segmentContainer != null)
                    {
                        segmentContainer.Position = nextPosition;
                        nextPosition += segmentContainer.Count;
                    }
                    else if (vertex != null)
                    {
                        vertex.Position = nextPosition;
                        nextPosition += 1;
                    }
                }
            }

            /// <inheritdoc cref="ICloneable.Clone"/>
            [Pure]
            [NotNull]
            public AlternatingLayer Clone()
            {
                var clonedLayer = new AlternatingLayer();
                foreach (IData item in this)
                {
                    if (item is ICloneable cloneable)
                        clonedLayer.Add(cloneable.Clone() as IData);
                    else
                        clonedLayer.Add(item);
                }
                return clonedLayer;
            }

            #region ICloneable

            /// <inheritdoc />
            object ICloneable.Clone()
            {
                return Clone();
            }

            #endregion
        }
    }
}