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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

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

        /// <summary>
        ///   Clamp a value to a certain range.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="val">The value to be clamped.</param>
        /// <param name="min">The lower bound.</param>
        /// <param name="max">The upper bound</param>
        /// <returns></returns>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) {
                return min;
            } else if (val.CompareTo(max) > 0) {
                return max;
            } else {
                return val;
            }
        }

        #region Inheritance

        public static bool IsDerivedFrom(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type);
        }

        public static bool IsImplementingInterface(this Type type, Type interfaceType)
        {
            return interfaceType.IsAssignableFrom(type);
        }

        public static bool IsDerivedFromOrImplements(this Type type, Type baseType)
        {
            return baseType.IsInterface ? type.IsImplementingInterface(baseType) : type.IsDerivedFrom(baseType);
        }

        #endregion

        #region Array operations

        public static void Clear<T>(this T[] array)
        {
            Array.Clear(array, 0, array.Length);
        }

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

        public static void AddOrSet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key)) {
                dictionary[key] = value;
            } else {
                dictionary.Add(key, value);
            }
        }

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

        #region Enumerable

        /// <summary>
        ///   Wraps this object instance into an IEnumerable&lt;T&gt;
        ///   consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the wrapped object.</typeparam>
        /// <param name="item"> The object to wrap.</param>
        /// <returns>
        ///   An IEnumerable&lt;T&gt; consisting of a single item.
        /// </returns>
        /// <see cref="http://stackoverflow.com/questions/1577822/passing-a-single-item-as-ienumerablet" />
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        public static string Join(this IEnumerable<string> source, string delimiter)
        {
            return string.Join(delimiter, source.ToArray());
        }

        public static string Join(this IEnumerable<string> source)
        {
            return source.Join("");
        }

        public static string JoinLines(this IEnumerable<string> source)
        {
            return source.Join("\r\n");
        }

        #endregion

        #region Threads

        public static bool TryKill(this Thread thread)
        {
            try {
                thread.Abort();
                return true;
            } catch {
                return false;
            }
        }

        #endregion
    }
}
