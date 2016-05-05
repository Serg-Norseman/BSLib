using System;
using GKCommon;

namespace UDNTest
{
    public static class UDNHelper
    {
        public const uint ignoreYear = 1u << 31;
        public const uint ignoreMonth = 1u << 30;
        public const uint ignoreDay = 1u << 29;
        public const uint yearMask = ignoreYear;
        public const uint monthMask = ignoreMonth;
        public const uint dayMask = ignoreDay;
        public const uint valueMask = 0x1fffffff;
        public const int unknownYear = -4713;
        public const int unknownMonth = 0;
        public const int unknownDay = 0;

        public static uint getGregorianUDN(int year, int month, int day)
        {
            uint result = (uint)CalendarConverter.gregorian_to_jd(
                Math.Max(unknownYear + 1, year),
                Math.Max(unknownMonth + 1, month),
                Math.Max(unknownDay + 1, day));

            if (unknownYear + 1 > year)
            {
                result |= ignoreYear;
            }
            if (unknownMonth + 1 > month)
            {
                result |= ignoreMonth;
            }
            if (unknownDay + 1 > day)
            {
                result |= ignoreDay;
            }
            return result;
        }

        public static uint getJulianUDN(int year, int month, int day)
        {
            uint result = (uint) CalendarConverter.julian_to_jd(
                Math.Max(unknownYear + 1, year),
                Math.Max(unknownMonth + 1, month),
                Math.Max(unknownDay + 1, day));

            if (unknownYear + 1 > year)
            {
                result |= ignoreYear;
            }
            if (unknownMonth + 1 > month)
            {
                result |= ignoreMonth;
            }
            if (unknownDay + 1 > day)
            {
                result |= ignoreDay;
            }
            return result;
        }
        
        
    }
}
