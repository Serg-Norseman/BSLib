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

namespace BSLib
{
    /// <summary>
    /// Static methods to expand array operations.
    /// </summary>
    public static class ArrayHelper
    {
        public static T[] ConcatArrays<T>(T[] arr1, T[] arr2)
        {
            if (arr1 == null)
                throw new ArgumentNullException("arr1");

            if (arr2 == null)
                throw new ArgumentNullException("arr2");

            var result = new T[arr1.Length + arr2.Length];
            Buffer.BlockCopy(arr1, 0, result, 0, arr1.Length);
            Buffer.BlockCopy(arr2, 0, result, arr1.Length, arr2.Length);
            return result;
        }

        public static T[] SubArray<T>(T[] arr, int start, int length)
        {
            if (arr == null)
                throw new ArgumentNullException("arr");

            if (start < 0)
                throw new ArgumentOutOfRangeException("start", "start must be greater or equal 0");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length", "length must be greater or equal 0");

            if (start + length > arr.Length)
                throw new Exception("start + length must be lower or equal arr.Length");

            var result = new T[length];
            Buffer.BlockCopy(arr, start, result, 0, length);
            return result;
        }

        public static T[] SubArray<T>(T[] arr, int start)
        {
            return SubArray(arr, start, arr.Length - start);
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
                if (!comparer.Equals(a1[i], a2[i]))
                    return false;
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
    }
}
