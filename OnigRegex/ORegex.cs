using System;
using System.Collections.Generic;

namespace OnigRegex
{
    public class ORegex : IDisposable
    {
        private string text;
        private IntPtr regex;
        private IntPtr region;
        private bool regionSet = false;
        private bool disposed = false;
        private object syncObject = new object();
        private string regexString;

        /// <summary>
        /// Indicates whether or not a search has been run
        /// </summary>
        public bool Ran
        {
            get
            {
                return regionSet;
            }
        }

        public bool Valid
        {
            get
            {
                return regex != IntPtr.Zero;
            }
        }

        public ORegex(string pattern, bool ignoreCase = true, bool multiline = false)
        {
            int ignoreCaseArg = ignoreCase ? 1 : 0;
            int multilineArg = multiline ? 1 : 0;

            regex = OnigInterop.onigwrap_create(pattern, pattern.Length * 2, ignoreCaseArg, multilineArg);

            if (!Valid)
                regexString = pattern; // Save the pattern off on invalid patterns for throwing exceptions
        }

        public int IndexIn(string text, int offset = 0)
        {
            if (disposed) throw new ObjectDisposedException("ORegex");
            if (!Valid) throw new ArgumentException(string.Format("Invalid Onigmo regular expression: {0}", regexString));

            return OnigInterop.onigwrap_index_in(regex, text, offset * 2, text.Length * 2);
        }

        /// <summary>
        /// Performs a thread safe search and returns the results in a list
        /// </summary>
        /// <param name="text">The text to search</param>
        /// <param name="offset">An offset from which to start</param>
        /// <returns>A list of capture group matches</returns>
        public List<ORegexResult> SafeSearch(string text, int offset = 0)
        {
            if (disposed) throw new ObjectDisposedException("ORegex");
            if (!Valid) throw new ArgumentException(string.Format("Invalid Onigmo regular expression: {0}", regexString));

            var resultList = new List<ORegexResult>();

            lock (syncObject)
            {
                Search(text, offset);

                var captureCount = OnigInterop.onigwrap_num_regs(region);
                for (var capture = 0; capture < captureCount; capture++)
                {
                    var pos = OnigInterop.onigwrap_pos(region, capture);
                    if (capture == 0 && pos == -1)
                        break;

                    resultList.Add(new ORegexResult()
                    {
                        Position = pos,
                        Length = pos == -1 ? 0 : OnigInterop.onigwrap_len(region, capture)
                    });
                }

                this.text = null;
                OnigInterop.onigwrap_region_free(region);
                regionSet = false;
            }

            return resultList;
        }

        /// <summary>
        /// Perform a search for the given text starting at the specified offset
        /// </summary>
        /// <param name="text">The text to search</param>
        /// <param name="offset">An offset from which to start</param>
        /// <returns>Nothing. Use MatchPosition and MatchLength to query the results</returns>
        public void Search(string text, int offset = 0)
        {
            if (disposed) throw new ObjectDisposedException("ORegex");
            if (!Valid) throw new ArgumentException(string.Format("Invalid Onigmo regular expression: {0}", regexString));

            this.text = text;
            if (regionSet)
                OnigInterop.onigwrap_region_free(region);

            region = OnigInterop.onigwrap_search(regex, text, offset * 2, text.Length * 2);
            regionSet = true;
        }

        /// <summary>
        /// Get the start match position of the specified capture group
        /// </summary>
        /// <param name="nth">The capture group index</param>
        /// <returns>The position in the string that was searched where the specified capture group was matched</returns>
        public int MatchPosition(int nth)
        {
            if (disposed) throw new ObjectDisposedException("ORegex");
            if (!Valid) throw new ArgumentException(string.Format("Invalid Onigmo regular expression: {0}", regexString));
            if (!regionSet) throw new InvalidOperationException("ORegex.MatchPosition requires that ORegex.Search be run first.");

            return OnigInterop.onigwrap_pos(region, nth);
        }

        /// <summary>
        /// Get the length of the specified capture group match
        /// </summary>
        /// <param name="nth">The capture group index</param>
        /// <returns>The length in the string that was matched inside the specified capture group</returns>
        public int MatchLength(int nth)
        {
            if (disposed) throw new ObjectDisposedException("ORegex");
            if (!Valid) throw new ArgumentException(string.Format("Invalid Onigmo regular expression: {0}", regexString));
            if (!regionSet) throw new InvalidOperationException("ORegex.MatchLength requires that ORegex.Search be run first.");

            return OnigInterop.onigwrap_len(region, nth);
        }

        /// <summary>
        /// Get the text that matched inside the specified capture group
        /// </summary>
        /// <param name="nth">The capture group index</param>
        /// <returns>The text that was matched inside the specified capture group</returns>
        public string Capture(int nth)
        {
            if (disposed) throw new ObjectDisposedException("ORegex");
            if (!Valid) throw new ArgumentException(string.Format("Invalid Onigmo regular expression: {0}", regexString));
            if (!regionSet) throw new InvalidOperationException("ORegex.Capture requires that ORegex.Search be run first.");

            var pos = OnigInterop.onigwrap_pos(region, nth);
            if (pos < 0)
                return null;

            var len = OnigInterop.onigwrap_len(region, nth);

            return text.Substring(pos, len);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (regex != IntPtr.Zero)
                    OnigInterop.onigwrap_free(regex);

                if (regionSet)
                    OnigInterop.onigwrap_region_free(region);
            }
        }

        ~ORegex()
        {
            Dispose(false);
        }
    }
}