using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class FileHelperTests
    {
        [Test]
        public void Test_Common()
        {
            Assert.AreEqual("", FileHelper.GetFileExtension("testfile"));
            Assert.AreEqual(".ext", FileHelper.GetFileExtension("testfile.eXt"));

            Assert.IsFalse(FileHelper.IsRemovableDrive(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
        }
    }
}
