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
        public static string BytesToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes) {
                var t = b / 16;
                sb.Append((char)(t + (t <= 9 ? '0' : '7')));
                var f = b % 16;
                sb.Append((char)(f + (f <= 9 ? '0' : '7')));
            }
            return sb.ToString();
        }

        public static byte[] HexToBytes(string hex)
        {
            byte[] data = new byte[hex.Length / 2];
            for (int i = 0; i < data.Length; i++)
                data[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            return data;
        }
    }
}
