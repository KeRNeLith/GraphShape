using GraphShape.Controls.Utils;
using NUnit.Framework;

namespace GraphShape.Controls.Tests
{
    /// <summary>
    /// Tests for <see cref="DisposableHelpers"/>.
    /// </summary>
    [TestFixture]
    internal class DisposableHelpersTests
    {
        [Test]
        public void Finally()
        {
            Assert.IsNotNull(DisposableHelpers.Finally(() => {}));

            int counter = 0;
            using (DisposableHelpers.Finally(() => ++counter))
            {
                Assert.AreEqual(0, counter);
            }
            Assert.AreEqual(1, counter);
        }
    }
}
