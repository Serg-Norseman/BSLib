using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class DateHelperTests
    {
        [Test]
        public void Test_Common()
        {
            int days = DateHelper.DaysBetween(new DateTime(1990, 10, 10), new DateTime(1990, 10, 13));
            Assert.AreEqual(3, days);

            days = DateHelper.DaysBetween(new DateTime(1990, 10, 10), new DateTime(1990, 10, 02));
            Assert.AreEqual(-8, days);

            Assert.AreEqual(31, DateHelper.DaysInMonth(1990, 5));
        }
    }
}
