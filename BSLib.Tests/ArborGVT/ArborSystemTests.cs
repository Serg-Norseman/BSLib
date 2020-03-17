using System;
using NUnit.Framework;

namespace BSLib.DataViz.ArborGVT
{
    internal class ArborSystemTest : ArborSystem
    {
        public ArborSystemTest(double repulsion, double stiffness, double friction, IArborRenderer renderer)
            : base(repulsion, stiffness, friction, renderer)
        {
        }

        protected override void StartTimer()
        {
        }

        protected override void StopTimer()
        {
        }
    }

    [TestFixture]
    public class ArborSystemTests
    {
        [Test]
        public void Test_Common()
        {
            var system = new ArborSystemTest(10000.0f, 500.0f /*1000.0f*/, 0.1f, null);
            Assert.IsNotNull(system);
        }
    }
}
