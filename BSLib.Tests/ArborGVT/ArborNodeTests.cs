using System;
using BSLib.SmartGraph;
using NUnit.Framework;

namespace BSLib.ArborGVT
{
    [TestFixture]
    public class ArborNodeTests
    {
        [Test]
        public void Test_Common()
        {
            var node = new ArborNode("x");
            Assert.IsNotNull(node);

            var extensibleObj = new Vertex();
            node.Attach(extensibleObj);
            node.Detach(extensibleObj);

            Assert.AreEqual("x", node.Sign);
            Assert.AreEqual(null, node.Data);
            Assert.AreEqual(false, node.Fixed);
            Assert.AreEqual(1.0f, node.Mass);
            Assert.AreEqual(ArborPoint.Null, node.Pt);
        }
    }
}
