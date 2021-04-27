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
        public ArraySpan(T[] array)
        {
            if (array == null)
                throw new NullReferenceException();

            fArray = array;
            fStart = 0;
            fLength = array.Length;
        }

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
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

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
        public static implicit operator ArraySpan<T>(T[] array)
        {
            return new ArraySpan<T>(array);
        }

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
        public ISpan<T> Slice(int start)
        {
            return new ArraySpan<T>(fArray, start, fLength - start);
        }

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
        public ISpan<T> Slice(int start, int length)
        {
            return new ArraySpan<T>(fArray, start, length);
        }

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
        public T[] ToArray()
        {
            var destination = new T[fLength];
            if (fLength != 0) {
                Buffer.BlockCopy(fArray, fStart, destination, 0, fLength);
            }
            return destination;
        }

        [MethodImpl((MethodImplOptions)256/*MethodImplOptions.AggressiveInlining*/)]
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
