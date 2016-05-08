#define LEFT_AND_RIGHT_BORDERS
#define TOP_AND_BOTTOM_BORDERS

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

        public UDNRecord(UDN udn, UDNCalendarType calendar, string description)
        {
            this.Calendar = calendar;
            this.Description = description;
            this.Value = udn;
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

            // Add dates before.
            fDates.Add(new UDNRecord(UDN.CreateBefore(
                UDNCalendarType.ctGregorian, 1, 1, 4), UDNCalendarType.ctGregorian, "before 0001/01/04 [g]"));
            fDates.Add(new UDNRecord(UDN.CreateBefore(
                UDNCalendarType.ctGregorian, 2016, 05, 31), UDNCalendarType.ctGregorian, "before 2016/05/31 [g]"));
            fDates.Add(new UDNRecord(UDN.CreateBefore(
                UDNCalendarType.ctGregorian, -4712, 1, 2), UDNCalendarType.ctGregorian, "before -4712/01/02 [g]"));
            fDates.Add(new UDNRecord(UDN.CreateBefore(
                UDNCalendarType.ctGregorian, UDN.UnknownYear, 05, 31), UDNCalendarType.ctGregorian, "before ????/05/31 [g]"));
            fDates.Add(new UDNRecord(UDN.CreateBefore(
                UDNCalendarType.ctGregorian, 2015, UDN.UnknownMonth, 31), UDNCalendarType.ctGregorian, "before 2015/??/31 [g]"));
            // Add dates after.
            fDates.Add(new UDNRecord(UDN.CreateAfter(
                UDNCalendarType.ctGregorian, 2016, 05, 31), UDNCalendarType.ctGregorian, "after 2016/05/31 [g]"));
            fDates.Add(new UDNRecord(UDN.CreateAfter(
                UDNCalendarType.ctGregorian, UDN.UnknownYear, 05, 31), UDNCalendarType.ctGregorian, "after ????/05/31 [g]"));
            fDates.Add(new UDNRecord(UDN.CreateAfter(
                UDNCalendarType.ctGregorian, UDN.UnknownYear, 06, 15), UDNCalendarType.ctGregorian, "after ????/06/15 [g]"));
            fDates.Add(new UDNRecord(UDN.CreateAfter(
                UDNCalendarType.ctGregorian, 2015, UDN.UnknownMonth, 31), UDNCalendarType.ctGregorian, "after 2015/??/31 [g]"));
            fDates.Add(new UDNRecord(UDN.CreateAfter(
                UDNCalendarType.ctGregorian, 2015, UDN.UnknownMonth, 30), UDNCalendarType.ctGregorian, "after 2015/??/30 [g]"));

            fDates.Sort(delegate(UDNRecord left, UDNRecord right) { return left.Value.CompareTo(right.Value); });

            int[] widths = {16, 16, 12, 32};
            string format =
#if LEFT_AND_RIGHT_BORDERS
                string.Format("{{4}} {{0, {0}}} {{4}} {{1, {1}}} {{4}} {{2, {2}}} {{4}} {{3, {3}}} {{4}}", widths[0], widths[1], widths[2], widths[3]);
#else
                string.Format("{{0, {0}}} {{4}} {{1, {1}}} {{4}} {{2, {2}}} {{4}} {{3, {3}}}", widths[0], widths[1], widths[2], widths[3]);
#endif
            Object[] a =
            {
                new string('-', widths[0]),
                new string('-', widths[1]),
                new string('-', widths[2]),
                new string('-', widths[3]),
                new string('+', 1)
            };
            string delimiter = string.Format(format, a);
#if TOP_AND_BOTTOM_BORDERS
            Console.WriteLine(delimiter);
#endif
            a = new Object[] {"Value", "Unmasked value", "Calendar", "Description", "|"};
            Console.WriteLine(format, a);
            Console.WriteLine(delimiter);
            foreach (UDNRecord udn_rec in fDates)
            {
                a = new Object[] {udn_rec.Value, udn_rec.Value.GetUnmaskedValue(), udn_rec.Calendar.ToString(), udn_rec.Description, "|"};
                Console.WriteLine(format, a);
            }
#if TOP_AND_BOTTOM_BORDERS
            Console.WriteLine(delimiter);
#endif

            widths = new int[] {32, 32, 48};
            format =
#if LEFT_AND_RIGHT_BORDERS
                string.Format("{{3}} {{0, {0}}} {{3}} {{1, {1}}} {{3}} {{2, {2}}} {{3}}", widths[0], widths[1], widths[2]);
#else
                string.Format("{{0, {0}}} {{3}} {{1, {1}}} {{3}} {{2, {2}}}}", widths[0], widths[1], widths[2]);
#endif
            a = new Object[]
            {
                new string('-', widths[0]),
                new string('-', widths[1]),
                new string('-', widths[2]),
                new string('+', 1)
            };
            delimiter = string.Format(format, a);
#if TOP_AND_BOTTOM_BORDERS
            Console.WriteLine(delimiter);
#endif
            a = new Object[] {"Left", "Right", "Between 'Left' and 'Right'", "|"};
            Console.WriteLine(format, a);
            Console.WriteLine(delimiter);
            for (int i = 0; i < fDates.Count - 1; i++)
            {
                string between;
                try
                {
                    UDN foo = UDN.Between(fDates[i].Value, fDates[i + 1].Value);
                    // Always use Gregorian calendar to show "between-date".
                    int year;
                    int month;
                    int day;
                    CalendarConverter.jd_to_gregorian(foo.GetUnmaskedValue(), out year, out month, out day);
                    // If 'left' or 'right' dates ain't in the Gregorian calendar, show "+f".
                    bool forced =
                        (UDNCalendarType.ctGregorian != fDates[i].Calendar) ||
                        (UDNCalendarType.ctGregorian != fDates[i + 1].Calendar);
                    between = string.Format(
                        "{4} == {0}/{1}/{2} [g{3}]",
                        year,
                        foo.hasKnownMonth() ? month.ToString() : "??",
                        foo.hasKnownDay() ? day.ToString() : "??",
                        forced ? "+f" : "",
                        foo.GetUnmaskedValue());
                }
                catch (Exception e)
                {
                    between = e.Message;
                }
                a = new Object[] {fDates[i].Description, fDates[i + 1].Description, between, "|"};
                Console.WriteLine(format, a);
            }
#if TOP_AND_BOTTOM_BORDERS
            Console.WriteLine(delimiter);
#endif

            Console.WriteLine();
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}