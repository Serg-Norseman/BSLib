using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class ReflectionHelperTests
    {
        [Test]
        public void Test_Common()
        {
            Assert.Throws(typeof(ArgumentNullException), () => { ReflectionHelper.GetPropertyValue(null, "Text"); });
            Assert.Throws(typeof(ArgumentNullException), () => { ReflectionHelper.SetPropertyValue(null, "Text", null); });
            Assert.Throws(typeof(ArgumentNullException), () => { ReflectionHelper.GetFieldValue(null, "Text"); });
            Assert.Throws(typeof(ArgumentNullException), () => { ReflectionHelper.SetPropertyValue(null, "Text", null); });

            using (StringList strList = new StringList()) {
                strList.Text = "Test line";

                object obj = ReflectionHelper.GetPropertyValue(strList, "Text");
                Assert.AreEqual("Test line", obj);

                ReflectionHelper.SetPropertyValue(strList, "Text", "Test2");
                Assert.AreEqual("Test2", strList.Text);

                Assert.Throws(typeof(ArgumentOutOfRangeException), () => { ReflectionHelper.GetPropertyValue(strList, "test"); });
                Assert.Throws(typeof(ArgumentOutOfRangeException), () => { ReflectionHelper.SetPropertyValue(strList, "test", ""); });
            }

            Token tkn = new Token(TokenKind.Unknown, 111, 0, "");
            object obj1 = ReflectionHelper.GetFieldValue(tkn, "Line");
            Assert.AreEqual(111, obj1);
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => { ReflectionHelper.GetFieldValue(tkn, "Lines"); });
        }
    }
}
