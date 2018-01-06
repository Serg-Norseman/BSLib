using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class EnumSetTests
    {
        private enum RestrictionEnum
        {
            rnNone,
            rnLocked,
            rnConfidential,
            rnPrivacy,

            rnLast = rnPrivacy
        }

        [Test]
        public void Test_Common()
        {
            EnumSet<RestrictionEnum> es = EnumSet<RestrictionEnum>.Create();
            Assert.IsTrue(es.IsEmpty());

            es.Include(null);
            Assert.IsTrue(es.IsEmpty());

            es.Include(RestrictionEnum.rnPrivacy, RestrictionEnum.rnLocked);
            Assert.IsTrue(es.Contains(RestrictionEnum.rnPrivacy));
            Assert.IsFalse(es.Contains(RestrictionEnum.rnNone));
            Assert.IsFalse(es.IsEmpty());

            es.Exclude(RestrictionEnum.rnPrivacy);
            Assert.IsFalse(es.Contains(RestrictionEnum.rnPrivacy));
            Assert.IsTrue(es.Contains(RestrictionEnum.rnLocked));

            es = EnumSet<RestrictionEnum>.Create(RestrictionEnum.rnNone, RestrictionEnum.rnLocked);
            Assert.IsTrue(es.Contains(RestrictionEnum.rnNone));
            Assert.IsTrue(es.Contains(RestrictionEnum.rnLocked));

            string test = es.ToString().Substring(64-8);
            Assert.AreEqual("00000011", test);

            // clone test
            EnumSet<RestrictionEnum> copy = (EnumSet<RestrictionEnum>)es.Clone();
            test = copy.ToString().Substring(64-8);
            Assert.AreEqual("00000011", test);

            // clear test
            copy.Clear();
            Assert.IsTrue(copy.IsEmpty());

            //
            EnumSet<RestrictionEnum> es2 = EnumSet<RestrictionEnum>.Create(RestrictionEnum.rnNone, RestrictionEnum.rnLocked);

            Assert.IsTrue(es.Equals(es2));
            Assert.IsFalse(es.Equals(null));

            Assert.IsTrue(es.Contains(RestrictionEnum.rnLocked));
            Assert.IsFalse(es.Contains(RestrictionEnum.rnPrivacy));

            EnumSet<RestrictionEnum> es3 = EnumSet<RestrictionEnum>.Create(RestrictionEnum.rnLocked);
            EnumSet<RestrictionEnum> es4 = es * es3;
            Assert.IsTrue(es4.Contains(RestrictionEnum.rnLocked));

            es = EnumSet<RestrictionEnum>.Create(RestrictionEnum.rnNone);
            es2 = EnumSet<RestrictionEnum>.Create(RestrictionEnum.rnLocked);
            Assert.IsTrue(es != es2);

            es = es + es2;
            es3 = EnumSet<RestrictionEnum>.Create(RestrictionEnum.rnNone, RestrictionEnum.rnLocked);
            Assert.IsTrue(es.Equals(es3));

            Assert.IsFalse(es3.ContainsAll(new RestrictionEnum[] {}));
            Assert.IsTrue(es3.ContainsAll(RestrictionEnum.rnNone, RestrictionEnum.rnLocked));
            Assert.IsFalse(es3.ContainsAll(RestrictionEnum.rnNone, RestrictionEnum.rnPrivacy));

            Assert.IsFalse(es3.HasIntersect(new RestrictionEnum[] {}));
            Assert.IsTrue(es3.HasIntersect(RestrictionEnum.rnNone, RestrictionEnum.rnPrivacy));
            Assert.IsFalse(es3.HasIntersect(RestrictionEnum.rnPrivacy));

            es = es - es2;
            es3 = EnumSet<RestrictionEnum>.Create(RestrictionEnum.rnNone);
            Assert.IsTrue(es == es3);
            Assert.AreEqual("0000000000000000000000000000000000000000000000000000000000000001", es3.ToString());
            Assert.AreNotEqual(0, es3.GetHashCode());
        }
    }
}
