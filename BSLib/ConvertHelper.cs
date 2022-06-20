﻿/*
 *  "BSLib".
 *  Copyright (C) 2009-2017 by Sergey V. Zhdanovskih.
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Globalization;
using System.Text;

namespace BSLib
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConvertHelper
    {
        static ConvertHelper()
        {
            InitRome();
        }

        public static bool IsDigit(char chr)
        {
            return chr >= '0' && chr <= '9';
        }

        public static bool IsDigits(string str)
        {
            bool res = false;

            if (!string.IsNullOrEmpty(str))
            {
                int I;
                for (I = 1; I <= str.Length; I++)
                {
                    char c = str[I - 1];
                    if (c < '0' || c >= ':')
                    {
                        break;
                    }
                }
                res = (I > str.Length);
            }

            return res;
        }

        public static bool IsLetterOrDigit(char ch)
        {
            char ch2 = ch;
            ch2 |= ' ';
            return (ch2 >= 'a' && ch2 <= 'z') || (ch >= '0' && ch <= '9');
        }

        public static bool IsValidInt(string value)
        {
            if (string.IsNullOrEmpty(value)) {
                return false;
            }

            for (int i = 0; i < value.Length; i++) {
                if (!IsDigit(value[i])) {
                    return false;
                }
            }

            return true;
        }

        #region Rome numbers

        private static int[] RN_N;
        private static string[] RN_S;

        private static void InitRome()
        {
            RN_N = new int[] { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };
            RN_S = new string[] { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M" };
        }

        public static string GetRome(int num)
        {
            string rome = "";
            int T = 12;

            if (num > 0)
            {
                while (true)
                {
                    int rn = RN_N[T];

                    if (num >= rn) {
                        while (num >= rn) {
                            num -= rn;
                            rome += RN_S[T];
                        }

                        if (num <= 0) break;
                    } else {
                        T -= 1;
                    }
                }
            }
            return rome;
        }

        #endregion

        public static NumberFormatInfo CreateDefaultNumberFormat()
        {
            var result = new NumberFormatInfo();
            result.NumberDecimalSeparator = ".";
            result.NumberGroupSeparator = "";
            return result;
        }

        public static int ParseInt(string str, int Default)
        {
            int res;
            if (!int.TryParse(str, out res)) res = Default;
            return res;
        }

        public static long ParseLong(string str, int Default)
        {
            long res;
            if (!long.TryParse(str, out res)) res = Default;
            return res;
        }

        public static double ParseFloat(string str, double defaultValue, bool checkSeparator = false)
        {
            if (string.IsNullOrEmpty(str)) return defaultValue;

            string decSep;
            if (checkSeparator) {
                decSep = (str.Contains(",") ? "," : ".");
            } else {
                decSep = ".";
            }

            NumberFormatInfo formatInfo = new NumberFormatInfo();
            formatInfo.NumberDecimalSeparator = decSep;
            formatInfo.NumberGroupSeparator = " ";

            double value;
            double result;
            if (double.TryParse(str, NumberStyles.Float, formatInfo, out value)) {
                result = value;
            } else {
                result = defaultValue;
            }
            return result;
        }

        public static string AdjustNumber(int val, int up, char pad = '0')
        {
            string result = Convert.ToString(val);
            if (result.Length < up) {
                StringBuilder sb = new StringBuilder(result);
                while (sb.Length < up) {
                    sb.Insert(0, pad);
                }
                result = sb.ToString();
            }
            return result;
        }

        public static string Repeat(char ch, int repeat)
        {
            char[] chars = new char[repeat];
            for (int i = 0; i < repeat; i++)
                chars[i] = ch;
            return new string(chars);
        }

        public static string UniformName(string val)
        {
            if (string.IsNullOrEmpty(val)) {
                return null;
            }

            StringBuilder str = new StringBuilder(val.ToLower());
            str[0] = char.ToUpper(str[0]);
            return str.ToString();
        }
    }
}
