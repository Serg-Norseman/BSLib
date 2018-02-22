using System;
using System.IO;

namespace BSLib
{
    public static class TestStubs
    {
        public static string CSVData_CRLF =
            "test,test2,test3,test4\r\n"+
            "12,\"alpha\",12.5,15.4\r\n"+
            "15,\"beta\",15.4,3.7\r\n"+
            "2100,\"gamma delta\",21.5,1.02\r\n"+
            "91000,\"omega\",21.5,1.02\r\n";

        public static string CSVData_LF =
            "test,test2,test3,test4\n"+
            "12,\"alpha\",12.5,15.4\n"+
            "15,\"beta\",15.4,3.7\n"+
            "2100,\"gamma delta\",21.5,1.02\n"+
            "91000,\"omega\",21.5,1.02\n";


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
    }
}
