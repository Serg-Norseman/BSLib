/*
 *  "BSLib".
 *  Copyright (C) 2009-2026 by Sergey V. Zhdanovskih.
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
using System.Collections.Generic;
using System.Text;

namespace BSLib
{
    public static class StringHelper
    {
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

        public static IEnumerable<string> SplitAndKeep(this string s, char[] delims)
        {
            int start = 0, index;

            while ((index = s.IndexOfAny(delims, start)) != -1) {
                if (index - start > 0)
                    yield return s.Substring(start, index - start);
                yield return s.Substring(index, 1);
                start = index + 1;
            }

            if (start < s.Length) {
                yield return s.Substring(start);
            }
        }

        public static List<string> WordWrap(string text, int maxLineLength)
        {
            var list = new List<string>();

            int currentIndex;
            var lastWrap = 0;
            var whitespace = new[] { ' ', '\r', '\n', '\t' };
            var splitChars = new[] { ' ', ',', '.', '?', '!', ':', ';', '-', '\n', '\r', '\t' };
            do {
                currentIndex = (lastWrap + maxLineLength > text.Length) ? text.Length : (text.LastIndexOfAny(splitChars, Math.Min(text.Length - 1, lastWrap + maxLineLength)) + 1);
                if (currentIndex <= lastWrap)
                    currentIndex = Math.Min(lastWrap + maxLineLength, text.Length);
                list.Add(text.Substring(lastWrap, currentIndex - lastWrap).Trim(whitespace));
                lastWrap = currentIndex;
            } while (currentIndex < text.Length);

            return list;
        }

        public static string GetToken(string str, string sepChars, int tokenNum)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(sepChars)) {
                return "";
            }

            if (sepChars.IndexOf(str[str.Length - 1]) < 0) {
                str += sepChars[0];
            }

            int tok_num = 0;
            string tok = "";

            for (int i = 0; i < str.Length; i++) {
                if (sepChars.IndexOf(str[i]) >= 0) {
                    tok_num++;
                    if (tok_num == tokenNum) {
                        return tok;
                    }
                    tok = "";
                } else {
                    tok += (str[i]);
                }

            }
            return "";
        }

        public static int GetTokensCount(string str, string sepChars)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(sepChars)) {
                return 0;
            }

            int result = 0;

            if (sepChars.IndexOf(str[str.Length - 1]) < 0) {
                str += sepChars[0];
            }
            int num = str.Length;
            for (int i = 0; i < num; i++) {
                if (sepChars.IndexOf(str[i]) >= 0) {
                    result += 1;
                }
            }

            return result;
        }
    }
}
