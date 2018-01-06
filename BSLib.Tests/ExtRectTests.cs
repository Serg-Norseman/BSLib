using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class ExtRectTests
    {
        [Test]
        public void Test_ExtRect()
        {
            ExtRect rt = ExtRect.Create(0, 0, 9, 9);

            Assert.AreEqual(0, rt.Left);
            Assert.AreEqual(0, rt.Top);
            Assert.AreEqual(9, rt.Right);
            Assert.AreEqual(9, rt.Bottom);
            Assert.AreEqual(10, rt.GetHeight());
            Assert.AreEqual(10, rt.GetWidth());

            rt = ExtRect.CreateBounds(0, 0, 10, 10);

            Assert.AreEqual(0, rt.Left);
            Assert.AreEqual(0, rt.Top);
            Assert.AreEqual(9, rt.Right);
            Assert.AreEqual(9, rt.Bottom);
            Assert.AreEqual(10, rt.GetHeight());
            Assert.AreEqual(10, rt.GetWidth());

            Assert.AreEqual("{X=0,Y=0,Width=10,Height=10}", rt.ToString());

            Assert.IsTrue(rt.Contains(5, 5));

            rt.Inflate(3, -2);
            Assert.AreEqual("{X=3,Y=-2,Width=4,Height=14}", rt.ToString());

            rt.Offset(2, 5);
            Assert.AreEqual("{X=5,Y=3,Width=4,Height=14}", rt.ToString());

            rt = rt.GetOffset(10, 10);
            Assert.AreEqual("{X=15,Y=13,Width=4,Height=14}", rt.ToString());

            Assert.IsTrue(rt.IntersectsWith(ExtRect.Create(16, 14, 20, 20)));

            rt = ExtRect.CreateEmpty();
            Assert.IsTrue(rt.IsEmpty());

            Assert.IsFalse(rt.Contains(5, 5));
        }

        [Test]
        public void Test_ExtRectF()
        {
            ExtRectF rt = ExtRectF.Create(0, 0, 9, 9);

            Assert.AreEqual(0, rt.Left);
            Assert.AreEqual(0, rt.Top);
            Assert.AreEqual(9, rt.Right);
            Assert.AreEqual(9, rt.Bottom);
            Assert.AreEqual(10, rt.GetHeight());
            Assert.AreEqual(10, rt.GetWidth());

            rt = ExtRectF.CreateBounds(0, 0, 10, 10);

            Assert.AreEqual(0, rt.Left);
            Assert.AreEqual(0, rt.Top);
            Assert.AreEqual(9, rt.Right);
            Assert.AreEqual(9, rt.Bottom);
            Assert.AreEqual(10, rt.GetHeight());
            Assert.AreEqual(10, rt.GetWidth());

            Assert.AreEqual("{X=0,Y=0,Width=10,Height=10}", rt.ToString());

            Assert.IsTrue(rt.Contains(5, 5));

            rt.Inflate(3, -2);
            Assert.AreEqual("{X=3,Y=-2,Width=4,Height=14}", rt.ToString());

            rt.Offset(2, 5);
            Assert.AreEqual("{X=5,Y=3,Width=4,Height=14}", rt.ToString());

            rt = rt.GetOffset(10, 10);
            Assert.AreEqual("{X=15,Y=13,Width=4,Height=14}", rt.ToString());

            Assert.IsTrue(rt.IntersectsWith(ExtRect.Create(16, 14, 20, 20)));

            rt = ExtRectF.CreateEmpty();
            Assert.IsTrue(rt.IsEmpty());

            Assert.IsFalse(rt.Contains(5, 5));

            // implicit
            ExtRectF rtf = ExtRect.CreateBounds(11, 11, 20, 20);
            Assert.AreEqual("{X=11,Y=11,Width=20,Height=20}", rtf.ToString());
        }


        [Test]
        public void Test_ExtSize()
        {
            ExtSize sz = new ExtSize(9, 9);
            Assert.AreEqual(9, sz.Width);
            Assert.AreEqual(9, sz.Height);
            Assert.IsFalse(sz.IsEmpty);
            Assert.AreEqual("{Width=9, Height=9}", sz.ToString());
        }

        [Test]
        public void Test_ExtSizeF()
        {
            ExtSizeF sz = new ExtSizeF(11, 11);
            Assert.AreEqual(11, sz.Width);
            Assert.AreEqual(11, sz.Height);
            Assert.IsFalse(sz.IsEmpty);
            Assert.AreEqual("{Width=11, Height=11}", sz.ToString());
        }


        [Test]
        public void Test_ExtPoint()
        {
            var pt = new ExtPoint(9, 9);
            Assert.AreEqual(9, pt.X);
            Assert.AreEqual(9, pt.Y);

            pt.Offset(2, 3);
            Assert.AreEqual(11, pt.X);
            Assert.AreEqual(12, pt.Y);

            Assert.IsFalse(pt.IsEmpty);
            Assert.AreEqual("{X=11,Y=12}", pt.ToString());
        }

        [Test]
        public void Test_ExtPointF()
        {
            var pt = new ExtPointF(11, 11);
            Assert.AreEqual(11, pt.X);
            Assert.AreEqual(11, pt.Y);

            pt.Offset(2, 3);
            Assert.AreEqual(13, pt.X);
            Assert.AreEqual(14, pt.Y);

            Assert.IsFalse(pt.IsEmpty);
            Assert.AreEqual("{X=13, Y=14}", pt.ToString());
        }
    }
}
