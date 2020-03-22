using QuikGraph;
using System.Diagnostics;
using JetBrains.Annotations;

namespace GraphShape.Sample
{
    /// <summary>
    /// A simple identifiable edge.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Source) + "." + nameof(PocVertex.ID) + "} -> {" + nameof(Target) + "." + nameof(PocVertex.ID) + "}")]
    internal class PocEdge : Edge<PocVertex>
    {
        /// <summary>
        /// Edge ID.
        /// </summary>
        [NotNull]
        public string ID { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PocEdge"/> class.
        /// </summary>
        public PocEdge([NotNull] string id, [NotNull] PocVertex source, [NotNull] PocVertex target)
            : base(source, target)
        {
            ID = id;
        }
    }
}