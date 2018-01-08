using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class BitHelperTests
    {
        [Test]
        public void Test_IsSetBit()
        {
            Assert.AreEqual(true, BitHelper.IsSetBit(3, 0));
            Assert.AreEqual(true, BitHelper.IsSetBit(3, 1));
            Assert.AreEqual(false, BitHelper.IsSetBit(3, 4));
        }

        [Test]
        public void Test_SetBit()
        {
            int val = 0;
            Assert.AreEqual(false, BitHelper.IsSetBit(val, 4));
            val = BitHelper.SetBit(val, 4);
            Assert.AreEqual(true, BitHelper.IsSetBit(val, 4));
        }

        [Test]
        public void Test_UnsetBit()
        {
            int val = 3;
            Assert.AreEqual(true, BitHelper.IsSetBit(val, 1));
            val = BitHelper.UnsetBit(val, 1);
            Assert.AreEqual(false, BitHelper.IsSetBit(val, 1));
        }
    }
}
