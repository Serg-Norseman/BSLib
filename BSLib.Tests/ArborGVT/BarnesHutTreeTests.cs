using System;
using NUnit.Framework;

namespace BSLib.ArborGVT
{
    [TestFixture]
    public class BarnesHutTreeTests
    {
        #if DEBUG

        [Test]
        public void Test_Branch()
        {
            var branch = new Branch(new ArborPoint(-1, -1), new ArborPoint(2, 2));
            Assert.IsNotNull(branch);
        }

        [Test]
        public void Test_BarnesHutTree_ctor()
        {
            var bht = new BarnesHutTree(new ArborPoint(-1, -1), new ArborPoint(+1, +1), 0.5f);
            Assert.IsNotNull(bht);

            bht.Reset();
            Assert.AreEqual(0, bht.Root.Mass);
            Assert.AreEqual(ArborPoint.Zero, bht.Root.Pt);
        }

        [Test]
        public void Test_BarnesHutTree_GetQuad()
        {
            var bht = new BarnesHutTree(new ArborPoint(-1, -1), new ArborPoint(+1, +1), 0.5f);

            var node = new ArborNode("x");

            node.Pt = new ArborPoint(0.5f, 0.5f);
            int qd = BarnesHutTree.GetQuad(node, bht.Root);
            Assert.AreEqual(BarnesHutTree.QSe, qd);

            node.Pt = new ArborPoint(-0.5f, 0.5f);
            qd = BarnesHutTree.GetQuad(node, bht.Root);
            Assert.AreEqual(BarnesHutTree.QSw, qd);

            node.Pt = new ArborPoint(-0.5f, -0.5f);
            qd = BarnesHutTree.GetQuad(node, bht.Root);
            Assert.AreEqual(BarnesHutTree.QNw, qd);

            node.Pt = new ArborPoint(0.5f, -0.5f);
            qd = BarnesHutTree.GetQuad(node, bht.Root);
            Assert.AreEqual(BarnesHutTree.QNe, qd);

            node.Pt = ArborPoint.Null;
            qd = BarnesHutTree.GetQuad(node, bht.Root);
            Assert.AreEqual(BarnesHutTree.QNone, qd);

            qd = BarnesHutTree.GetQuad(null, bht.Root);
            Assert.AreEqual(BarnesHutTree.QNone, qd);
        }

        [Test]
        public void Test_BarnesHutTree_Insert()
        {
            var bht = new BarnesHutTree(new ArborPoint(-1, -1), new ArborPoint(+1, +1), 0.5f);

            var node = new ArborNode("x");
            node.Pt = new ArborPoint(0.5f, 0.5f);
            bht.Insert(node);
        }

        [Test]
        public void Test_BarnesHutTree_ApplyForces()
        {
            var bht = new BarnesHutTree(new ArborPoint(-1, -1), new ArborPoint(+1, +1), 0.5f);

            var node = new ArborNode("x");
            node.Pt = new ArborPoint(0.5f, 0.5f);
            bht.ApplyForces(node, 10000.0f);
        }

        #endif
    }
}
