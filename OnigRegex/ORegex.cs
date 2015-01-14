using System;

namespace OnigRegex
{
    public class ORegex : IDisposable
    {
        private string text;
        private IntPtr regex;
        private IntPtr region;
        private bool regionSet = false;
        private bool disposed = false;

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
            if (!disposed)
            {
                disposed = true;
                OnigInterop.onigwrap_free(regex);
                if (regionSet)
                    OnigInterop.onigwrap_region_free(region);
            }
        }
    }
}