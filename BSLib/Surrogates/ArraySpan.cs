/*
 *  "BSLib".
 *  Copyright (C) 2009-2023 by Sergey V. Zhdanovskih.
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

#if NET45_AND_ABOVE

using System;
using System.Runtime.CompilerServices;

namespace BSLib.Surrogates
{
    public struct ArraySpan<T> : ISpan<T>
    {
        private readonly T[] fArray;
        private readonly int fStart;
        private readonly int fLength;

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return fArray[fStart + index];
            }
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return fLength;
            }
        }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return 0 >= fLength;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySpan(T[] array)
        {
            if (array == null)
                throw new NullReferenceException();

            fArray = array;
            fStart = 0;
            fLength = array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySpan(T[] array, int start, int length)
        {
            if (array == null)
                throw new NullReferenceException();

            if (start > array.Length || length > (array.Length - start))
                throw new ArgumentOutOfRangeException();

            fArray = array;
            fStart = start;
            fLength = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ArraySpan<T>(T[] array)
        {
            return new ArraySpan<T>(array);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISpan<T> Slice(int start)
        {
            return new ArraySpan<T>(fArray, start, fLength - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISpan<T> Slice(int start, int length)
        {
            return new ArraySpan<T>(fArray, start, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ToArray()
        {
            var destination = new T[fLength];
            if (fLength != 0) {
                Buffer.BlockCopy(fArray, fStart, destination, 0, fLength);
            }
            return destination;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            if (typeof(T) == typeof(char)) {
                var charArr = (char[])(object)fArray;
                return new string(charArr, fStart, fLength);
            }
            return string.Format("BSLib.Surrogates.ArraySpan<{0}>[{1}]", typeof(T).Name, fLength);
        }
    }
}

#endif
