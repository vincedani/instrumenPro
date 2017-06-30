using System;
using System.IO;
using System.Linq;
using processInstrumentation.Models;

namespace processInstrumentation
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("--parse"))
            {
                if (args.Length != 3 && !args[0].Equals("--parse"))
                {
                    Console.Error.WriteLine("Wrong argument list!");
                    Console.Error.WriteLine("Arg: --parce result_1 result_2");
                    return;
                }

                var parser = new Parser();
                var parsedList = parser.Parse(args[1], args[2]);

                foreach (var element in parsedList)
                {
                    Console.WriteLine(string.Concat(
                        element.FirstSide.Count, " ^ ",
                        element.FirstSide.Secundum, " ^ ",
                        element.FirstSide.NanoSecundum, " ^ ",
                        element.SecondSide.Count, " ^ ",
                        element.SecondSide.Secundum, " ^ ",
                        element.SecondSide.NanoSecundum, "^ ",
                        element.Function, " ^ ",
                        element.Caller, " ^ "));
                }
            }
            else
            {
                if (args.Length != 1)
                {
                    Console.Error.WriteLine("Wrong argument list!");
                    Console.Error.WriteLine("Arg: trace.out");
                    return;
                }

                var startTime = DateTime.Now;
                // Process the result from instrumentation.
                var worker = new InstrumentationProcesser();
                var firstList = worker.BeginProcess(args[0]);

                var processedTime = DateTime.Now;
                // Normalize the processed results.
                var secondList = worker.NormalizeResults(
                    (from element in firstList
                     let keySplit = element.Key.Split(' ')
                     select new NormalizedRowType
                     {
                         Function = keySplit[0],
                         Caller = keySplit[1],
                         Secundum = element.Value.Summary.Secundum,
                         NanoSecundum = element.Value.Summary.NanoSecundum,
                         Count = element.Value.Count
                     }).ToList());

                var normalizedTime = DateTime.Now;
                foreach (var element in secondList)
                {
                    string row = string.Format("{0} {1} {2} {3} {4}", element.Function, element.Caller,
                        element.Secundum, element.NanoSecundum, element.Count);
                    Console.WriteLine(row);
                }

                // Time summary
                Console.Error.WriteLine(string.Concat("Process time: ", (processedTime - startTime).TotalMilliseconds));
                Console.Error.WriteLine(string.Concat("Normalization time: ", (normalizedTime - processedTime).TotalMilliseconds));
                Console.Error.WriteLine(string.Concat("Total time: ", (DateTime.Now - startTime).TotalMilliseconds));
                Console.ReadLine();
            }
        }
    }
}
