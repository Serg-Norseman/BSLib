using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace BSLib
{
    internal class ValItem
    {
        public double Value;

        public ValItem(double value)
        {
            Value = value;
        }
    }

    [TestFixture]
    public class SortHelperTests
    {
        [Test]
        public void Test_Common()
        {
            Assert.Throws(typeof(ArgumentNullException), () => { SortHelper.QuickSort<ValItem>(null, null); });
            Assert.Throws(typeof(ArgumentNullException), () => { SortHelper.MergeSort<ValItem>(null, null); });
            Assert.Throws(typeof(ArgumentNullException), () => { ListTimSort<int>.Sort(null, null); });

            Random rnd = new Random();

            List<ValItem> listQS = new List<ValItem>();
            List<ValItem> listMS = new List<ValItem>();
            List<ValItem> listTS = new List<ValItem>();
            List<ValItem> listCS = new List<ValItem>();

            //const int MaxCount = 1000000; // for performance test
            const int MaxCount = 1000; // for common test

            for (int i = 0; i < MaxCount; i++)
            {
                double val = rnd.NextDouble();

                listTS.Add(new ValItem(val));
                listQS.Add(new ValItem(val));
                listMS.Add(new ValItem(val));
                listCS.Add(new ValItem(val));
            }

            listCS.Sort(CompareItems);

            SortHelper.QuickSort(listQS, CompareItems);

            SortHelper.MergeSort(listMS, CompareItems);

            ListTimSort<ValItem>.Sort(listTS, CompareItems);

            // test for sort valid
            //(only for numbers, because some methods is with the permutations, and part - no)
            for (int i = 0; i < MaxCount; i++)
            {
                Assert.AreEqual(listTS[i].Value, listQS[i].Value);
                Assert.AreEqual(listQS[i].Value, listMS[i].Value);
                Assert.AreEqual(listMS[i].Value, listCS[i].Value);
            }
        }

        private static int CompareItems(ValItem item1, ValItem item2)
        {
            return item1.Value.CompareTo(item2.Value);
        }
    }
}
