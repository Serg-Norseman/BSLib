using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class GfxHelperTests
    {
        [Test]
        public void Test_Darker()
        {
            Assert.AreEqual(0x606060, GfxHelper.Darker(0xC0C0C0, 0.5f));
        }

        [Test]
        public void Test_Lighter()
        {
            Assert.AreEqual(0xC0C0C0, GfxHelper.Lighter(0x808080, 0.5f));
        }

        [Test]
        public void Test_ZoomToFit()
        {
            Assert.AreEqual(2.0, GfxHelper.ZoomToFit(50, 20, 100, 50));
            Assert.AreEqual(3.0, GfxHelper.ZoomToFit(15, 40, 45, 120));

            Assert.AreEqual(1.0, GfxHelper.ZoomToFit(0, 40, 45, 120));
            Assert.AreEqual(1.0, GfxHelper.ZoomToFit(15, 0, 45, 120));
        }
    }
}
