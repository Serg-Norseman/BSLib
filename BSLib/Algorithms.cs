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
