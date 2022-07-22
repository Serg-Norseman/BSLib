/*
 *  "BSLib".
 *  Copyright (C) 2009-2022 by Sergey V. Zhdanovskih.
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
    }
}
