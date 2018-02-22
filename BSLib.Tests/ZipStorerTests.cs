using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class ZipStorerTests
    {
        [Test]
        public void Test_Common()
        {
            Assert.Throws(typeof(ArgumentNullException), () => { ZipStorer.Open("", FileAccess.Read); });

            string fileName = TestStubs.GetTempFilePath("test.zip");

            using (ZipStorer zip = ZipStorer.Create(fileName, "test")) {
                using (MemoryStream csvStream = new MemoryStream(Encoding.ASCII.GetBytes(TestStubs.CSVData_CRLF))) {
                    zip.AddStream(ZipStorer.Compression.Deflate, "csv_file.csv", csvStream, DateTime.Now, "");
                }

                Assert.Throws(typeof(InvalidOperationException), () => { zip.ReadCentralDir(); });

                ZipStorer xzip = null;
                Assert.Throws(typeof(ArgumentNullException), () => { xzip = ZipStorer.RemoveEntries(xzip, null); });
                Assert.Throws(typeof(ArgumentNullException), () => { xzip = ZipStorer.RemoveEntries(xzip, null); });
            }

            using (ZipStorer zip = ZipStorer.Open(fileName, FileAccess.Read)) {
                Assert.Throws(typeof(ArgumentNullException), () => { zip.FindFile(null); });

                ZipStorer.ZipFileEntry entry = zip.FindFile("invalid");
                Assert.IsNull(entry);

                entry = zip.FindFile("csv_file.csv");
                Assert.IsNotNull(entry);

                using (MemoryStream csvStream = new MemoryStream()) {
                    Assert.Throws(typeof(ArgumentNullException), () => { zip.ExtractStream(entry, null); });

                    zip.ExtractStream(entry, csvStream);

                    csvStream.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(csvStream, Encoding.ASCII)) {
                        string text = reader.ReadToEnd();
                        Assert.AreEqual(TestStubs.CSVData_CRLF, text);
                    }
                }
            }
        }
    }
}
