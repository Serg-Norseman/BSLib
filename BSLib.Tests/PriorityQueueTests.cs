using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class PriorityQueueTests
    {
        [Test]
        public void Test_Common()
        {
            var pq = new PriorityQueue<int>(100);
            Assert.IsNotNull(pq);

            pq.Add(11);

            pq.Add(5);
            Assert.AreEqual(5, pq.Peek());

            pq.Add(1);
            Assert.AreEqual(3, pq.Size());
            Assert.IsTrue(pq.Contains(11));
            Assert.IsFalse(pq.Contains(7));
            Assert.IsFalse(pq.Empty);
            Assert.IsFalse(pq.Full);

            Assert.AreEqual(1, pq.Poll());
            Assert.AreEqual(5, pq.Poll());

            pq.Clear();
            Assert.AreEqual(0, pq.Size());

            Assert.AreEqual(0, pq.Peek()); // default(int)
            Assert.AreEqual(0, pq.Poll()); // default(int)
        }
    }
}
