using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class WeightedMeanTests
    {
        [Test]
        public void Test_Common()
        {
            var wm = new WeightedMean();
            Assert.IsNaN(wm.GetResult());

            wm.AddValue(1, 2);
            wm.AddValue(2, 3);
            Assert.AreEqual(1.6, wm.GetResult());
        }
    }
}
