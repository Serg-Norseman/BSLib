using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class MathHelperTests
    {
        [Test]
        public void Test_Common()
        {
            Assert.AreEqual(495, MathHelper.Trunc(495.575));

            Assert.AreEqual(3.0f, MathHelper.SafeDiv(9.0f, 3.0f));
            Assert.AreEqual(0.0f, MathHelper.SafeDiv(9.0f, 0.0f));

            Assert.AreEqual(57.295779513, MathHelper.RadiansToDegrees(1.0), 0.0000000001);
            Assert.AreEqual(1.0, MathHelper.DegreesToRadians(57.295779513), 0.0000000001);

            Assert.AreEqual(11, MathHelper.Distance(1, 0, 12, 0));
            Assert.AreEqual(16, MathHelper.Distance(new ExtPoint(1, 1), new ExtPoint(12, 12)));
        }
    }
}
