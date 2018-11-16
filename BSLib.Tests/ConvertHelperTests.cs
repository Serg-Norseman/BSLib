using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class ConvertHelperTests
    {
        [Test]
        public void Test_ParseInt()
        {
            int ival = ConvertHelper.ParseInt("495", 0);
            Assert.AreEqual(495, ival);

            ival = ConvertHelper.ParseInt("asdfa", 11);
            Assert.AreEqual(11, ival);
        }

        [Test]
        public void Test_ParseFloat()
        {
            Assert.AreEqual(11.05, ConvertHelper.ParseFloat(null, 11.05, false));
            Assert.AreEqual(11.05, ConvertHelper.ParseFloat("495,575", 11.05, false)); // badVal -> defVal

            double fval = ConvertHelper.ParseFloat("495.575", 0);
            Assert.AreEqual(495.575, fval);

            fval = ConvertHelper.ParseFloat("575,495", 0, true);
            Assert.AreEqual(575.495, fval);

            fval = ConvertHelper.ParseFloat("", 22.1);
            Assert.AreEqual(22.1, fval);

            fval = ConvertHelper.ParseFloat("sdgfdf", 22.2);
            Assert.AreEqual(22.2, fval);
        }

        [Test]
        public void Test_AdjustNumber()
        {
            string st = ConvertHelper.AdjustNumber(9, 3);
            Assert.AreEqual("009", st);
        }

        [Test]
        public void Test_UniformName()
        {
            string st = "ivan";
            st = ConvertHelper.UniformName(st);
            Assert.AreEqual("Ivan", st);

            st = ConvertHelper.UniformName(null);
            Assert.AreEqual(null, st);
        }

        [Test]
        public void Test_GetRome()
        {
            Assert.AreEqual("VI", ConvertHelper.GetRome(6), "RomeTest_00");
            Assert.AreEqual("VIII", ConvertHelper.GetRome(8), "RomeTest_01");
            Assert.AreEqual("IX", ConvertHelper.GetRome(9), "RomeTest_02");
            Assert.AreEqual("XXXI", ConvertHelper.GetRome(31), "RomeTest_03");
            Assert.AreEqual("XLVI", ConvertHelper.GetRome(46), "RomeTest_04");
            Assert.AreEqual("XCIX", ConvertHelper.GetRome(99), "RomeTest_05");
            Assert.AreEqual("DLXXXIII", ConvertHelper.GetRome(583), "RomeTest_06");
            Assert.AreEqual("DCCCLXXXVIII", ConvertHelper.GetRome(888), "RomeTest_07");
            Assert.AreEqual("MDCLXVIII", ConvertHelper.GetRome(1668), "RomeTest_08");
            Assert.AreEqual("MCMLXXXIX", ConvertHelper.GetRome(1989), "RomeTest_09");
            Assert.AreEqual("MMMCMXCIX", ConvertHelper.GetRome(3999), "RomeTest_10");
        }

        [Test]
        public void Test_IsDigit()
        {
            Assert.IsFalse(ConvertHelper.IsDigit('F'), "IsDigit(F)");
            Assert.IsTrue(ConvertHelper.IsDigit('9'), "IsDigit(9)");
        }

        [Test]
        public void Test_IsDigits()
        {
            Assert.IsFalse(ConvertHelper.IsDigits("f09"), "IsDigits(f09)");
            Assert.IsTrue(ConvertHelper.IsDigits("99"), "IsDigits(99)");
        }
    }
}
