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
    /// 
    /// </summary>
    public static class RandomHelper
    {
        public static int RandSeed;
        private static int LastRandSeed;
        private static Random RandomEngine;

        static RandomHelper()
        {
            Randomize();
        }

        public static void Randomize()
        {
            RandSeed = 0;
            LastRandSeed = -1;

            if (LastRandSeed != RandSeed) {
                if (RandSeed == 0) {
                    RandomEngine = new Random();
                } else {
                    RandomEngine = new Random(RandSeed);
                }
                LastRandSeed = RandSeed;
            }
        }

        public static int GetRandom(int range)
        {
            return RandomEngine.Next(range);
        }

        public static int GetBoundedRnd(int low, int high)
        {
            if (low > high) {
                int temp = low;
                low = high;
                high = temp;
            }

            return low + GetRandom(high - low + 1);
        }

        public static ExtPoint GetRandomPoint(ExtRect area)
        {
            int x = GetBoundedRnd(area.Left, area.Right);
            int y = GetBoundedRnd(area.Top, area.Bottom);
            return new ExtPoint(x, y);
        }

        public static T GetRandomItem<T>(T[] array)
        {
            int idx = GetRandom(array.Length);
            return array[idx];
        }

        public static T GetRandomItem<T>(IList<T> list)
        {
            int size = list.Count;

            if (size < 1) {
                return default(T);
            } else if (size == 1) {
                return list[0];
            } else {
                return list[GetRandom(size)];
            }
        }

        public static T GetRandomEnum<T>(Type enumType)
        {
            Array values = Enum.GetValues(enumType);
            int idx = GetRandom(values.Length);
            return (T)values.GetValue(idx);
        }
    }
}
