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

namespace BSLib
{
    /// <summary>
    /// Static methods to expand array operations.
    /// </summary>
    public static class ArrayHelper
    {
        public static void Clear(this Array array)
        {
            Array.Clear(array, 0, array.Length);
        }

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
    }
}
