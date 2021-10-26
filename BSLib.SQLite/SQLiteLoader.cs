using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace SQLite
{
    public static class SQLiteLoader
    {
        public static void Load()
        {
            if (IsRunningOnMono())
                return;

            string asmDir = GetAssemblyPath();
            if (string.IsNullOrEmpty(asmDir)) {
                return;
            }

            string dll = asmDir + Path.DirectorySeparatorChar + "sqlite3.dll";
            if (!File.Exists(dll)) {
                Debug.WriteLine("Saving to dll: " + dll);

                string gzName = IntPtr.Size == 8 ? "sqlite3.x64.dll.gz" : "sqlite3.x86.dll.gz";
                var resStream = LoadResourceStream(gzName);

                using (var gs = new GZipStream(resStream, CompressionMode.Decompress)) {
                    using (var exctDll = new FileStream(dll, FileMode.CreateNew)) {
                        byte[] tmp = new byte[1024 * 256];
                        int r;
                        while ((r = gs.Read(tmp, 0, tmp.Length)) > 0) {
                            exctDll.Write(tmp, 0, r);
                        }
                    }
                }
            }

            Debug.WriteLine("Assembly.LoadFile: " + dll);
        }

        private static bool IsRunningOnMono()
        {
            try {
                bool isRunningOnMono = (Type.GetType("Mono.Runtime") != null);
                return isRunningOnMono;
            } catch {
            }
            return false;
        }

        private static string GetAssemblyPath()
        {
            Assembly asm = Assembly.GetEntryAssembly();
            if (asm == null) {
                asm = Assembly.GetExecutingAssembly();
            }

            Module[] mods = asm.GetModules();
            string fn = mods[0].FullyQualifiedName;
            return Path.GetDirectoryName(fn) + Path.DirectorySeparatorChar;
        }

        private static Stream LoadResourceStream(string resName)
        {
            Assembly assembly = typeof(SQLiteLoader).Assembly;
            Stream resStream = assembly.GetManifestResourceStream("Resources." + resName);
            return resStream;
        }
    }
}
