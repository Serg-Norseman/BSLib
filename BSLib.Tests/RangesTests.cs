using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class RangesTests
    {
        [Test]
        public void Test_Common()
        {
            Assert.Throws(typeof(ArgumentException), () => { new Range<int>((2), (1)); });

            Assert.IsTrue(new Range<int>((1), (2)).IsOverlapped(new Range<int>(1, 2)), "chk1"); // true
            Assert.IsTrue(new Range<int>((1), (3)).IsOverlapped(new Range<int>((2), (4))), "chk2"); // true
            Assert.IsTrue(new Range<int>((2), (4)).IsOverlapped(new Range<int>((1), (3))), "chk3"); // true
            Assert.IsFalse(new Range<int>((3), (4)).IsOverlapped(new Range<int>((1), (2))), "chk4"); // false
            Assert.IsFalse(new Range<int>((1), (2)).IsOverlapped(new Range<int>((3), (4))), "chk5"); // false
            Assert.IsTrue(new Range<int>((2), (3)).IsOverlapped(new Range<int>((1), (4))), "chk6"); // true
            Assert.IsTrue(new Range<int>((1), (4)).IsOverlapped(new Range<int>((2), (3))), "chk7"); // true

            Assert.IsTrue(new Range<int>((1), (2)).IsOverlapped(new Range<int>((1), (4))), "chk8"); // true
            Assert.IsTrue(new Range<int>((1), (4)).IsOverlapped(new Range<int>((1), (2))), "chk9"); // true
            Assert.IsTrue(new Range<int>((1), (4)).IsOverlapped(new Range<int>((3), (4))), "chk10"); // true
            Assert.IsTrue(new Range<int>((3), (4)).IsOverlapped(new Range<int>((1), (4))), "chk11"); // true

            var range = new Range<int>(11, 22);
            var range2 = (Range<int>)range.Clone();
            Assert.AreEqual(11, range2.Start);
            Assert.AreEqual(22, range2.End);
        }
    }
}
