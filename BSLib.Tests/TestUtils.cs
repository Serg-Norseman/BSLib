using System;
using System.IO;
using System.Reflection;

namespace BSLib
{
    public static class TestUtils
    {
        public static string GetTempDir()
        {
            string tempPath = Environment.GetEnvironmentVariable("TEMP");
            return tempPath + Path.DirectorySeparatorChar;
        }

        public static string GetHomePath()
        {
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return homePath + Path.DirectorySeparatorChar;
        }

        public static string GetTempFilePath(string fileName)
        {
            #if !__MonoCS__
            fileName = GetTempDir() + fileName;
            #else
            fileName = GetHomePath() + fileName;
            #endif

            if (File.Exists(fileName)) File.Delete(fileName); // for local tests!

            return fileName;
        }

        public static string PrepareTestFile(string resName)
        {
            string fileName = GetTempFilePath(resName);

            Assembly assembly = typeof(TestUtils).Assembly;
            using (Stream inStream = assembly.GetManifestResourceStream("BSLib.Resources." + resName)) {
                long size = inStream.Length;
                byte[] buffer = new byte[size];
                int res = inStream.Read(buffer, 0, (int)size);
                File.WriteAllBytes(fileName, buffer);
            }

            return fileName;
        }
    }
}
