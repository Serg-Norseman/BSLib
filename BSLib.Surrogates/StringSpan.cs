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
            [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
            get {
                return fArray[fStart + index];
            }
        }

        public int Length
        {
            [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
            get {
                return fLength;
            }
        }

        public bool IsEmpty
        {
            [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
            get {
                return 0 >= fLength;
            }
        }

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
        public StringSpan(string array)
        {
            if (array == null)
                throw new NullReferenceException();

            fArray = array;
            fStart = 0;
            fLength = array.Length;
        }

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
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

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
        public static implicit operator StringSpan(string array)
        {
            return new StringSpan(array);
        }

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
        public ISpan<char> Slice(int start)
        {
            return new StringSpan(fArray, start, fLength - start);
        }

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
        public ISpan<char> Slice(int start, int length)
        {
            return new StringSpan(fArray, start, length);
        }

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
        public char[] ToArray()
        {
            if (fLength == 0)
                return new char[0];

            var destination = fArray.ToCharArray(fStart, fLength);
            return destination;
        }

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
        public override string ToString()
        {
            return fArray.Substring(fStart, fLength);
        }
    }
}
