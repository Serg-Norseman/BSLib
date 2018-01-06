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

namespace BSLib
{
    /// <summary>
    /// 
    /// </summary>
    public static class DateHelper
    {
        public static bool IsLeapYear(short year)
        {
            return (year % 4 == 0) && (year % 100 != 0) || (year % 400 == 0);
        }

        public static int DaysBetween(DateTime now, DateTime then)
        {
            TimeSpan span = then - now;
            return span.Days;
        }

        private static readonly byte[][] MONTH_DAYS = new byte[][]
        {
            new byte[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 },
            new byte[] { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }
        };

        public static byte DaysInMonth(short year, byte month)
        {
            return MONTH_DAYS[(month == 2 && IsLeapYear(year)) ? 1 : 0][month - 1];
        }
    }
}
