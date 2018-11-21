using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using BSLib.ArborGVT;
using BSLib.Extensions;
using NUnit.Framework;

namespace BSLib.SmartGraph
{
    public class GTestObj : GraphObject
    {
    }

    [TestFixture]
    public class SmartGraphTests
    {
        GTestObj _context;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _context = new GTestObj();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
        }

        [Test]
        public void T1_Tests()
        {
            IExtensionCollection<GraphObject, IExtension<GraphObject>> exts = _context.Extensions;
            Assert.IsNotNull(exts);
        }

        [Test]
        public void T2_Tests()
        {
            IExtensionCollection<GraphObject, IExtension<GraphObject>> exts = _context.Extensions;

            ArborNode node = new ArborNode("");
            exts.Add(node);

            Assert.IsTrue(exts.Contains(node));
            Assert.AreEqual(1, exts.Count);

            IExtension<GraphObject> xt = exts.Find<ArborNode>();
            Assert.IsNotNull(xt);
            Assert.IsTrue(xt is ArborNode);
            Assert.AreEqual(node, xt);

            //

            Assert.IsTrue(exts.Remove(node));
            Assert.IsFalse(exts.Contains(node));
            Assert.AreEqual(0, exts.Count);

            //

            exts.Add(node);
            ArborNode node1 = new ArborNode("");
            exts.Add(node1);

            Assert.IsTrue(exts.Contains(node));
            Assert.IsTrue(exts.Contains(node1));
            Assert.AreEqual(2, exts.Count);

            Collection<ArborNode> cols = exts.FindAll<ArborNode>();
            Assert.AreEqual(2, cols.Count);

            Assert.IsTrue(exts.Remove(node));
            Assert.IsFalse(exts.Contains(node));
            Assert.IsTrue(exts.Contains(node1));
            Assert.AreEqual(1, exts.Count);

            //

            exts.Add(node); // adding second node, but internal array was length=2

            //

            exts.Clear();
            Assert.AreEqual(0, exts.Count);

            //

            Assert.IsFalse(exts.Remove(null));
            Assert.Throws(typeof(ArgumentNullException), () => { exts.Add(null); });
            Assert.Throws(typeof(ArgumentNullException), () => { new ExtensionCollection<GraphObject, IExtension<GraphObject>>(null); });
        }

        [Test]
        public void Test_Graph()
        {
            Vertex vertex = new Vertex();
            Assert.IsNotNull(vertex);

            Vertex vertex2 = new Vertex();
            Assert.AreNotEqual(0, vertex.CompareTo(vertex2));
            Assert.Throws(typeof(ArgumentException), () => { vertex.CompareTo(null); });

            Assert.Throws(typeof(ArgumentNullException), () => { new Edge(null, vertex2, 1, null); });
            Assert.Throws(typeof(ArgumentNullException), () => { new Edge(vertex, null, 1, null); });

            Edge edge = new Edge(vertex, vertex2, 1, null);
            Assert.IsNotNull(edge);
            Assert.AreEqual(1, edge.Cost);
            Assert.AreEqual(vertex, edge.Source);
            Assert.AreEqual(vertex2, edge.Target);
            Assert.AreEqual(null, edge.Value);

            Edge idg = edge;
            Assert.AreEqual(vertex, idg.Source);
            Assert.AreEqual(vertex2, idg.Target);

            Assert.AreNotEqual(0, edge.CompareTo(new Edge(vertex, vertex2, 1, null)));
            Assert.Throws(typeof(ArgumentException), () => { edge.CompareTo(null); });

            Vertex vert1 = edge.Source;
            Assert.AreEqual(vertex, vert1);
            Vertex vert2 = edge.Target;
            Assert.AreEqual(vertex2, vert2);

            using (Graph graph = new Graph())
            {
                Assert.IsNotNull(graph);

                /*vert1 = graph.AddVertex(null);
				Assert.IsNotNull(vert1);
				graph.DeleteVertex(vert1);*/

                vert1 = graph.AddVertex("test", null);
                Assert.IsNotNull(vert1);

                // the second node with the same signature is not added
                vert2 = graph.AddVertex("test", null);
                Assert.IsNotNull(vert2);
                Assert.AreEqual(vert1, vert2);

                vert2 = graph.FindVertex("test");
                Assert.AreEqual(vert1, vert2);

                graph.DeleteVertex(vert1);

                vert1 = graph.AddVertex("src", null);
                vert2 = graph.AddVertex("tgt", null);
                Edge edge3 = graph.AddDirectedEdge("src", "tgt", 1, null);
                Assert.IsNotNull(edge3);
                graph.DeleteEdge(edge3);

                edge3 = graph.AddDirectedEdge("1", "2", 1, null, false);
                Assert.IsNull(edge3);

                bool res = graph.AddUndirectedEdge(vert1, vert2, 1, null, null);
                Assert.AreEqual(true, res);

                graph.DeleteVertex(vert1); // "src", will be deleted subordinate edges

                foreach (Vertex vtx in graph.Vertices) {
                }

                foreach (Edge edg in graph.Edges) {
                }

                graph.Clear();

                graph.DeleteVertex(null); // no exception
                graph.DeleteEdge(null); // no exception

                graph.FindPathTree(null); // nothing will happen
            }
        }

        [Test]
        public void Graph2_Tests()
        {
            using (Graph graph = new Graph())
            {
                ArborSystem.CreateSample(graph);

                Vertex vertex = graph.FindVertex("1");
                graph.FindPathTree(vertex);

                IEnumerable<Edge> path = graph.GetPath(graph.FindVertex("110"));
                // 110, 88, 67, 53, 23, 4, 1
            }
        }

        [Test]
        public void Test_GraphvizWriter()
        {
            using (Graph graph = new Graph())
            {
                Vertex vert1 = graph.AddVertex("test", null);
                Assert.IsNotNull(vert1);

                Vertex vert2 = graph.AddVertex("test2");
                Assert.IsNotNull(vert2);

                vert1 = graph.AddVertex("src", null);
                vert2 = graph.AddVertex("tgt", null);
                Edge edge3 = graph.AddDirectedEdge("src", "tgt", 1, null);
                Assert.IsNotNull(edge3);

                edge3 = graph.AddDirectedEdge("1", "2", 1, null, false);
                Assert.IsNull(edge3);

                bool res = graph.AddUndirectedEdge(vert1, vert2, 1, null, null);
                Assert.AreEqual(true, res);

                string fileName = TestUtils.GetTempFilePath("test.gvf");
                string[] options = { "ratio=auto" };
                var gvw = new GraphvizWriter("testGraph", options);

                foreach (Vertex vtx in graph.Vertices) {
                    gvw.WriteNode(vtx.Sign, "name", "filled", "black", "box");
                }

                foreach (Edge edg in graph.Edges) {
                    gvw.WriteEdge(edg.Source.Sign, edg.Target.Sign);
                }

                gvw.SaveFile(fileName);
            }
        }
    }
}
