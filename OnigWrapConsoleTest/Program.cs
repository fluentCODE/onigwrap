using OnigRegex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnigWrapConsoleTest
{
    class Program
    {
        /// <summary>
        /// This is not intended as a full test suite. It's just here to easily
        /// test that the dll/so/dylib works on a given environment.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var text = "abcABC123";
            var pattern = "[A-C]+";

            Console.WriteLine("Building ORegex({0})", text);

            using (var re = new ORegex(pattern, false))
            {
                Console.WriteLine("Looking for {0} in {1}", pattern, text);
                Console.WriteLine("Found a match at {0}", re.IndexIn(text));
            }
        }
    }
}
