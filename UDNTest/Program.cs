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
        
        private static int CompareItems(UDN item1, UDN item2)
        {
            uint val1 = item1.Value;
            uint val2 = item2.Value;
            
            return val1.CompareTo(val2);
        }
        
        public static void Main(string[] args)
        {
            Console.WriteLine("Unified Date Numbers Test");
            Console.WriteLine();
            
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, 05, "2016/05/05 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, 04, "2016/05/04 [g]"));
            
            fDates.Add(new UDN(CalendarType.ctJulian, 2016, 04, 21, "2016/04/21 [j] = 2016/05/04 [g]"));
            fDates.Add(new UDN(CalendarType.ctJulian, 2016, 04, 23, "2016/04/23 [j] = 2016/05/06 [g]"));
            
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, 00, "2016/05/?? [g]")); // must be first
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 06, 00, "2016/06/?? [g]")); // must be last
            
            fDates.Sort(CompareItems);
            
            foreach (UDN udn in fDates)
            {
                Console.WriteLine(udn.Value + " /// " + udn.Calendar.ToString() + " /// " + udn.Description);
            }
            
            Console.WriteLine();
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}