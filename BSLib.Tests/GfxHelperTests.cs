using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class GfxHelperTests
    {
        [Test]
        public void Test_Common()
        {
            Assert.AreEqual(2.0, GfxHelper.ZoomToFit(50, 20, 100, 50));
            Assert.AreEqual(3.0, GfxHelper.ZoomToFit(15, 40, 45, 120));

            Assert.AreEqual(1.0, GfxHelper.ZoomToFit(0, 40, 45, 120));
            Assert.AreEqual(1.0, GfxHelper.ZoomToFit(15, 0, 45, 120));
        }
    }
}
