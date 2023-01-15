using System;
using System.Runtime.CompilerServices;

namespace SpanDev
{
#if NET45_AND_ABOVE
    using BSLib.Surrogates;
#endif

    class Program
    {
        private const int IterNum = 100000;

        public static void Main(string[] args)
        {
            const string TestStr = "aaaaaaaaaa bbbbbbbbbbbb cccccccccccc dddddddddddd eeeeeeeeee ffffffffff gggggggggg";

            long y = 0;
            var watch = new System.Diagnostics.Stopwatch();

            StringSpan strSpan = TestStr;
            char[] charArr = TestStr.ToCharArray();
            ArraySpan<char> arrSpan = charArr;
            Span<char> arrSysSpan = charArr;

            for (int i = 1; i < 21; i++) {
                watch.Reset();
                watch.Start();
                for (int k = 0; k < IterNum; k++)
                    y += EnumPureString(TestStr);
                Hole(ref y);
                watch.Stop();
                Console.WriteLine($"Execution Time (EnumPureString): {watch.ElapsedMilliseconds} ms");

                watch.Reset();
                watch.Start();
                for (int k = 0; k < IterNum; k++)
                    y += EnumStringSurrSpan(strSpan);
                Hole(ref y);
                watch.Stop();
                Console.WriteLine($"Execution Time (EnumStringSurrSpan): {watch.ElapsedMilliseconds} ms");

                watch.Reset();
                watch.Start();
                for (int k = 0; k < IterNum; k++)
                    y += EnumPureCharArr(charArr);
                Hole(ref y);
                watch.Stop();
                Console.WriteLine($"Execution Time (EnumPureCharArr): {watch.ElapsedMilliseconds} ms");

                watch.Reset();
                watch.Start();
                for (int k = 0; k < IterNum; k++)
                    y += EnumArraySurrSpan(arrSpan);
                Hole(ref y);
                watch.Stop();
                Console.WriteLine($"Execution Time (EnumArraySurrSpan): {watch.ElapsedMilliseconds} ms");

                watch.Reset();
                watch.Start();
                for (int k = 0; k < IterNum; k++)
                    y += EnumArraySysSpan(arrSysSpan);
                Hole(ref y);
                watch.Stop();
                Console.WriteLine($"Execution Time (EnumArraySysSpan): {watch.ElapsedMilliseconds} ms");
                Console.WriteLine();
            }


            watch.Reset();
            watch.Start();
            for (int k = 0; k < IterNum; k++) {
                var str1 = GetSubPureStr(TestStr);
                if (str1 != "bbbbbbbbbbbb") {
                    throw new ArgumentException("str1");
                }
                Hole(ref str1);
            }
            watch.Stop();
            Console.WriteLine($"Execution Time (GetSubPureStr): {watch.ElapsedMilliseconds} ms");

            watch.Reset();
            watch.Start();
            for (int k = 0; k < IterNum; k++) {
                var str2 = GetSubStrSpan(strSpan);
                if (str2 != "bbbbbbbbbbbb") {
                    throw new ArgumentException("str2");
                }
                Hole(ref str2);
            }
            watch.Stop();
            Console.WriteLine($"Execution Time (GetSubStrSpan): {watch.ElapsedMilliseconds} ms");

            watch.Reset();
            watch.Start();
            for (int k = 0; k < IterNum; k++) {
                var str2 = GetSubPureCharArr(charArr);
                if (str2 != "bbbbbbbbbbbb") {
                    throw new ArgumentException("str2");
                }
                Hole(ref str2);
            }
            watch.Stop();
            Console.WriteLine($"Execution Time (GetSubPureCharArr): {watch.ElapsedMilliseconds} ms");

            watch.Reset();
            watch.Start();
            for (int k = 0; k < IterNum; k++) {
                var str2 = GetSubArraySpan(arrSpan);
                if (str2 != "bbbbbbbbbbbb") {
                    throw new ArgumentException("str2");
                }
                Hole(ref str2);
            }
            watch.Stop();
            Console.WriteLine($"Execution Time (GetSubArraySpan): {watch.ElapsedMilliseconds} ms");

            Console.WriteLine(y);
            Console.ReadKey();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static string GetSubPureStr(string str)
        {
            return str.Substring(11, 12);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static string GetSubStrSpan(StringSpan strSpan)
        {
            return strSpan.Slice(11, 12).ToString();
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static string GetSubPureCharArr(char[] str)
        {
            return new string(str, 11, 12);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static string GetSubArraySpan(ArraySpan<char> strSpan)
        {
            return strSpan.Slice(11, 12).ToString();
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void Hole(ref string value)
        {
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void Hole(ref long value)
        {
        }

        public static int EnumPureString(string str)
        {
            int x = 0;
            for (int i = 0, len = str.Length; i < len; i++) {
                x += str[i];
            }
            return x;
        }

        public static int EnumStringSurrSpan(StringSpan str)
        {
            int x = 0;
            for (int i = 0, len = str.Length; i < len; i++) {
                x += str[i];
            }
            return x;
        }


        public static int EnumPureCharArr(char[] str)
        {
            int x = 0;
            for (int i = 0, len = str.Length; i < len; i++) {
                x += str[i];
            }
            return x;
        }

        public static int EnumArraySurrSpan(ArraySpan<char> str)
        {
            int x = 0;
            for (int i = 0, len = str.Length; i < len; i++) {
                x += str[i];
            }
            return x;
        }

        public static int EnumArraySysSpan(Span<char> str)
        {
            int x = 0;
            for (int i = 0, len = str.Length; i < len; i++) {
                x += str[i];
            }
            return x;
        }
    }
}
