using System;
using System.IO;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class IniFileTests
    {
        [Test]
        public void Test_Common()
        {
            string fileName = TestStubs.GetTempFilePath("test.ini");

            if (File.Exists(fileName)) File.Delete(fileName); // for local tests!

            using (IniFile iniFile = new IniFile(fileName)) {
                iniFile.WriteInteger("test", "int", 15);
                Assert.AreEqual(15, iniFile.ReadInteger("test", "int", 0));
                iniFile.WriteString("test", "int", "0x9F");
                Assert.AreEqual(159, iniFile.ReadInteger("test", "int", 0));

                iniFile.WriteBool("test", "bool", true);
                Assert.AreEqual(true, iniFile.ReadBool("test", "bool", false));

                iniFile.WriteFloat("test", "float", 0.6666d);
                Assert.AreEqual(0.6666d, iniFile.ReadFloat("test", "float", 0.3333d));

                iniFile.WriteString("test", "str", "alpha");
                Assert.AreEqual("alpha", iniFile.ReadString("test", "str", "beta"));

                DateTime dtx = new DateTime(2016, 08, 11);
                iniFile.WriteDateTime("test", "dtx", dtx);
                Assert.AreEqual(dtx, iniFile.ReadDateTime("test", "dtx", new DateTime())); // writed value

                dtx = new DateTime();
                Assert.AreEqual(dtx, iniFile.ReadDateTime("test", "dtx2", dtx)); // default value

                iniFile.DeleteKey("test", "str");
                Assert.AreEqual("beta", iniFile.ReadString("test", "str", "beta"));

                //iniFile.DeleteSection("test"); // don't work!!!
                iniFile.DeleteKey("test", "int");
                Assert.AreEqual(0, iniFile.ReadInteger("test", "int", 0));
                iniFile.DeleteKey("test", "bool");
                Assert.AreEqual(false, iniFile.ReadBool("test", "bool", false));
                iniFile.DeleteKey("test", "float");
                Assert.AreEqual(0.3333d, iniFile.ReadFloat("test", "float", 0.3333d));
            }
        }
    }
}
