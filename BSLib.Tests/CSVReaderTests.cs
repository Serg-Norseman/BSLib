using System.Collections.Generic;
using System.Data;
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class CSVReaderTests
    {
        [Test]
        public void Test_Common()
        {
            using (CSVReader csv = CSVReader.CreateFromString(TestStubs.CSVData)) {
                List<object> row;

                row = csv.ReadRow(); // header

                row = csv.ReadRow();
                Assert.AreEqual(12, row[0]);
                Assert.AreEqual("alpha", row[1]);
                Assert.AreEqual(12.5f, row[2]);
                Assert.AreEqual(15.4f, row[3]);

                row = csv.ReadRow();
                Assert.AreEqual(15, row[0]);
                Assert.AreEqual("beta", row[1]);
                Assert.AreEqual(15.4f, row[2]);
                Assert.AreEqual(3.7f, row[3]);

                row = csv.ReadRow();
                Assert.AreEqual(2100, row[0]);
                Assert.AreEqual("gamma delta", row[1]);
                Assert.AreEqual(21.5f, row[2]);
                Assert.AreEqual(1.02f, row[3]);

                row = csv.ReadRow();
                Assert.AreEqual(91000, row[0]);
                Assert.AreEqual("omega", row[1]);
                Assert.AreEqual(21.5f, row[2]);
                Assert.AreEqual(1.02f, row[3]);

                row = csv.ReadRow();
                Assert.IsNull(row);
            }

            using (CSVReader csv = CSVReader.CreateFromString(TestStubs.CSVData)) {
                DataTable tbl = csv.CreateDataTable(true);
                Assert.AreEqual(4, tbl.Rows.Count);
                Assert.AreEqual(4, tbl.Columns.Count);

                DataRow row = tbl.Rows[0];
                Assert.AreEqual(12, row[0]);
                Assert.AreEqual("alpha", row[1]);
                Assert.AreEqual(12.5f, row[2]);
                Assert.AreEqual(15.4f, row[3]);

                row = tbl.Rows[1];
                Assert.AreEqual(15, row[0]);
                Assert.AreEqual("beta", row[1]);
                Assert.AreEqual(15.4f, row[2]);
                Assert.AreEqual(3.7f, row[3]);

                row = tbl.Rows[2];
                Assert.AreEqual(2100, row[0]);
                Assert.AreEqual("gamma delta", row[1]);
                Assert.AreEqual(21.5f, row[2]);
                Assert.AreEqual(1.02f, row[3]);

                row = tbl.Rows[3];
                Assert.AreEqual(91000, row[0]);
                Assert.AreEqual("omega", row[1]);
                Assert.AreEqual(21.5f, row[2]);
                Assert.AreEqual(1.02f, row[3]);
            }
        }
    }
}
