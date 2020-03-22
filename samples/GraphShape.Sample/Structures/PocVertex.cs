using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace GraphShape.Sample
{
    /// <summary>
    /// A simple identifiable vertex.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ID) + "}")]
    internal class PocVertex
    {
        /// <summary>
        /// Vertex ID.
        /// </summary>
        [NotNull]
        public string ID { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PocVertex"/> class.
        /// </summary>
        public PocVertex([NotNull] string id)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ID;
        }
    }
}