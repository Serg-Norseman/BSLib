using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class BitHelperTests
    {
        [Test]
        public void Test_Common()
        {
            Assert.AreEqual(true, BitHelper.IsSetBit(3, 0));
            Assert.AreEqual(true, BitHelper.IsSetBit(3, 1));
            Assert.AreEqual(false, BitHelper.IsSetBit(3, 4));
        }
    }
}
