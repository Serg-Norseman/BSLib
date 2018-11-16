using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class ProbabilityTableTests
    {
        [Test]
        public void Test_Common()
        {
            var pt = new ProbabilityTable<int>();
            Assert.IsNotNull(pt);
            Assert.AreEqual(0, pt.Size());
            Assert.AreEqual(0, pt.GetRandomItem()); // default(int)

            pt.Add(5, 90);
            pt.Add(10, 10);

            Assert.AreNotEqual(0, pt.GetRandomItem());
        }
    }
}
