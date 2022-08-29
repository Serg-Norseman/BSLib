using System;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class AlgorithmsTests
    {
        [Test]
        public void Test_CheckBoundsF()
        {
            Assert.AreEqual(5.11f, Algorithms.CheckBounds(5.11f, 3.1f, 11.7f));
            Assert.AreEqual(3.1f, Algorithms.CheckBounds(2f, 3.1f, 11.7f));
            Assert.AreEqual(11.7f, Algorithms.CheckBounds(15f, 3.1f, 11.7f));
        }

        [Test]
        public void Test_CheckBoundsI()
        {
            Assert.AreEqual(5, Algorithms.CheckBounds(5, 3, 11));
            Assert.AreEqual(3, Algorithms.CheckBounds(2, 3, 11));
            Assert.AreEqual(11, Algorithms.CheckBounds(15, 3, 11));
        }

        [Test]
        public void Test_IndexOfT()
        {
            int[] array = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
            Assert.AreEqual(5, ArrayHelper.IndexOf(array, 6));
            Assert.AreEqual(-1, ArrayHelper.IndexOf(array, 11));
        }

        [Test]
        public void Test_ArraysEqualT()
        {
            int[] array1 = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
            int[] array2 = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9};
            int[] array3 = new int[] {1, 2, 3, 4, 5};
            int[] array4 = new int[] {1, 2, 3, 4, 5, 0, 7, 8, 9};
            Assert.AreEqual(true, ArrayHelper.ArraysEqual(array1, array1));
            Assert.AreEqual(true, ArrayHelper.ArraysEqual(array1, array2));

            Assert.AreEqual(false, ArrayHelper.ArraysEqual(array1, null));
            Assert.AreEqual(false, ArrayHelper.ArraysEqual(null, array1));
            Assert.AreEqual(false, ArrayHelper.ArraysEqual(array1, array3));
            Assert.AreEqual(false, ArrayHelper.ArraysEqual(array1, array4));
        }

        [Test]
        public void Test_SwapT()
        {
            int x1 = 5;
            int x2 = 11;
            Assert.AreEqual(5, x1);
            Assert.AreEqual(11, x2);

            Algorithms.Swap(ref x1, ref x2);

            Assert.AreEqual(5, x2);
            Assert.AreEqual(11, x1);
        }

        [Test]
        public void Test_Optional()
        {
            Assert.AreEqual(5, Algorithms.Optional(true, 5));
            Assert.AreEqual(0, Algorithms.Optional(false, 5));
        }

        [Test]
        public void Test_IfElse()
        {
            Assert.AreEqual(5, Algorithms.IfElse(true, 5, 11));
            Assert.AreEqual(11, Algorithms.IfElse(false, 5, 11));
        }
    }
}
