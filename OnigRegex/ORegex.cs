using System;

namespace OnigRegex
{
    public class ORegex : IDisposable
    {
        private string Text;
        private IntPtr Regex;
        private IntPtr Region;
        private bool RegionSet = false;

        public ORegex(string pattern, bool ignoreCase = true)
        {
            int ignoreCaseArg = ignoreCase ? 1 : 0;

            Regex = OnigInterop.onigwrap_create(pattern, pattern.Length * 2, ignoreCaseArg);
        }

        public int IndexIn(string text, int offset = 0)
        {
            return OnigInterop.onigwrap_index_in(Regex, text, offset * 2, text.Length * 2);
        }

        public void Search(string text, int offset = 0)
        {
            Text = text;
            if (RegionSet)
                OnigInterop.onigwrap_region_free(Region);

            Region = OnigInterop.onigwrap_search(Regex, text, offset * 2, text.Length * 2);
            RegionSet = true;
        }

        public int MatchPosition(int nth)
        {
            if (!RegionSet)
                return -1;

            return OnigInterop.onigwrap_pos(Region, nth);
        }

        public int MatchLength(int nth)
        {
            if (!RegionSet)
                return -1;

            return OnigInterop.onigwrap_len(Region, nth);
        }

        public string Capture(int nth)
        {
            if (!RegionSet)
                return null;

            var pos = OnigInterop.onigwrap_pos(Region, nth);
            if (pos < 0)
                return null;

            var len = OnigInterop.onigwrap_len(Region, nth);

            return Text.Substring(pos, len);
        }

        public void Dispose()
        {
            OnigInterop.onigwrap_free(Regex);
            if (RegionSet)
                OnigInterop.onigwrap_region_free(Region);
        }
    }
}