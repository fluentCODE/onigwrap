using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnigRegex;

namespace OnigRegexTests
{
    [TestClass]
    public class OnigRegexTests
    {
        [TestMethod]
        public void ORegex_IndexIn_NoOffset()
        {
            using (var r = new ORegex("A"))
            {
                Assert.AreEqual(0, r.IndexIn("A--"));
                Assert.AreEqual(1, r.IndexIn("-A-"));
                Assert.AreEqual(-1, r.IndexIn("---"));
                Assert.AreEqual(2, r.IndexIn("--A"));
            }
        }

        [TestMethod]
        public void ORegex_IndexIn_WithOffset()
        {
            using (var r = new ORegex("A"))
            {
                Assert.AreEqual(-1, r.IndexIn("A--", 1));
                Assert.AreEqual(1, r.IndexIn("AA-", 1));
                Assert.AreEqual(2, r.IndexIn("A-A", 2));
                Assert.AreEqual(2, r.IndexIn("--A", 2));
            }
        }

        [TestMethod]
        public void ORegex_Search()
        {
            using (var r = new ORegex("A"))
            {
                r.Search("-A-");

                Assert.AreEqual(1, r.MatchPosition(0));
                Assert.AreEqual(1, r.MatchLength(0));
                Assert.AreEqual(-1, r.MatchPosition(1));
            }
        }

        [TestMethod]
        public void ORegex_Search_Offset()
        {
            using (var r = new ORegex("A"))
            {
                r.Search("-A-", 1);

                Assert.AreEqual(1, r.MatchPosition(0));
                Assert.AreEqual(1, r.MatchLength(0));
                Assert.AreEqual(-1, r.MatchPosition(1));
            }
        }

        [TestMethod]
        public void ORegex_Search_Offset_Capture()
        {
            using (var r = new ORegex("(A)"))
            {
                r.Search("-A-", 1);

                Assert.AreEqual(1, r.MatchPosition(0));
                Assert.AreEqual(1, r.MatchLength(0));
                Assert.AreEqual(1, r.MatchPosition(1));
                Assert.AreEqual(1, r.MatchLength(1));
                Assert.AreEqual("A", r.Capture(1));

                r.Search("--A", 1);

                Assert.AreEqual(2, r.MatchPosition(0));
                Assert.AreEqual(1, r.MatchLength(0));
                Assert.AreEqual(2, r.MatchPosition(1));
                Assert.AreEqual(1, r.MatchLength(1));
                Assert.AreEqual("A", r.Capture(1));
            }
        }

        [TestMethod]
        public void ORegex_Search_MultipleCaptures()
        {
            using (var r = new ORegex("(A)(.*)"))
            {
                r.Search("---A---");
                Assert.AreEqual(3, r.MatchPosition(0));
                Assert.AreEqual(4, r.MatchLength(0));
                Assert.AreEqual(3, r.MatchPosition(1));
                Assert.AreEqual(1, r.MatchLength(1));
                Assert.AreEqual(4, r.MatchPosition(2));
                Assert.AreEqual(3, r.MatchLength(2));
            }
        }
    }
}
