using System;
using System.Collections.Generic;
using GKCommon;

namespace UDNTest
{
    public enum CalendarType { ctGregorian, ctJulian, ctHebrew, ctIslamic }
    
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
                    this.Value = UDNHelper.GetGregorianUDN(year, month, day);
                    break;

                case CalendarType.ctJulian:
                    this.Value = UDNHelper.GetJulianUDN(year, month, day);
                    break;

                case CalendarType.ctHebrew:
                    this.Value = UDNHelper.GetHebrewUDN(year, month, day);
                    break;

                case CalendarType.ctIslamic:
                    this.Value = UDNHelper.GetIslamicUDN(year, month, day);
                    break;
            }
        }
    }

    class Program
    {
        private static List<UDN> fDates = new List<UDN>();

        public static void Main(string[] args)
        {
            Console.WriteLine("Unified Date Numbers Test");
            Console.WriteLine();
            
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, 05, "2016/05/05 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, 04, "2016/05/04 [g]"));
            
            fDates.Add(new UDN(CalendarType.ctJulian, 2016, 04, 21, "2016/05/04 [g] = 2016/04/21 [j]"));
            fDates.Add(new UDN(CalendarType.ctJulian, 2016, 04, 23, "2016/05/06 [g] = 2016/04/23 [j]"));
            
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, UDNHelper.UnknownDay, "2016/05/?? [g]")); // must be first
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 06, UDNHelper.UnknownDay, "2016/06/?? [g]")); // must be last

            fDates.Add(new UDN(CalendarType.ctGregorian, UDNHelper.UnknownYear, UDNHelper.UnknownMonth, UDNHelper.UnknownDay, "??/??/?? [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, UDNHelper.UnknownYear, 04, 23, "??/04/23 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, UDNHelper.UnknownYear, 03, 23, "??/03/23 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, UDNHelper.UnknownYear, UDNHelper.UnknownMonth, 23, "??/??/23 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, UDNHelper.UnknownMonth, UDNHelper.UnknownDay, "2016/??/?? [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, UDNHelper.UnknownMonth, 10, "2016/??/10 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2015, 03, 23, "2015/03/23 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2014, UDNHelper.UnknownMonth, 23, "2014/??/23 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, 31, "2016/05/31 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 2016, 05, 31, "2016/05/31 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, -4712, 1, 2, "-4712/01/02 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, -4712, 1, 3, "-4712/01/03 [g]"));

            fDates.Add(new UDN(CalendarType.ctHebrew, 5564, 04, 04, "1804/06/13 [g] = 5564/04/04 [h]"));
            fDates.Add(new UDN(CalendarType.ctIslamic, 1216, 01, 04, "1801/05/17 [g] = 1216/01/04 [i]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, 1802, 05, 01, "1802/05/01 [g]"));

            fDates.Add(new UDN(CalendarType.ctGregorian, 0, 1, 3, "0000/01/03 [g]"));
            fDates.Add(new UDN(CalendarType.ctGregorian, -1, 1, 3, "-0001/01/03 [g]"));

            fDates.Add(new UDN(CalendarType.ctGregorian, 1, 1, 3, "0001/01/03 [g]"));

            fDates.Sort(delegate(UDN left, UDN right) { return UDNHelper.CompareUDN(left.Value, right.Value); });
            
            foreach (UDN udn in fDates)
            {
                Object[] a = {udn.Value, (UDNHelper.ValueMask & udn.Value), udn.Calendar.ToString(), udn.Description};
                Console.WriteLine("Value: {0, 12}\t(unmasked value: {1, 12})\t{2}\t{3}", a);
            }
            
            Console.WriteLine();
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}