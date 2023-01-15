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
    public struct StringSpan : ISpan<char>
    {
        private readonly string fArray;
        private readonly int fStart;
        private readonly int fLength;

        public char this[int index]
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
        public StringSpan(string array)
        {
            if (array == null)
                throw new NullReferenceException();

            fArray = array;
            fStart = 0;
            fLength = array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringSpan(string array, int start, int length)
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
        public static implicit operator StringSpan(string array)
        {
            return new StringSpan(array);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISpan<char> Slice(int start)
        {
            return new StringSpan(fArray, start, fLength - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISpan<char> Slice(int start, int length)
        {
            return new StringSpan(fArray, start, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char[] ToArray()
        {
            if (fLength == 0)
                return new char[0];

            var destination = fArray.ToCharArray(fStart, fLength);
            return destination;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return fArray.Substring(fStart, fLength);
        }
    }
}

#endif
