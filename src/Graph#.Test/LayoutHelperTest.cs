using System;
using GraphSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using GraphSharp.Algorithms.Layout;
using System.Collections.Generic;
using System.Diagnostics;

namespace GraphSharp.Test
{
    
    
    /// <summary>
    ///This is a test class for LayoutHelperTest and is intended
    ///to contain all LayoutHelperTest Unit Tests
    ///</summary>
    [TestClass]
    public class LayoutUtilTest
    {
        [TestMethod]
        public void GetClippingPoint_Target_Outside_Source_Rect_ClippingPoint_OnCorner_Test()
        {
            Size size = new Size(10,10);
            Point s = new Point(5,5);
            Point t = new Point(20,20);
            Point expected = new Point(10,10);

            Point actual = LayoutUtil.GetClippingPoint(size, s, t);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetClippingPoint_Target_Outside_Source_Rect_ClippingPoint_OnBottomSide_Test()
        {
            Size size = new Size(10, 10);
            Point s = new Point(5, 5);
            Point t = new Point(20, 30);
            Point expected = new Point(8, 10);

            Point actual = LayoutUtil.GetClippingPoint(size, s, t);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetClippingPoint_Target_Inside_Source_Rect_ClippingPoint_OnBottomSide_Test()
        {
            Size size = new Size(10, 10);
            Point s = new Point(5, 5);
            Point t = new Point(5.3, 5.5);
            Point expected = new Point(8, 10);

            Point actual = LayoutUtil.GetClippingPoint(size, s, t);
            double epsilon = 0.0000001;
            Assert.IsTrue(Math.Abs(expected.X - actual.X) < epsilon);
            Assert.IsTrue(Math.Abs(expected.Y - actual.Y) < epsilon);
        }

        /// <summary>
        ///A test for BiLayerCrossCount
        ///</summary>
        [TestMethod()]
        public void BiLayerCrossCountTest()
        {
            IEnumerable<Pair> pairs = new Pair[] { 
			                                     	new Pair() { First = 0, Second = 1, Weight = 2},
			                                     	new Pair() { First = 0, Second = 2, Weight = 3},
			                                     	new Pair() { First = 3, Second = 0, Weight = 2}, 
                                                    new Pair() { First = 3, Second = 1, Weight = 4},
                                                    new Pair() { First = 5, Second = 0, Weight = 3}
			                                     };
            int firstLayerVertexCount = 26;
            int secondLayerVertexCount = 3;
            int expected = 49;
            int actual = LayoutUtil.BiLayerCrossCount(pairs, firstLayerVertexCount, secondLayerVertexCount);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BiLayerCrossCount
        ///</summary>
        [TestMethod()]
        public void Markable_BiLayerCrossCountTest()
        {
            CrossCounterPair[] pairs = new CrossCounterPair[] { 
			                                     	new CrossCounterPair() { First = 0, Second = 1, Weight = 1, Markable = true},
			                                     	new CrossCounterPair() { First = 1, Second = 0, Weight = 1, Markable = false},
			                                     	new CrossCounterPair() { First = 3, Second = 1, Weight = 1, Markable = true}, 
                                                    new CrossCounterPair() { First = 3, Second = 2, Weight = 1, Markable = true},
                                                    new CrossCounterPair() { First = 3, Second = 3, Weight = 1, Markable = true},
                                                    new CrossCounterPair() { First = 4, Second = 2, Weight = 1, Markable = false},
                                                    new CrossCounterPair() { First = 2, Second = 2, Weight = 1, Markable = false}
			                                     };
            int firstLayerVertexCount = 5;
            int secondLayerVertexCount = 5;
            int expected = 3;
            int actual = BiLayerCrossCount(pairs, firstLayerVertexCount, secondLayerVertexCount);
            Assert.AreEqual(expected, actual);

            foreach (var pair in pairs)
            {
                Debug.WriteLine(pair.First + " " + pair.Second + " " + pair.Marked);
            }

            Assert.IsTrue(pairs[0].Marked);
            Assert.IsFalse(pairs[1].Marked);
            Assert.IsTrue(pairs[2].Marked);
            Assert.IsFalse(pairs[3].Marked);
            Assert.IsTrue(pairs[4].Marked);
            Assert.IsFalse(pairs[5].Marked);
            Assert.IsFalse(pairs[5].Marked);
        }

        private class CrossCounterPair : Pair
        {
            public bool Markable = false;
            public bool Marked = false;
        }


        private class CrossCounterTreeNode
        {
            public int Accumulator;
            public bool InnerSegmentMarker;
            public readonly Queue<CrossCounterPair> NonInnerSegmentQueue = new Queue<CrossCounterPair>();
        }

        private static int BiLayerCrossCount(IEnumerable<CrossCounterPair> pairs, int firstLayerVertexCount, int secondLayerVertexCount)
        {
            if (pairs == null)
                return 0;

            //radix sort of the pair, order by First asc, Second asc

            #region Sort by Second ASC
            var radixBySecond = new List<CrossCounterPair>[secondLayerVertexCount];
            List<CrossCounterPair> r;
            int pairCount = 0;
            foreach (var pair in pairs)
            {
                //get the radix where the pair should be inserted
                r = radixBySecond[pair.Second];
                if (r == null)
                {
                    r = new List<CrossCounterPair>();
                    radixBySecond[pair.Second] = r;
                }
                r.Add(pair);
                pairCount++;
            }
            #endregion

            #region Sort By First ASC
            var radixByFirst = new List<CrossCounterPair>[firstLayerVertexCount];
            foreach (var list in radixBySecond)
            {
                if (list == null)
                    continue;

                foreach (var pair in list)
                {
                    //get the radix where the pair should be inserted
                    r = radixByFirst[pair.First];
                    if (r == null)
                    {
                        r = new List<CrossCounterPair>();
                        radixByFirst[pair.First] = r;
                    }
                    r.Add(pair);
                }
            }
            #endregion

            //
            // Build the accumulator tree
            //
            int firstIndex = 1;
            while (firstIndex < pairCount)
                firstIndex *= 2;
            int treeSize = 2 * firstIndex - 1;
            firstIndex -= 1;
            CrossCounterTreeNode[] tree = new CrossCounterTreeNode[treeSize];
            for (int i = 0; i < treeSize; i++)
                tree[i] = new CrossCounterTreeNode();

            //
            // Count the crossings
            //
            int crossCount = 0;
            int index;
            foreach (var list in radixByFirst)
            {
                if (list == null)
                    continue;

                foreach (var pair in list)
                {
                    index = pair.Second + firstIndex;
                    tree[index].Accumulator += pair.Weight;
                    switch (pair.Markable)
                    {
                        case false:
                            tree[index].InnerSegmentMarker = true;
                            break;
                        case true:
                            tree[index].NonInnerSegmentQueue.Enqueue(pair);
                            break;
                        default:
                            break;
                    }
                    while (index > 0)
                    {
                        if (index % 2 > 0)
                        {
                            crossCount += tree[index + 1].Accumulator * pair.Weight;
                            switch (pair.Markable)
                            {
                                case false:
                                    var queue = tree[index + 1].NonInnerSegmentQueue;
                                    while (queue.Count > 0)
                                    {
                                        queue.Dequeue().Marked = true;
                                    }
                                    break;
                                case true:
                                    if (tree[index+1].InnerSegmentMarker)
                                    {
                                        pair.Marked = true;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        index = (index - 1) / 2;
                        tree[index].Accumulator += pair.Weight;
                        switch (pair.Markable)
                        {
                            case false:
                                tree[index].InnerSegmentMarker = true;
                                break;
                            case true:
                                tree[index].NonInnerSegmentQueue.Enqueue(pair);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return crossCount;
        }
    }
}
