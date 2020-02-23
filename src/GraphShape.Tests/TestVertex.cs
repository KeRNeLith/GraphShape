using JetBrains.Annotations;

namespace GraphShape.Tests
{
    /// <summary>
    /// Vertex type used for tests.
    /// </summary>
    internal class TestVertex
    {
        private static int _counter;

        public TestVertex()
            : this($"TestVertex{_counter++}")
        {
        }

        public TestVertex([NotNull] string name)
        {
            Name = name;
        }

        [NotNull]
        public string Name { get; }
    }
}