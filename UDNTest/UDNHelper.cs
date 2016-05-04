using System;
using GKCommon;

namespace UDNTest
{
    public static class UDNHelper
    {
        public static uint getGregorianUDN(int year, int month, int day)
        {
            // pre
            int jdYear = (year > 0) ? year : 1;
            int jdMonth = (month > 0) ? month : 1;
            int jdDay = (day > 0) ? day : 1;

            uint result = (uint)CalendarConverter.gregorian_to_jd(year, month, day);

            bool unkYear = (year < 1);
            bool unkMonth = (month < 1);
            bool unkDay = (day < 1);
            
            if (unkYear) result |= (1u << 31);
            if (unkMonth) result |= (1u << 30);
            if (unkDay) result |= (1u << 29);
            
            return result;
        }

        public static uint getJulianUDN(int year, int month, int day)
        {
            // pre
            int jdYear = (year > 0) ? year : 1;
            int jdMonth = (month > 0) ? month : 1;
            int jdDay = (day > 0) ? day : 1;

            uint result = (uint)CalendarConverter.julian_to_jd(year, month, day);

            bool unkYear = (year < 1);
            bool unkMonth = (month < 1);
            bool unkDay = (day < 1);
            
            if (unkYear) result |= (1u << 31);
            if (unkMonth) result |= (1u << 30);
            if (unkDay) result |= (1u << 29);
            
            return result;
        }
        
        
    }
}
