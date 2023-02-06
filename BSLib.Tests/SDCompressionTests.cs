using System;
using NUnit.Framework;

namespace BSLib.Data
{
    [TestFixture]
    public class SDCompressionTests
    {
        [Test]
        public void Test_Common()
        {
            // DS18B20: -55°C до +125°C, ±0.5°C
            var instance = new SDCompression(0.5, 3600);
            Assert.IsNotNull(instance);

            DateTime timestamp;
            double value;

            timestamp = new DateTime(2019, 08, 02, 20, 00, 00);
            value = 22.0;
            Assert.AreEqual(true, instance.ReceivePoint(ref timestamp, ref value));

            var timestamp2 = new DateTime(2019, 08, 02, 20, 00, 01);
            var value2 = 22.1;
            Assert.AreEqual(false, instance.ReceivePoint(ref timestamp2, ref value2));

            DateTime timestamp3 = new DateTime(2019, 08, 02, 20, 00, 02);
            timestamp = timestamp3;
            double value3 = 23.8;
            value = value3;
            Assert.AreEqual(true, instance.ReceivePoint(ref timestamp, ref value));
            Assert.AreEqual(timestamp2, timestamp);
            Assert.AreEqual(value2, value);

            // point exceeds CorridorTimeSec (+2h > 1h (3600sec)), save previous point
            DateTime timestamp4 = new DateTime(2019, 08, 02, 22, 00, 02);
            double value4 = 23.5;
            Assert.AreEqual(true, instance.ReceivePoint(ref timestamp4, ref value4));
            Assert.AreEqual(timestamp3, timestamp4);
            Assert.AreEqual(value3, value4);
        }
    }
}
