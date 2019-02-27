/*
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
using System.Collections.Generic;

namespace BSLib
{
    /// <summary>
    /// Miscellaneous algorithms
    /// </summary>
    public static class Algorithms
    {
        public static float CheckBounds(float value, float min, float max)
        {
            if (value < min) {
                value = min;
            }
            if (value > max) {
                value = max;
            }
            return value;
        }

        public static int CheckBounds(int value, int min, int max)
        {
            if (value < min) {
                value = min;
            }
            if (value > max) {
                value = max;
            }
            return value;
        }

        public static int IndexOf<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++) {
                if (array[i].Equals(value)) {
                    return i;
                }
            }
            return -1;
        }

        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++) {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        // TODO: tests!
        public static int BinarySearch<T>(T[] array, T value, Comparison<T> comparer)
        {
            int min = 0;
            int max = array.Length - 1;
            while (min <= max) {
                int mid = min + (max - min >> 1);
                int res = comparer(array[mid], value);
                if (res == 0) {
                    return mid;
                }
                if (res < 0) {
                    min = mid + 1;
                } else {
                    max = mid - 1;
                }
            }
            return ~min;
        }

        /// <summary>
        /// Swaps two objects
        /// </summary>
        /// <typeparam name="T">Type of values</typeparam>
        /// <param name="value1">First value to swap.</param>
        /// <param name="value2">Second value to swap.</param>
        public static void Swap<T>(ref T value1, ref T value2)
        {
            var temp = value1;
            value1 = value2;
            value2 = temp;
        }

        // Returns: value if flag is true, 0 otherwise
        public static int Optional(bool flag, int value)
        {
            return -Convert.ToInt32(flag) & value;
        }

        // Returns: v1 if flag is true, v2 otherwise
        public static int IfElse(bool flag, int v1, int v2)
        {
            return (-Convert.ToInt32(flag) & v1) | ((Convert.ToInt32(flag) - 1) & v2);
        }
    }
}
