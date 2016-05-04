using System;
using System.Collections.Generic;

namespace UDNTest
{
    public enum CalendarType { ctGregorian, ctJulian }
    
    public class UDN
    {
        public readonly CalendarType Calendar;
        public readonly uint Value;
        public readonly string Description;
        
        public UDN(CalendarType calendar, int year, int month, int day, string description)
        {
            this.Calendar = calendar;
            this.Description = description;
            
            switch (calendar)
            {
                case CalendarType.ctGregorian:
                    this.Value = UDNHelper.getGregorianUDN(year, month, day);
                    break;
                    
                case CalendarType.ctJulian:
                    this.Value = UDNHelper.getJulianUDN(year, month, day);
                    break;
            }
        }
    }
    
    class Program
    {
        private static List<UDN> fDates = new List<UDN>();
        
        private static int CompareItems(UDN left, UDN right)
        {
            uint l = left.Value;
            uint r = right.Value;

            if ((UDNHelper.yearMask & l) != UDNHelper.ignoreYear)
            {
                if ((UDNHelper.yearMask & r) != UDNHelper.ignoreYear)
                {
                    return ((int) (UDNHelper.valueMask & l)) - ((int) (UDNHelper.valueMask & r));
                }
                else
                {
                    return 1;
                }
            }
            else if ((UDNHelper.yearMask & r) != UDNHelper.ignoreYear)
            {
                return -1;
            }
            else if ((UDNHelper.monthMask & l) != UDNHelper.ignoreMonth)
            {
                if ((UDNHelper.monthMask & r) != UDNHelper.ignoreMonth)
                {
                    return ((int) (UDNHelper.valueMask & l)) - ((int) (UDNHelper.valueMask & r));
                }
                else
                {
                    return 1;
                }
            }
            else if ((UDNHelper.monthMask & r) != UDNHelper.ignoreMonth)
            {
                return -1;
            }
            else if ((UDNHelper.dayMask & l) != UDNHelper.ignoreDay)
            {
                if ((UDNHelper.dayMask & r) != UDNHelper.ignoreDay)
                {
                    return ((int) (UDNHelper.valueMask & l)) - ((int) (UDNHelper.valueMask & r));
                }
                else
                {
                    return 1;
                }
            }
            else if ((UDNHelper.dayMask & r) != UDNHelper.ignoreDay)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Unified Date Numbers Test");
            Console.WriteLine();
            
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, 05, "2016/05/05 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, 04, "2016/05/04 [g]"));
            
            fDates.Add(new UDN(CalendarType.ctJulian, 2016, 04, 21, "2016/05/04 [g] = 2016/04/21 [j]"));
            fDates.Add(new UDN(CalendarType.ctJulian, 2016, 04, 23, "2016/05/06 [g] = 2016/04/23 [j]"));
            
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, 0, "2016/05/?? [g]")); // must be first
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 06, 0, "2016/06/?? [g]")); // must be last

            fDates.Add(new UDN(CalendarType.ctGregorian, 0, 0, 0, "??/??/?? [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 0, 04, 23, "??/04/23 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 0, 03, 23, "??/03/23 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 0, 0, 23, "??/??/23 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 0, 0, "2016/??/?? [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 0, 10, "2016/??/10 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2015, 03, 23, "2015/03/23 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2014, 0, 23, "2014/??/23 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, 31, "2016/05/31 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, 31, "2016/05/31 [g]"));

            fDates.Sort(CompareItems);
            
            foreach (UDN udn in fDates)
            {
                Object[] a = {udn.Value, (UDNHelper.valueMask & udn.Value), udn.Calendar.ToString(), udn.Description};
                Console.WriteLine("Value: {0, 12}\t(unmasked value: {1, 12})\t{2}\t{3}", a);
            }
            
            Console.WriteLine();
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}