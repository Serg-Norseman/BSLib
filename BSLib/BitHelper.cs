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

using System.Runtime.CompilerServices;

namespace BSLib
{
    /// <summary>
    /// 
    /// </summary>
    public static class BitHelper
    {
        public static int SetBit(int bitMask, int bit)
        {
            return (bitMask | (1 << bit));
        }

        public static int UnsetBit(int bitMask, int bit)
        {
            return (bitMask & ~(1 << bit));
        }

        public static bool IsSetBit(int bitMask, int bit)
        {
            return (bitMask & (1 << bit)) != 0;
        }

        /// <summary>
        /// Converts the given <see cref="bool"/> value into a <see cref="byte"/>.
        /// </summary>
        /// <param name="value">The input value to convert.</param>
        /// <returns>1 if <paramref name="flag"/> is <see langword="true"/>, 0 otherwise.</returns>
        /// <remarks>This method does not contain branching instructions.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte ToByte(bool value)
        {
            // Whenever we need to take the address of an argument, we make a local copy first.
            // This will be removed by the JIT anyway, but it can help produce better codegen and
            // remove unwanted stack spills if the caller is using constant arguments. This is
            // because taking the address of an argument can interfere with some of the flow
            // analysis executed by the JIT, which can in some cases block constant propagation.
            bool copy = value;
            return *(byte*)(&copy);
        }
    }
}
