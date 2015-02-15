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
                Assert.IsFalse(r.Ran);
                r.Search("-A-");
                Assert.IsTrue(r.Ran);

                Assert.AreEqual(1, r.MatchPosition(0));
                Assert.AreEqual(1, r.MatchLength(0));
                Assert.AreEqual("A", r.Capture(0));

                Assert.AreEqual(-1, r.MatchPosition(1));
                Assert.AreEqual(-1, r.MatchLength(1));
                Assert.AreEqual(null, r.Capture(1));
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
                Assert.AreEqual("A", r.Capture(0));

                Assert.AreEqual(-1, r.MatchPosition(1));
                Assert.AreEqual(-1, r.MatchLength(1));
                Assert.AreEqual(null, r.Capture(1));
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
                Assert.AreEqual("A", r.Capture(0));

                Assert.AreEqual(1, r.MatchPosition(1));
                Assert.AreEqual(1, r.MatchLength(1));
                Assert.AreEqual("A", r.Capture(1));

                Assert.AreEqual(-1, r.MatchPosition(2));
                Assert.AreEqual(-1, r.MatchLength(2));
                Assert.AreEqual(null, r.Capture(2));

                r.Search("--A", 1);

                Assert.AreEqual(2, r.MatchPosition(0));
                Assert.AreEqual(1, r.MatchLength(0));
                Assert.AreEqual("A", r.Capture(0));

                Assert.AreEqual(2, r.MatchPosition(1));
                Assert.AreEqual(1, r.MatchLength(1));
                Assert.AreEqual("A", r.Capture(1));

                Assert.AreEqual(-1, r.MatchPosition(2));
                Assert.AreEqual(-1, r.MatchLength(2));
                Assert.AreEqual(null, r.Capture(2));
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
                Assert.AreEqual("A---", r.Capture(0));

                Assert.AreEqual(3, r.MatchPosition(1));
                Assert.AreEqual(1, r.MatchLength(1));
                Assert.AreEqual("A", r.Capture(1));

                Assert.AreEqual(4, r.MatchPosition(2));
                Assert.AreEqual(3, r.MatchLength(2));
                Assert.AreEqual("---", r.Capture(2));

                Assert.AreEqual(-1, r.MatchPosition(3));
                Assert.AreEqual(-1, r.MatchLength(3));
                Assert.AreEqual(null, r.Capture(3));
            }
        }

        [TestMethod]
        public void ORegex_SafeSearch()
        {
            using (var r = new ORegex("(A)(.*)"))
            {
                var result = r.SafeSearch("---A---");
                Assert.AreEqual(result.Count, 3);

                Assert.AreEqual(3, result[0].Position);
                Assert.AreEqual(4, result[0].Length);

                Assert.AreEqual(3, result[1].Position);
                Assert.AreEqual(1, result[1].Length);

                Assert.AreEqual(4, result[2].Position);
                Assert.AreEqual(3, result[2].Length);
            }
        }

        [TestMethod]
        public void ORegex_SafeSearch_SparseMatches()
        {
            using (var r = new ORegex("((A)|(B))(.*)"))
            {
                var result = r.SafeSearch("---A---");
                Assert.AreEqual(result.Count, 5);

                Assert.AreEqual(-1, result[3].Position);
                Assert.AreEqual(0, result[3].Length);

                Assert.AreEqual(4, result[4].Position);
                Assert.AreEqual(3, result[4].Length);
            }
        }

        [TestMethod]
        public void ORegex_IgnoreCase()
        {
            using (var r = new ORegex("A"))
            {
                r.Search("A");
                Assert.AreEqual(0, r.MatchPosition(0));

                r.Search("a");
                Assert.AreEqual(0, r.MatchPosition(0));
            }

            using (var r = new ORegex("A", true))
            {
                r.Search("A");
                Assert.AreEqual(0, r.MatchPosition(0));

                r.Search("a");
                Assert.AreEqual(0, r.MatchPosition(0));
            }

            using (var r = new ORegex("A", false))
            {
                r.Search("A");
                Assert.AreEqual(0, r.MatchPosition(0));

                r.Search("a");
                Assert.AreEqual(-1, r.MatchPosition(0));
            }
        }

        [TestMethod]
        public void ORegex_NoCaptureExceptions()
        {
            using (var r = new ORegex("A"))
            {
                // No region is defined since we haven't searched
                try
                {
                    r.MatchPosition(0);
                    Assert.Fail();
                }
                catch (InvalidOperationException) { }

                try
                {
                    r.MatchLength(0);
                    Assert.Fail();
                }
                catch (InvalidOperationException) { }

                try
                {
                    r.Capture(0);
                    Assert.Fail();
                }
                catch (InvalidOperationException) { }

                // IndexIn should not create a region. Only Search does that.
                r.IndexIn("A");
                try
                {
                    r.MatchPosition(0);
                    Assert.Fail();
                }
                catch (InvalidOperationException) { }

                try
                {
                    r.MatchLength(0);
                    Assert.Fail();
                }
                catch (InvalidOperationException) { }

                try
                {
                    r.Capture(0);
                    Assert.Fail();
                }
                catch (InvalidOperationException) { }
            }
        }

        [TestMethod]
        public void ORegex_ObjectDisposedExceptions()
        {
            var r = new ORegex("A");
            r.Dispose();

            try
            {
                r.Search("test");
                Assert.Fail();
            }
            catch (ObjectDisposedException) { }

            try
            {
                r.IndexIn("test");
                Assert.Fail();
            }
            catch (ObjectDisposedException) { }

            try
            {
                r.MatchPosition(0);
                Assert.Fail();
            }
            catch (ObjectDisposedException) { }

            try
            {
                r.MatchLength(0);
                Assert.Fail();
            }
            catch (ObjectDisposedException) { }

            try
            {
                r.Capture(0);
                Assert.Fail();
            }
            catch (ObjectDisposedException) { }

            // IDisposable.Dispose() should be idempotent
            try
            {
                r.Dispose();
            }
            catch (ObjectDisposedException)
            {
                Assert.Fail();
            }
        }
    }
}
