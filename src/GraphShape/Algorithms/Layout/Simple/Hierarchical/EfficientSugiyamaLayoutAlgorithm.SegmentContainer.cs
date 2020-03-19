using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
    public partial class EfficientSugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Represents a <see cref="Segment"/> container.
        /// </summary>
        protected interface ISegmentContainer : IEnumerable<Segment>, IData, ICloneable
        {
            /// <summary>
            /// Appends the segment <paramref name="segment"/> to the end of this container.
            /// </summary>
            /// <param name="segment">The segment to append.</param>
            void Append([NotNull] Segment segment);

            /// <summary>
            /// Appends all elements of the <paramref name="container"/> to this container.
            /// </summary>
            /// <param name="container">Container to append.</param>
            void Join([NotNull] ISegmentContainer container);

            /// <summary>
            /// Splits this container at segment <paramref name="segment"/> into two containers
            /// <paramref name="container1"/> and <paramref name="container2"/>. 
            /// All elements less than <paramref name="segment"/> are stored in container <paramref name="container1"/> and
            /// those who are greater than <paramref name="segment"/> in <paramref name="container2"/>.
            /// Element <paramref name="segment"/> is neither in <paramref name="container1"/> or 
            /// <paramref name="container2"/>.
            /// </summary>
            /// <param name="segment">The segment to split at.</param>
            /// <param name="container1">The container which contains the elements before <paramref name="segment"/>.</param>
            /// <param name="container2">The container which contains the elements after <paramref name="segment"/>.</param>
            void Split(
                [NotNull] Segment segment,
                [NotNull] out ISegmentContainer container1,
                [NotNull] out ISegmentContainer container2);

            /// <summary>
            /// Splits this container at position <paramref name="k"/>. The first <paramref name="k"/>
            /// elements of the container are stored in <paramref name="container1"/> and the remainder
            /// in <paramref name="container2"/>.
            /// </summary>
            /// <param name="k">The index where the container should be split.</param>
            /// <param name="container1">The container which contains the elements before <paramref name="k"/>.</param>
            /// <param name="container2">The container which contains the elements after <paramref name="k"/>.</param>
            void Split(
                int k,
                [NotNull] out ISegmentContainer container1,
                [NotNull] out ISegmentContainer container2);

            /// <summary>
            /// Element count.
            /// </summary>
            int Count { get; }
        }

        // TODO: Implement it with a SplayTree
        // Info could be found at:
        // http://en.wikipedia.org/wiki/Splay_tree
        //
        // Implementation that could be ported can be found at:
        // http://www.link.cs.cmu.edu/link/ftp-site/splaying/SplayTree.java
        /// <summary>
        /// <see cref="Segment"/> container.
        /// </summary>
        protected class SegmentContainer : List<Segment>, ISegmentContainer
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SegmentContainer"/> class.
            /// </summary>
            public SegmentContainer()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SegmentContainer"/> class.
            /// </summary>
            /// <param name="capacity">Container capacity.</param>
            public SegmentContainer(int capacity)
                : base(capacity)
            {
            }

            #region ISegmentContainer

            /// <inheritdoc />
            public void Append(Segment segment)
            {
                Debug.Assert(segment != null);

                Add(segment);
            }

            /// <inheritdoc />
            public void Join(ISegmentContainer container)
            {
                Debug.Assert(container != null);

                AddRange(container);
            }

            /// <inheritdoc />
            public void Split(Segment segment, out ISegmentContainer container1, out ISegmentContainer container2)
            {
                Debug.Assert(segment != null);

                int index = IndexOf(segment);
                Split(index, out container1, out container2, false);
            }

            /// <inheritdoc />
            public void Split(int k, out ISegmentContainer container1, out ISegmentContainer container2)
            {
                Split(k, out container1, out container2, true);
            }

            /// <summary>
            /// Splits this container at position <paramref name="k"/>. The first <paramref name="k"/>
            /// elements of the container are stored in <paramref name="container1"/> and the remainder
            /// in <paramref name="container2"/>.
            /// </summary>
            /// <param name="k">The index where the container should be split.</param>
            /// <param name="container1">The container which contains the elements before <paramref name="k"/>.</param>
            /// <param name="container2">The container which contains the elements after <paramref name="k"/>.</param>
            /// <param name="keep">Indicates if <paramref name="k"/>th item should be kept in <paramref name="container1"/> or not.</param>
            protected void Split(
                int k,
                [NotNull] out ISegmentContainer container1,
                [NotNull] out ISegmentContainer container2,
                bool keep)
            {
                Debug.Assert(k < Count);

                int container1Count = k + (keep ? 1 : 0);
                int container2Count = Count - k - 1;

                container1 = new SegmentContainer(container1Count);
                container2 = new SegmentContainer(container2Count);

                for (int i = 0; i < container1Count; ++i)
                    container1.Append(this[i]);

                for (int i = k + 1; i < Count; ++i)
                    container2.Append(this[i]);
            }

            #endregion

            #region IData

            // TODO: Get them from the first element of the container, MAYBE!
            /// <inheritdoc />
            public int Position { get; set; }

            #endregion

            #region ICloneable

            /// <inheritdoc />
            public object Clone()
            {
                return MemberwiseClone();
            }

            #endregion
        }
    }
}
