using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class ExtListTests
    {
        [Test]
        public void Test_Common()
        {
            using (ExtList<object> list = new ExtList<object>(true))
            {
                Assert.IsNotNull(list);
            }

            using (ExtList<object> list = new ExtList<object>())
            {
                Assert.IsNotNull(list);
                Assert.AreEqual(0, list.Count);

                Assert.Throws(typeof(ListException), () => { list[-1] = null; });

                object obj = new object();
                object obj1 = new object();

                list.Add(obj);
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(obj, list[0]);
                Assert.AreEqual(0, list.IndexOf(obj));

                list.Delete(0);
                Assert.AreEqual(0, list.Count);

                list.Add(obj);
                Assert.AreEqual(obj, list.Extract(obj));

                list.Insert(0, obj);

                list[0] = obj;
                Assert.AreEqual(obj, list[0]);

                list.Add(null);
                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(null, list[1]);
                list[1] = obj1;
                Assert.AreEqual(obj1, list[1]);

                list[1] = null;
                Assert.AreEqual(2, list.Count);
                list.Pack();
                Assert.AreEqual(1, list.Count);

                list.Remove(obj);
                Assert.AreEqual(0, list.Count);

                Assert.AreEqual(false, list.OwnsObjects);

                list.OwnsObjects = true;
                Assert.AreEqual(true, list.OwnsObjects);

                list.Clear();
                list.Add(obj);
                list.Add(obj1);
                Assert.AreEqual(obj, list[0]);
                Assert.AreEqual(obj1, list[1]);
                list.Exchange(0, 1);
                Assert.AreEqual(obj, list[1]);
                Assert.AreEqual(obj1, list[0]);
            }

            using (ExtList<ValItem> list = new ExtList<ValItem>())
            {
                Assert.IsNotNull(list);

                list.Add(new ValItem(5));
                list.Add(new ValItem(1));
                list.Add(new ValItem(17));
                list.Add(new ValItem(4));

                list.QuickSort(CompareItems);

                list.MergeSort(CompareItems);
            }
        }

        private static int CompareItems(ValItem item1, ValItem item2)
        {
            return item1.Value.CompareTo(item2.Value);
        }
    }
}
