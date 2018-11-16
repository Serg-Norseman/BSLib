using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class MathHelperTests
    {
        [Test]
        public void Test_Trunc()
        {
            Assert.AreEqual(495, MathHelper.Trunc(495.575));
        }

        [Test]
        public void Test_SafeDiv()
        {
            Assert.AreEqual(3.0f, MathHelper.SafeDiv(9.0f, 3.0f));
            Assert.AreEqual(0.0f, MathHelper.SafeDiv(9.0f, 0.0f));
        }

        [Test]
        public void Test_RadiansToDegrees()
        {
            Assert.AreEqual(57.295779513, MathHelper.RadiansToDegrees(1.0), 0.0000000001);
        }

        [Test]
        public void Test_DegreesToRadians()
        {
            Assert.AreEqual(1.0, MathHelper.DegreesToRadians(57.295779513), 0.0000000001);
        }

        [Test]
        public void Test_Distance()
        {
            Assert.AreEqual(11, MathHelper.Distance(1, 0, 12, 0));
            Assert.AreEqual(16, MathHelper.Distance(new ExtPoint(1, 1), new ExtPoint(12, 12)));
        }

        [Test]
        public void Test_IsValueBetweenI()
        {
            Assert.IsTrue(MathHelper.IsValueBetween(15, 0, 20, false));
            Assert.IsTrue(MathHelper.IsValueBetween(15, 20, 0, false));
            Assert.IsTrue(MathHelper.IsValueBetween(20, 0, 20, true));
            Assert.IsFalse(MathHelper.IsValueBetween(20, 0, 20, false));
            Assert.IsFalse(MathHelper.IsValueBetween(25, 0, 20, false));
        }

        [Test]
        public void Test_IsValueBetweenD()
        {
            Assert.IsTrue(MathHelper.IsValueBetween(15.0f, 0.0f, 20.0f, false));
            Assert.IsTrue(MathHelper.IsValueBetween(15.0f, 20.0f, 0.0f, false));
            Assert.IsTrue(MathHelper.IsValueBetween(20.0f, 0.0f, 20.0f, true));
            Assert.IsFalse(MathHelper.IsValueBetween(20.0f, 0.0f, 20.0f, false));
            Assert.IsFalse(MathHelper.IsValueBetween(25.0f, 0.0f, 20.0f, false));
        }

        [Test]
        public void Test_IsOdd()
        {
            Assert.IsTrue(MathHelper.IsOdd(3));
            Assert.IsFalse(MathHelper.IsOdd(2));
        }
    }
}
