using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class RandomHelperTests
    {
        [Test]
        public void Test_Common()
        {
            int val = RandomHelper.GetBoundedRnd(10, 21);
            Assert.IsTrue(MathHelper.IsValueBetween(val, 10, 21, true));

            val = RandomHelper.GetRandomItem(new int[] { 7, 9, 11, 15 });
            Assert.IsTrue(MathHelper.IsValueBetween(val, 5, 17, true));
        }
    }
}
