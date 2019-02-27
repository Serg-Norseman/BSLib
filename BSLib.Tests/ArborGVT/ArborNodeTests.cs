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

            node.Pt = ArborPoint.Zero;
            Assert.AreEqual(ArborPoint.Zero, node.Pt);
        }

        [Test]
        public void Test_ApplyForce()
        {
            var node = new ArborNode("x");
            Assert.IsNotNull(node);

            #if DEBUG

            node.V = new ArborPoint(1.0f, 1.0f);
            node.F = new ArborPoint(3.0f, 4.0f);

            node.Mass = 2.0f;
            node.ApplyForce(new ArborPoint(3.0f, 3.0f));
            Assert.AreEqual(new ArborPoint(4.5f, 5.5f), node.F);

            node.Mass = 0.0d;
            node.ApplyForce(new ArborPoint(3.0f, 3.0f));
            Assert.AreEqual(new ArborPoint(double.PositiveInfinity, double.PositiveInfinity), node.F);

            #endif
        }
    }
}
