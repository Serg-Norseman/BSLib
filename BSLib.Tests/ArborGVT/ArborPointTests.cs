using System;
using NUnit.Framework;

namespace BSLib.ArborGVT
{
    [TestFixture]
    public class ArborPointTests
    {
        [Test]
        public void Test_Equals()
        {
            var pt = new ArborPoint(3, 4);
            Assert.IsTrue(pt.Equals(new ArborPoint(3, 4)));
            Assert.IsFalse(pt.Equals(new ArborPoint(10, 11)));
        }

        [Test]
        public void Test_EqualsObj()
        {
            var pt = new ArborPoint(3, 4);

            object ptx = new ArborPoint(3, 4);
            Assert.IsTrue(pt.Equals(ptx));

            ptx = new ArborPoint(10, 11);
            Assert.IsFalse(pt.Equals(ptx));

            Assert.IsFalse(pt.Equals(null));
        }

        [Test]
        public void Test_IsNull()
        {
            var pt = ArborPoint.Null;
            Assert.IsTrue(pt.IsNull());

            pt = ArborPoint.Zero;
            Assert.IsFalse(pt.IsNull());
        }

        [Test]
        public void Test_IsExploded()
        {
            var pt = new ArborPoint(double.NaN, 4);
            Assert.IsTrue(pt.IsExploded());

            pt = new ArborPoint(3, 4);
            Assert.IsFalse(pt.IsExploded());
        }

        [Test]
        public void Test_Add()
        {
            var pt = new ArborPoint(3, 4);
            Assert.AreEqual(3.0f, pt.X);
            Assert.AreEqual(4.0f, pt.Y);

            pt = pt.Add(new ArborPoint(10, 11));
            Assert.AreEqual(13.0f, pt.X);
            Assert.AreEqual(15.0f, pt.Y);
        }

        [Test]
        public void Test_Sub()
        {
            var pt = new ArborPoint(3, 4);
            Assert.AreEqual(3.0f, pt.X);
            Assert.AreEqual(4.0f, pt.Y);

            pt = pt.Sub(new ArborPoint(10, 11));
            Assert.AreEqual(-7.0f, pt.X);
            Assert.AreEqual(-7.0f, pt.Y);
        }

        [Test]
        public void Test_Mul()
        {
            var pt = new ArborPoint(3, 4);
            Assert.AreEqual(3.0f, pt.X);
            Assert.AreEqual(4.0f, pt.Y);

            pt = pt.Mul(5);
            Assert.AreEqual(15.0f, pt.X);
            Assert.AreEqual(20.0f, pt.Y);
        }

        [Test]
        public void Test_Div()
        {
            var pt = new ArborPoint(3, 4);
            Assert.AreEqual(3.0f, pt.X);
            Assert.AreEqual(4.0f, pt.Y);

            pt = pt.Div(5);
            Assert.AreEqual(3d / 5d, pt.X);
            Assert.AreEqual(4d / 5d, pt.Y);
        }

        [Test]
        public void Test_Magnitude()
        {
            var pt = new ArborPoint(3, 4);
            Assert.AreEqual(3.0f, pt.X);
            Assert.AreEqual(4.0f, pt.Y);

            Assert.AreEqual(5d, pt.Magnitude());
        }

        [Test]
        public void Test_MagnitudeSquare()
        {
            var pt = new ArborPoint(3, 4);
            Assert.AreEqual(3.0f, pt.X);
            Assert.AreEqual(4.0f, pt.Y);

            Assert.AreEqual(25d, pt.MagnitudeSquare());
        }

        [Test]
        public void Test_Normalize()
        {
            var pt = new ArborPoint(3, 4);
            Assert.AreEqual(3.0f, pt.X);
            Assert.AreEqual(4.0f, pt.Y);

            pt = pt.Normalize();
            Assert.AreEqual(3d / 5d, pt.X);
            Assert.AreEqual(4d / 5d, pt.Y);
        }
    }
}
