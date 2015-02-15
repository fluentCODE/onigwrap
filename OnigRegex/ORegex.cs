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

        public ORegex(string pattern, bool ignoreCase = true)
        {
            int ignoreCaseArg = ignoreCase ? 1 : 0;

            regex = OnigInterop.onigwrap_create(pattern, pattern.Length * 2, ignoreCaseArg);
        }

        public int IndexIn(string text, int offset = 0)
        {
            if (disposed)
                throw new ObjectDisposedException("ORegex");

            return OnigInterop.onigwrap_index_in(regex, text, offset * 2, text.Length * 2);
        }

        /// <summary>
        /// Performs a thread safe search and returns the results in a list
        /// </summary>
        /// <param name="text">The text to search</param>
        /// <param name="offset">An offset from which to start</param>
        /// <param name="resultList">A List to pass the results into. If omitted, SafeSearch creates a new list.</param>
        /// <returns></returns>
        public List<ORegexResult> SafeSearch(string text, int offset = 0, List<ORegexResult> resultList = null)
        {
            if (resultList == null)
                resultList = new List<ORegexResult>();

            lock(this)
            {
                Search(text, offset);

                var captureCount = OnigInterop.onigwrap_num_regs(region);
                for (var capture = 0; capture < captureCount; capture++)
                {
                    var pos = MatchPosition(capture);
                    if (capture == 0 && pos == -1)
                        break;

                    resultList.Add(new ORegexResult()
                    {
                        Position = pos,
                        Length = pos == -1 ? 0 : MatchLength(capture)
                    });
                }

                this.text = null;
                OnigInterop.onigwrap_region_free(region);
                regionSet = false;
            }
            return resultList;
        }

        public void Search(string text, int offset = 0)
        {
            if (disposed)
                throw new ObjectDisposedException("ORegex");

            this.text = text;
            if (regionSet)
                OnigInterop.onigwrap_region_free(region);

            region = OnigInterop.onigwrap_search(regex, text, offset * 2, text.Length * 2);
            regionSet = true;
        }

        public int MatchPosition(int nth)
        {
            if (disposed)
                throw new ObjectDisposedException("ORegex");

            if (!regionSet)
                throw new InvalidOperationException("ORegex.MatchPosition requires that ORegex.Search be run first.");

            return OnigInterop.onigwrap_pos(region, nth);
        }

        public int MatchLength(int nth)
        {
            if (disposed)
                throw new ObjectDisposedException("ORegex");

            if (!regionSet)
                throw new InvalidOperationException("ORegex.MatchLength requires that ORegex.Search be run first.");

            return OnigInterop.onigwrap_len(region, nth);
        }

        public string Capture(int nth)
        {
            if (disposed)
                throw new ObjectDisposedException("ORegex");

            if (!regionSet)
                throw new InvalidOperationException("ORegex.MatchLength requires that ORegex.Search be run first.");

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