using System.Diagnostics;
using System.Windows;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout.Simple.Hierarchical
{
    public partial class SugiyamaLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IVertexAndEdgeListGraph<TVertex, TEdge>
    {
        [DebuggerDisplay("{" + nameof(Original) + "} [{" + nameof(LayerIndex) + "}] " +
                         "Pos={" + nameof(Position) + "} " +
                         "Measure={" + nameof(Measure) + "} " +
                         "RealPos={" + nameof(RealPosition) + "}")]
        private class SugiVertex : WrappedVertex<TVertex>
        {
            // Constants
            public const int UndefinedLayerIndex = -1;
            private const int UndefinedPosition = -1;

            private int _layerIndex = UndefinedLayerIndex;

            /// <summary>
            /// Used in the algorithms for temporary storage.
            /// </summary>
            private int _tempPosition;

            /// <summary>
            /// The position of the vertex inside the layer.
            /// </summary>
            public int Position { get; set; }

            /// <summary>
            /// The measure of the vertex (up/down-barycenter/median depends on the implementation).
            /// </summary>
            public double Measure { get; set; }

            /// <summary>
            /// The real position (x and y coordinates) of the vertex.
            /// </summary>
            public Point RealPosition;

            /// <summary>
            /// Size of the vertex.
            /// </summary>
            public Size Size { get; }

            /// <summary>
            /// The index of the layer where this vertex belongs to.
            /// </summary>
            public int LayerIndex
            {
                get { return _layerIndex; }
                set
                {
                    if (_layerIndex != value)
                    {
                        //change the index
                        _layerIndex = value;

                        //add to the new layer
                        if (_layerIndex == UndefinedLayerIndex)
                            Position = UndefinedPosition;
                    }
                }
            }

            /// <summary>
            /// Checks if this vertex is a dummy vertex (a point of a replaced long edge) or not.
            /// </summary>
            public bool IsDummyVertex => Original is null;

            /// <summary>
            /// The priority of the vertex. Used in the horizontal position assignment phase.
            /// The dummy vertices has maximal priorities (because the dummy edge should be as vertical as possible).
            /// The other vertices priority based on it's edge count.
            /// </summary>
            public int Priority { get; set; }

            /// <summary>
            /// Represents the sub-priority of this vertex between the vertices with the same priority.
            /// </summary>
            public int SubPriority { get; set; }

            public int PermutationIndex { get; set; }

            /// <summary>
            /// Constructor of the vertex.
            /// </summary>
            /// <param name="originalVertex">The object which is wrapped by this <see cref="SugiVertex"/>.</param>
            /// <param name="size">The size of the original vertex.</param>
            public SugiVertex([CanBeNull] TVertex originalVertex, Size size)
                : base(originalVertex)
            {
                Size = size;
            }

            /// <summary>
            /// Backup <see cref="Position"/> into <see cref="_tempPosition"/>.
            /// </summary>
            public void SavePositionToTemp()
            {
                _tempPosition = Position;
            }

            /// <summary>
            /// Restore <see cref="Position"/> from <see cref="_tempPosition"/>.
            /// </summary>
            public void LoadPositionFromTemp()
            {
                Position = _tempPosition;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return (Original is null ? "Dummy" : Original.ToString()) + $" [{LayerIndex}]";
            }
        }
    }
}