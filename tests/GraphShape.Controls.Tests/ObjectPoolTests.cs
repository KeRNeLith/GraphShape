using System;
using System.Linq;
using NUnit.Framework;

namespace GraphShape.Controls.Tests
{
    /// <summary>
    /// Tests for <see cref="ObjectPool{T}"/>.
    /// </summary>
    [TestFixture]
    internal class ObjectPoolTests
    {
        #region Test classes

        private sealed class TestPoolObject : IPoolObject, IDisposable
        {
            private int _resets;

            /// <inheritdoc />
            public void Reset()
            {
                ++_resets;
            }

            private int _terminates;

            /// <inheritdoc />
            public void Terminate()
            {
                ++_terminates;
            }

            /// <inheritdoc />
            public event DisposingHandler Disposing;

            /// <inheritdoc />
            public void Dispose()
            {
                Disposing?.Invoke(this);
            }

            public void CheckResets(int resets)
            {
                Assert.AreEqual(resets, _resets);
                _resets = 0;
            }

            public void CheckTerminates(int terminates)
            {
                Assert.AreEqual(terminates, _terminates);
                _terminates = 0;
            }
        }

        #endregion

        [Test]
        public void Constructor()
        {
            // ReSharper disable ObjectCreationAsStatement
            Assert.DoesNotThrow(() => new ObjectPool<TestPoolObject>());
            Assert.DoesNotThrow(() => new ObjectPool<TestPoolObject>(10, false));
            Assert.DoesNotThrow(() => new ObjectPool<TestPoolObject>(10, true));
            // ReSharper restore ObjectCreationAsStatement
        }

        [TestCase(5)]
        [TestCase(100)]
        public void GetPoolObjectNoGrowth(int nbObjects)
        {
            var pool = new ObjectPool<TestPoolObject>(nbObjects, false);
            for (int i = 0; i < nbObjects; ++i)
            {
                TestPoolObject obj = pool.GetObject();
                Assert.IsNotNull(obj);
            }

            TestPoolObject objNPlus1 = pool.GetObject();
            Assert.IsNull(objNPlus1);
            TestPoolObject objNPlus2 = pool.GetObject();
            Assert.IsNull(objNPlus2);
        }

        [TestCase(5)]
        [TestCase(100)]
        public void GetPoolObjectGrowth(int nbObjects)
        {
            var pool = new ObjectPool<TestPoolObject>(nbObjects, true);
            for (int i = 0; i < nbObjects; ++i)
            {
                TestPoolObject obj = pool.GetObject();
                Assert.IsNotNull(obj);
            }

            TestPoolObject objNPlus1 = pool.GetObject();
            Assert.IsNotNull(objNPlus1);
            TestPoolObject objNPlus2 = pool.GetObject();
            Assert.IsNotNull(objNPlus2);
        }

        [Test]
        public void GetPoolObjectNoGrowth_WithDispose()
        {
            var pool = new ObjectPool<TestPoolObject>(5, false);

            // Get 2 objects
            TestPoolObject obj1 = pool.GetObject();
            Assert.IsNotNull(obj1);
            TestPoolObject obj2 = pool.GetObject();
            Assert.IsNotNull(obj2);
            // => 3 objects remaining in the pool

            // Release one => get back into the pool
            obj1.Dispose();
            // => 4 objects remaining in the pool
            obj1.CheckResets(1);
            obj1.CheckTerminates(0);
            obj2.CheckResets(0);
            obj2.CheckTerminates(0);

            for (int i = 0; i < 4; ++i)
            {
                Assert.IsNotNull(pool.GetObject());
            }

            // No more object in the pool
            TestPoolObject obj = pool.GetObject();
            Assert.IsNull(obj);
        }

        [Test]
        public void GetPoolObjectGrowth_WithDispose()
        {
            var pool = new ObjectPool<TestPoolObject>(5, true);

            // Get 2 objects
            TestPoolObject obj1 = pool.GetObject();
            Assert.IsNotNull(obj1);
            TestPoolObject obj2 = pool.GetObject();
            Assert.IsNotNull(obj2);
            // => 3 objects remaining in the pool

            // Release one => get back into the pool
            obj1.Dispose();
            // => 4 objects remaining in the pool
            obj1.CheckResets(1);
            obj1.CheckTerminates(0);
            obj2.CheckResets(0);
            obj2.CheckTerminates(0);

            TestPoolObject[] objects = Enumerable.Range(0, 4)
                .Select(_ =>
                {
                    TestPoolObject obj = pool.GetObject();
                    Assert.IsNotNull(obj);
                    return obj;
                })
                .ToArray();
            // => 0 object remaining in the pool

            // Object from pool growth
            TestPoolObject obj6 = pool.GetObject();
            Assert.IsNotNull(obj6);

            // Release 5 objects => get back into the pool
            objects[0].Dispose();
            objects[1].Dispose();
            objects[3].Dispose();
            obj6.Dispose();
            obj2.Dispose();

            foreach (TestPoolObject obj in new[] { objects[0], objects[1], objects[3], obj6, obj2 })
            {
                obj.CheckResets(1);
                obj.CheckTerminates(0);
            }
            objects[2].CheckResets(0);
            objects[2].CheckTerminates(0);
            // => 5 objects remaining in the pool

            // Release 6th object => it is terminated
            objects[2].Dispose();
            objects[2].CheckResets(0);
            objects[2].CheckTerminates(1);
        }
    }
}