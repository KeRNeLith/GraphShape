using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GraphSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphSharp.Algorithms.Layout;
using GraphSharp.Algorithms.Layout.Compound.FDP;
using System.Windows;

namespace GraphSharp.Test.Algorithms
{
    /// <summary>
    /// Summary description for CompoundFDPForceTest
    /// </summary>
    [TestClass]
    public class CompoundFDPForceTest
    {
        public CompoundFDPForceTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        CompoundFDPLayoutParameters parameters = new CompoundFDPLayoutParameters();

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void ForceTest()
        {
            var repulsionRange = parameters.IdealEdgeLength * 2;
            Point uPos = new Point();
            Size uSize = new Size(10, 10);
            Size vSize = new Size(10, 10);
            double step = 0.5;
            for (double i = step; i < 30.0; i += step)
            {
                Point vPos = new Point(i, 0);
                Vector repulsionForce = GetRepulsionForce(uPos, vPos, uSize, vSize, repulsionRange);
                Vector springForce = GetSpringForce(uPos, vPos, uSize, vSize);

                Console.WriteLine("{0}:\t{1,40}\t{2,40}", i, springForce, repulsionForce);
            }
        }

        public Vector GetSpringForce(Point uPos, Point vPos, Size uSize, Size vSize)
        {
            double idealLength = parameters.IdealEdgeLength;

            //get the clipping points
            var c_u = LayoutUtil.GetClippingPoint(uSize, uPos, vPos);
            var c_v = LayoutUtil.GetClippingPoint(vSize, vPos, uPos);

            var positionVector = (uPos - vPos);
            positionVector.Normalize();

            Vector F = (c_u - c_v);
            bool isSameDirection = LayoutUtil.IsSameDirection(positionVector, F);
            double length = 0;
            if (isSameDirection)
                length = F.Length - idealLength;
            else
                length = F.Length + idealLength;

            if (F.Length == 0)
                F = -positionVector;
            F.Normalize();
            if (length > 0)
                F *= -1;

            var Fs = Math.Pow(length, 2) / parameters.ElasticConstant * F;
            return Fs;
        }

        public Vector GetRepulsionForce(Point uPos, Point vPos, Size uSize, Size vSize, double repulsionRange)
        {
            var c_u = LayoutUtil.GetClippingPoint(uSize, uPos, vPos);
            var c_v = LayoutUtil.GetClippingPoint(vSize, vPos, uPos);

            var positionVector = (uPos - vPos);
            if (positionVector.Length == 0)
                return new Vector();
            positionVector.Normalize();
            var F = c_u - c_v;
            var isSameDirection = LayoutUtil.IsSameDirection(positionVector, F);
            var Fr = new Vector();

            /*if (isSameDirection)
            {*/
            if (F.Length > repulsionRange)
                return new Vector();
            double length = Math.Max(1, F.Length);
            length = Math.Pow(isSameDirection ? length : 1 / length, isSameDirection ? 2 : 1);
            Fr = parameters.RepulsionConstant / length * positionVector;
            return Fr;
        }

        [TestMethod]
        public void EdgeCrossingTest()
        {
            var uPoint1 = new Point(0, 0);
            var vPoint1 = new Point(5, 5);

            var uPoint2 = new Point(2, 1);
            var vPoint2 = new Point(-4, 2);

            Vector v1 = (vPoint1 - uPoint1);
            Vector v2 = (vPoint2 - uPoint2);

            if (v1 == v2 || v1 == -v2)
            {
                Console.WriteLine("Parallel");
                return; //parallel edges
            }

            var t2 = (uPoint1.Y - uPoint2.Y + (uPoint2.X - uPoint1.X) * v1.Y / v1.X) / (v2.Y - v2.X * v1.Y / v1.X);
            var t1 = (uPoint2.X - uPoint1.X + t2 * v2.X) / v1.X;

            var p = uPoint1 + t1 * v1;
            var b1 = t1 > 0 && (p - uPoint1).Length < (vPoint1 - uPoint1).Length;
            var b2 = t2 > 0 && (p - uPoint2).Length < (vPoint2 - uPoint2).Length;
            Console.WriteLine(b1);
            Console.WriteLine(b2);
            Console.WriteLine(t2);
            Console.WriteLine(p);

            if (b1 && b2)
                Console.WriteLine("Cross: " + p);
            else
                Console.WriteLine("NO cross");
        }
    }
}
