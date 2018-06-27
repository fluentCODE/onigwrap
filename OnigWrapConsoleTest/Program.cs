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
            var text = args?.Length == 0 ? "abcABC123" : args[0];
            var pattern =  args?.Length < 2 ? @"\w(\p{L})(?=(\d))" : args[1];
            var start_position = args?.Length < 3 ? 0 : int.Parse(args[2]);

            Console.WriteLine("Building ORegex({0})", text);

            using (var re = new ORegex(pattern, false))
            {
                Console.WriteLine("Looking for '{0}' in '{1}', starting at position {2}...", pattern, text, start_position);
                var result = re.SafeSearch(text, start_position);
                if (!result.Any())
                {
                    Console.WriteLine("No matches found.");
                }
                else
                {
                    foreach (var tuple in result.Select((i, m) => Tuple.Create(m, i)))
                    {
                        var index = tuple.Item1;
                        var match = tuple.Item2;
                        Console.WriteLine("Found capture group {3} at position {0} with length {1}: {2}", match.Position, match.Length, text.Substring(match.Position, match.Length), index);
                    }
                }
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // when running through Visual Studio, the console output window disappears immediately when the application exists, so prompt the user to continue
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
