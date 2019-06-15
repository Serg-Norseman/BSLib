/*
 *  "BSLib".
 *  Copyright (C) 2009-2019 by Sergey V. Zhdanovskih.
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
    public static class Hash32
    {
        public static uint HashAP(byte[] data, int index, int length)
        {
            uint result = 0;
            int idx = 0;

            for (int i = index; length > 0; i++, length--) {
                result ^= ((idx & 1) == 0) ? ((result << 7) ^ data[i] ^ (result >> 3)) :
                                         (~((result << 11) ^ data[i] ^ (result >> 5)));
                idx++;
            }

            return result;
        }

        public static uint HashBernstein(byte[] data, int index, int length)
        {
            uint result = 5381;

            for (int i = index; length > 0; i++, length--)
                result = (result * 33) + data[i];

            return result;
        }

        public static uint HashBernstein1(byte[] data, int index, int length)
        {
            uint result = 5381;

            for (int i = index; length > 0; i++, length--)
                result = (result * 33) ^ data[i];

            return result;
        }

        public static uint HashBKDR(byte[] data, int index, int length)
        {
            const int SEED = 131;

            uint result = 0;

            for (int i = index; length > 0; i++, length--)
                result = (result * SEED) + data[i];

            return result;
        }

        public static uint HashDEK(byte[] data, int index, int length)
        {
            uint hash = (uint)data.Length;

            for (int i = index; length > 0; i++, length--)
                hash = ((hash << 5) ^ (hash >> 27)) ^ data[i];

            return hash;
        }

        public static uint HashDJB(byte[] data, int index, int length)
        {
            uint result = 5381;

            for (int i = index; length > 0; i++, length--)
                result = ((result << 5) + result) + data[i];

            return result;
        }

        public static uint HashELF(byte[] data, int index, int length)
        {
            uint result = 0;

            for (int i = index; length > 0; i++, length--) {
                result = (result << 4) + data[i];
                uint g = result & 0xf0000000;

                if (g != 0)
                    result ^= g >> 24;

                result &= ~g;
            }

            return result;
        }

        public static uint HashFNV(byte[] data, int index, int length)
        {
            uint result = 2166136261;

            for (int i = index; length > 0; i++, length--)
                result = (result * 16777619) ^ data[i];

            return result;
        }

        public static uint HashFNV1a(byte[] data, int index, int length)
        {
            uint result = 2166136261;

            for (int i = index; length > 0; i++, length--)
                result = (result ^ data[i]) * 16777619;

            return result;
        }

        public static uint HashJS(byte[] data, int index, int length)
        {
            uint result = 1315423911;

            for (int i = index; length > 0; i++, length--)
                result ^= ((result << 5) + data[i] + (result >> 2));

            return result;
        }

        public static uint HashOneAtTime(byte[] data, int index, int length)
        {
            uint result = 0;

            for (int i = index; length > 0; i++, length--) {
                result += data[i];
                result += (result << 10);
                result ^= (result >> 6);
            }

            result += (result << 3);
            result ^= (result >> 11);
            result += (result << 15);

            return result;
        }

        public static uint HashPJW(byte[] data, int index, int length)
        {
            const int BitsInUnsignedInt = sizeof(uint) * 8;
            const int ThreeQuarters = (BitsInUnsignedInt * 3) / 4;
            const int OneEighth = BitsInUnsignedInt / 8;
            const uint HighBits = uint.MaxValue << (BitsInUnsignedInt - OneEighth);

            uint result = 0;

            for (int i = index; length > 0; i++, length--) {
                result = (result << OneEighth) + data[i];

                uint test = result & HighBits;
                if (test != 0)
                    result = ((result ^ (test >> ThreeQuarters)) & (~HighBits));
            }

            return result;
        }

        public static uint HashRotating(byte[] data, int index, int length)
        {
            uint result = 0;

            for (int i = index; length > 0; i++, length--)
                result = (result << 4) ^ (result >> 28) ^ data[i];

            return result;
        }

        public static uint HashSDBM(byte[] data, int index, int length)
        {
            uint result = 0;

            for (int i = index; length > 0; i++, length--)
                result = data[i] + (result << 6) + (result << 16) - result;

            return result;
        }

        public static uint HashShiftAndXor(byte[] data, int index, int length)
        {
            uint result = 0;

            for (int i = index; length > 0; i++, length--)
                result ^= (result << 5) + (result >> 2) + data[i];

            return result;
        }

        public static uint HashRS(byte[] data, int index, int length)
        {
            const uint B = 378551;

            uint result = 0;
            uint m_a = 63689;

            for (int i = index; length > 0; i++, length--) {
                result = (result * m_a) + data[i];
                m_a = m_a * B;
            }

            return result;
        }
    }
}
