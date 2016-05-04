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

        public static uint getGregorianUDN(int year, int month, int day)
        {
            // pre
            int jdYear = (year > 0) ? year : 1;
            int jdMonth = (month > 0) ? month : 1;
            int jdDay = (day > 0) ? day : 1;

            uint result = (uint)CalendarConverter.gregorian_to_jd(jdYear, jdMonth, jdDay);

            bool unkYear = (year < 1);
            bool unkMonth = (month < 1);
            bool unkDay = (day < 1);
            
            if (unkYear) result |= ignoreYear;
            if (unkMonth) result |= ignoreMonth;
            if (unkDay) result |= ignoreDay;
            
            return result;
        }

        public static uint getJulianUDN(int year, int month, int day)
        {
            // pre
            int jdYear = (year > 0) ? year : 1;
            int jdMonth = (month > 0) ? month : 1;
            int jdDay = (day > 0) ? day : 1;

            uint result = (uint)CalendarConverter.julian_to_jd(jdYear, jdMonth, jdDay);

            bool unkYear = (year < 1);
            bool unkMonth = (month < 1);
            bool unkDay = (day < 1);
            
            if (unkYear) result |= ignoreYear;
            if (unkMonth) result |= ignoreMonth;
            if (unkDay) result |= ignoreDay;
            
            return result;
        }
        
        
    }
}
