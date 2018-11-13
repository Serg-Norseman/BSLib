using System;
using NUnit.Framework;
using BSLib.SmartGraph;

namespace BSLib.ArborGVT
{
    [TestFixture]
    public class ArborEdgeTests
    {
        [Test]
        public void Test_Common()
        {
            var node1 = new ArborNode("x1");
            var node2 = new ArborNode("x2");

            var edge = new ArborEdge(node1, node2, 11.0f, 22.0f, true);
            Assert.IsNotNull(edge);

            Assert.Throws(typeof(ArgumentNullException), () => { new ArborEdge(null, node2, 11.0f, 22.0f, true); });
            Assert.Throws(typeof(ArgumentNullException), () => { new ArborEdge(node1, null, 11.0f, 22.0f, true); });

            var vertex1 = new Vertex();
            var vertex2 = new Vertex();
            var extensibleObj = new Edge(vertex1, vertex2, 1, null);
            edge.Attach(extensibleObj);
            edge.Detach(extensibleObj);

            Assert.AreEqual(node1, edge.Source);
            Assert.AreEqual(node2, edge.Target);
            Assert.AreEqual(11.0f, edge.Length);
            Assert.AreEqual(22.0f, edge.Stiffness);
            Assert.AreEqual(true, edge.Directed);
        }
    }
}
