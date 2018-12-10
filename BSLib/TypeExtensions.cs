/*
 *  "BSLib".
 *  Copyright (C) 2009-2018 by Sergey V. Zhdanovskih.
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
using System.IO;
using System.Text;

namespace BSLib
{
    /// <summary>
    /// Static methods to expand type operations.
    /// </summary>
    public static class TypeExtensions
    {
        public static bool IsNull<T>(this T obj) where T : class
        {
            return ReferenceEquals(obj, null);
        }

        #region Array operations

        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array.IsNull() || array.Length == 0;
        }

        #endregion

        #region Char operations

        public static bool IsDigit(this char c)
        {
            return c >= '0' && c <= '9';
        }

        #endregion

        #region Dictionary operations

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : default(TValue);
        }

        #endregion

        #region Stream operations

        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public static Stream Streamify(this string theString, Encoding encoding = null)
        {
            return new MemoryStream((encoding ?? DefaultEncoding).GetBytes(theString));
        }

        public static string Stringify(this Stream theStream, Encoding encoding = null)
        {
            using (var reader = new StreamReader(theStream, encoding ?? DefaultEncoding)) {
                return reader.ReadToEnd();
            }
        }

        #endregion
    }
}
