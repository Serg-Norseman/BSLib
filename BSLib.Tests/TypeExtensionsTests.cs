using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void Test_ObjectIsNull()
        {
            object obj = null;
            Assert.IsTrue(obj.IsNull());

            obj = new object();
            Assert.IsFalse(obj.IsNull());
        }

        [Test]
        public void Test_ArrayIsNullOrEmpty()
        {
            byte[] bytes = null;
            Assert.IsTrue(bytes.IsNullOrEmpty());

            bytes = new byte[] {};
            Assert.IsTrue(bytes.IsNullOrEmpty());

            bytes = new byte[] { 10, 15 };
            Assert.IsFalse(bytes.IsNullOrEmpty());
        }

        [Test]
        public void Test_CharIsDigit()
        {
            char c = 'a';
            Assert.IsFalse(c.IsDigit());

            c = '7';
            Assert.IsTrue(c.IsDigit());
        }
    }
}
