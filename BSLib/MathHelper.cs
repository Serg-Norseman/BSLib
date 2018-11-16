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
    /// 
    /// </summary>
    public static class MathHelper
    {
        public static long Trunc(double value)
        {
            return (long)Math.Truncate(value);
        }

        public static double SafeDiv(double dividend, double divisor)
        {
            return (divisor == 0.0d) ? 0.0d : (dividend / divisor);
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static int Distance(int x1, int y1, int x2, int y2)
        {
            int dX = x2 - x1;
            int dY = y2 - y1;
            return (int)Math.Round(Math.Sqrt(dX * dX + dY * dY));
        }

        public static int Distance(ExtPoint pt1, ExtPoint pt2)
        {
            return Distance(pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        public static bool IsValueBetween(int value, int lowLimit, int topLimit, bool includeLimits)
        {
            if (lowLimit > topLimit) {
                Algorithms.Swap(ref lowLimit, ref topLimit);
            }

            if (!includeLimits) {
                lowLimit++;
                topLimit--;
            }

            return value >= lowLimit && value <= topLimit;
        }

        public static bool IsValueBetween(double value, double lowLimit, double topLimit, bool includeLimits)
        {
            if (lowLimit > topLimit) {
                Algorithms.Swap(ref lowLimit, ref topLimit);
            }

            if (!includeLimits) {
                lowLimit += 0.000001;
                topLimit -= 0.000001;
            }

            return value >= lowLimit && value <= topLimit;
        }

        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }
    }
}
