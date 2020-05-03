using System.Diagnostics;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
    public partial class SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        /// <summary>
        /// Sugiyama internal graph edge.
        /// </summary>
        protected class SugiEdge : TaggedEdge<SugiVertex, TEdge>
        {
            /// <summary>
            /// <see cref="Marked"/> backup value.
            /// </summary>
            private bool _tempMark;

            /// <summary>
            /// Initializes a new instance of the <see cref="SugiEdge"/> class.
            /// </summary>
            /// <param name="originalEdge">Original edge (tag).</param>
            /// <param name="source">The source vertex.</param>
            /// <param name="target">The target vertex.</param>
            public SugiEdge([CanBeNull] TEdge originalEdge, [NotNull] SugiVertex source, [NotNull] SugiVertex target)
                : base(source, target, originalEdge)
            {
            }

            /// <summary>
            /// Gets the original edge of this <see cref="SugiEdge"/>.
            /// </summary>
            public TEdge OriginalEdge => Tag;

            /// <summary>
            /// Gets or sets that the edge is included in a 
            /// type 1 conflict as a non-inner segment (true) or not (false).
            /// </summary>
            public bool Marked { get; set; }

            /// <summary>
            /// Backup <see cref="Marked"/> into <see cref="_tempMark"/>.
            /// </summary>
            public void SaveMarkedToTemp()
            {
                _tempMark = Marked;
            }

            /// <summary>
            /// Restore <see cref="Marked"/> from <see cref="_tempMark"/>.
            /// </summary>
            public void LoadMarkedFromTemp()
            {
                Marked = _tempMark;
            }
        }

        /// <summary>
        /// Enumeration of possible vertex types.
        /// </summary>
        protected enum VertexTypes
        {
            /// <summary>
            /// Original vertex.
            /// </summary>
            Original,

            /// <summary>
            /// P vertex.
            /// </summary>
            PVertex,

            /// <summary>
            /// Q vertex.
            /// </summary>
            QVertex,

            /// <summary>
            /// R vertex.
            /// </summary>
            RVertex
        }

        /// <summary>
        /// Enumeration of possible edge types.
        /// </summary>
        protected enum EdgeTypes
        {
            /// <summary>
            /// Non inner segment.
            /// </summary>
            NonInnerSegment,

            /// <summary>
            /// Inner segment.
            /// </summary>
            InnerSegment
        }

        /// <summary>
        /// Represents an indexed data.
        /// </summary>
        protected interface IData
        {
            /// <summary>
            /// Data position.
            /// </summary>
            int Position { get; set; }
        }

        /// <summary>
        /// Base class for any <see cref="IData"/>.
        /// </summary>
        protected abstract class Data : IData
        {
            /// <inheritdoc />
            public int Position { get; set; }


            // Used by slice assignment
            /// <summary>
            /// Sinks.
            /// </summary>
            [NotNull, ItemCanBeNull]
            public readonly Data[] Sinks = new Data[4];

            /// <summary>
            /// Shifts
            /// </summary>
            [NotNull]
            public readonly double[] Shifts =
            {
                double.PositiveInfinity,
                double.PositiveInfinity,
                double.PositiveInfinity,
                double.PositiveInfinity
            };
        }

        /// <summary>
        /// Sugiyama internal graph vertex.
        /// </summary>
        [DebuggerDisplay("{" + nameof(Type) + "}: " +
                         "{" + nameof(OriginalVertex) + "} - {" + nameof(Position) + "} ; " +
                         "{" + nameof(MeasuredPosition) + "} on layer {" + nameof(LayerIndex) + "}")]
        protected class SugiVertex : Data
        {
            private int _tempPosition;

            /// <summary>
            /// Vertex from the original graph to layout.
            /// </summary>
            public readonly TVertex OriginalVertex;

            /// <summary>
            /// Vertex type.
            /// </summary>
            public VertexTypes Type { get; }

            /// <summary>
            /// Vertex size.
            /// </summary>
            public readonly Size Size;

            /// <summary>
            /// Attached segment.
            /// </summary>
            public Segment Segment { get; set; }

            /// <summary>
            /// Layer index.
            /// </summary>
            public int LayerIndex { get; set; }

            /// <summary>
            /// The index inside the layer.
            /// </summary>
            public int IndexInsideLayer { get; set; }

            /// <summary>
            /// Measured position.
            /// </summary>
            public double MeasuredPosition { get; set; }

            /// <summary>
            /// Vertex layer position.
            /// </summary>
            public double LayerPosition { get; set; } = double.NaN;

            /// <summary>
            /// Vertex slice position.
            /// </summary>
            public double SlicePosition { get; set; } = double.NaN;

            /// <summary>
            /// Root vertices.
            /// </summary>
            [NotNull, ItemCanBeNull]
            public readonly SugiVertex[] Roots = new SugiVertex[4];

            /// <summary>
            /// Align vertices..
            /// </summary>
            [NotNull, ItemCanBeNull]
            public readonly SugiVertex[] Aligns = new SugiVertex[4];

            /// <summary>
            /// Block widths.
            /// </summary>
            [NotNull]
            public readonly double[] BlockWidths = { double.NaN, double.NaN, double.NaN, double.NaN };

            /// <summary>
            /// Slice positions.
            /// </summary>
            [NotNull]
            public readonly double[] SlicePositions = { double.NaN, double.NaN, double.NaN, double.NaN };

            /// <summary>
            /// Permutation index.
            /// </summary>
            public int PermutationIndex { get; set; }

            /// <summary>
            /// Indicates if the vertex must be optimized or not.
            /// </summary>
            public bool DoNotOptimize { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="SugiVertex"/> class.
            /// </summary>
            public SugiVertex()
                : this(VertexTypes.Original)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SugiVertex"/> class.
            /// </summary>
            /// <param name="type">Vertex type.</param>
            public SugiVertex(VertexTypes type)
                : this(null, type, default(Size))
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SugiVertex"/> class.
            /// </summary>
            /// <param name="originalVertex">Wrapped vertex.</param>
            /// <param name="size">Vertex size.</param>
            public SugiVertex(TVertex originalVertex, Size size)
                : this(originalVertex, VertexTypes.Original, size)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SugiVertex"/> class.
            /// </summary>
            /// <param name="originalVertex">Wrapped vertex.</param>
            /// <param name="type">Vertex type.</param>
            /// <param name="size">Vertex size.</param>
            public SugiVertex(TVertex originalVertex, VertexTypes type, Size size)
            {
                OriginalVertex = originalVertex;
                Type = type;
                Size = size;
                Segment = null;
            }

            /// <summary>
            /// Backup <see cref="Data.Position"/> into <see cref="_tempPosition"/>.
            /// </summary>
            public void SavePositionToTemp()
            {
                _tempPosition = Position;
            }

            /// <summary>
            /// Restore <see cref="Data.Position"/> from <see cref="_tempPosition"/>.
            /// </summary>
            public void LoadPositionFromTemp()
            {
                Position = _tempPosition;
            }
        }

        /// <summary>
        /// Segment linking two <see cref="SugiVertex"/>.
        /// </summary>
        protected class Segment : Data
        {
            /// <summary>
            /// Gets or sets the p-vertex of the segment.
            /// </summary>
            [NotNull]
            public SugiVertex PVertex { get; }

            /// <summary>
            /// Gets or sets the q-vertex of the segment.
            /// </summary>
            [NotNull]
            public SugiVertex QVertex { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Segment"/> class.
            /// </summary>
            /// <param name="pVertex">P vertex.</param>
            /// <param name="qVertex">Q vertex.</param>
            public Segment([NotNull] SugiVertex pVertex, [NotNull] SugiVertex qVertex)
            {
                PVertex = pVertex;
                QVertex = qVertex;
            }
        }
    }
}
