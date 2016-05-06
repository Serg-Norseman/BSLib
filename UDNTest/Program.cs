using System;
using System.Collections.Generic;
using GKCommon;

namespace UDNTest
{
    public class UDNRecord
    {
        public readonly UDNCalendarType Calendar;
        public readonly string Description;
        public readonly UDN Value;
        
        public UDNRecord(UDNCalendarType calendar, int year, int month, int day, string description)
        {
            this.Calendar = calendar;
            this.Description = description;
            this.Value = new UDN(calendar, year, month, day);
        }
    }

    class Program
    {
        private static List<UDNRecord> fDates = new List<UDNRecord>();

        public static void Main(string[] args)
        {
            Console.WriteLine("Unified Date Numbers Test");
            Console.WriteLine();
            
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 2016, 05, 05, "2016/05/05 [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 2016, 05, 04, "2016/05/04 [g]"));
            
            fDates.Add(new UDNRecord(UDNCalendarType.ctJulian, 2016, 04, 21, "2016/05/04 [g] = 2016/04/21 [j]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctJulian, 2016, 04, 23, "2016/05/06 [g] = 2016/04/23 [j]"));
            
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 2016, 05, UDN.UnknownDay, "2016/05/?? [g]")); // must be first
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 2016, 06, UDN.UnknownDay, "2016/06/?? [g]")); // must be last

            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, UDN.UnknownYear, UDN.UnknownMonth, UDN.UnknownDay, "??/??/?? [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, UDN.UnknownYear, 04, 23, "??/04/23 [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, UDN.UnknownYear, 03, 23, "??/03/23 [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, UDN.UnknownYear, UDN.UnknownMonth, 23, "??/??/23 [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 2016, UDN.UnknownMonth, UDN.UnknownDay, "2016/??/?? [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 2016, UDN.UnknownMonth, 10, "2016/??/10 [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 2015, 03, 23, "2015/03/23 [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 2014, UDN.UnknownMonth, 23, "2014/??/23 [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 2016, 05, 31, "2016/05/31 [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 2016, 05, 31, "2016/05/31 [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, -4712, 1, 2, "-4712/01/02 [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, -4712, 1, 3, "-4712/01/03 [g]"));

            fDates.Add(new UDNRecord(UDNCalendarType.ctHebrew, 5564, 04, 04, "1804/06/13 [g] = 5564/04/04 [h]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctIslamic, 1216, 01, 04, "1801/05/17 [g] = 1216/01/04 [i]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 1802, 05, 01, "1802/05/01 [g]"));

            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 0, 1, 3, "0000/01/03 [g]"));
            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, -1, 1, 3, "-0001/01/03 [g]"));

            fDates.Add(new UDNRecord(UDNCalendarType.ctGregorian, 1, 1, 3, "0001/01/03 [g]"));

            fDates.Sort(delegate(UDNRecord left, UDNRecord right) { return left.Value.CompareTo(right.Value); });

            int[] widths = {16, 16, 12, 32};
            string format = string.Format(
                "{{0, {0}}}\t{{1, {1}}}\t{{2, {2}}}\t{{3, {3}}}", widths[0], widths[1], widths[2], widths[3]);
            Object[] a = {"Value", "Unmasked value", "Calendar", "Description"};
            Console.WriteLine(format, a);
            a = new Object[]
            {
                new string('-', widths[0]),
                new string('-', widths[1]),
                new string('-', widths[2]),
                new string('-', widths[3])
            };
            Console.WriteLine(format, a);
            foreach (UDNRecord udn_rec in fDates)
            {
                a = new Object[] {udn_rec.Value, udn_rec.Value.GetUnmaskedValue(), udn_rec.Calendar.ToString(), udn_rec.Description};
                Console.WriteLine(format, a);
            }
            
            Console.WriteLine();
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}