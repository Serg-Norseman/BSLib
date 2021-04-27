using System;
using System.Runtime.CompilerServices;
using BSLib.Surrogates;

namespace SpanDev
{
    class Program
    {
        private const int IterNum = 100000;

        public static void Main(string[] args)
        {
            const string TestStr = "aaaaaaaaaa bbbbbbbbbbbb cccccccccccc dddddddddddd eeeeeeeeee ffffffffff gggggggggg";

            long y = 0;

            for (int k = 0; k < IterNum; k++) y += EnumPureString(TestStr);

            StringSpan strSpan = TestStr;
            for (int k = 0; k < IterNum; k++) y += EnumStringSpan(strSpan);

            char[] charArr = TestStr.ToCharArray();

            for (int k = 0; k < IterNum; k++) y += EnumPureCharArr(charArr);

            ArraySpan<char> arrSpan = charArr;
            for (int k = 0; k < IterNum; k++) y += EnumArraySpan(arrSpan);

            for (int k = 0; k < IterNum; k++) {
                var str1 = GetSubPureStr(TestStr);
                if (str1 != "bbbbbbbbbbbb") {
                    throw new ArgumentException("str1");
                }
                Hole(ref str1);
            }

            for (int k = 0; k < IterNum; k++) {
                var str2 = GetSubStrSpan(strSpan);
                if (str2 != "bbbbbbbbbbbb") {
                    throw new ArgumentException("str2");
                }
                Hole(ref str2);
            }

            for (int k = 0; k < IterNum; k++) {
                var str2 = GetSubPureCharArr(charArr);
                if (str2 != "bbbbbbbbbbbb") {
                    throw new ArgumentException("str2");
                }
                Hole(ref str2);
            }

            for (int k = 0; k < IterNum; k++) {
                var str2 = GetSubArraySpan(arrSpan);
                if (str2 != "bbbbbbbbbbbb") {
                    throw new ArgumentException("str2");
                }
                Hole(ref str2);
            }

            Console.WriteLine(y);
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

        public static int EnumPureString(string str)
        {
            int x = 0;
            for (int i = 0, len = str.Length; i < len; i++) {
                x += str[i];
            }
            return x;
        }

        public static int EnumStringSpan(StringSpan str)
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

        public static int EnumArraySpan(ArraySpan<char> str)
        {
            int x = 0;
            for (int i = 0, len = str.Length; i < len; i++) {
                x += str[i];
            }
            return x;
        }
    }
}
