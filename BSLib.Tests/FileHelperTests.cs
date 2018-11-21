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

        [TestCase(0, Result = "0 B")]
        [TestCase(1, Result = "1 B")]
        [TestCase(1000, Result = "1000 B")]
        [TestCase(1500000, Result = "1.43 MB")]
        [TestCase(2254857830, Result = "2.1 GB")]
        [TestCase(10606877842382992, Result = "9.42 PB")]
        public string Test_FileSizeToStr(long size)
        {
            return FileHelper.FileSizeToStr(size);
        }
    }
}
