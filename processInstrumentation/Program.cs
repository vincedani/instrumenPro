using System;
using System.Linq;
using processInstrumentation.Models;

namespace processInstrumentation
{
    class Program
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Wrong argument list!");
                Console.WriteLine("Arg: trace.out");
                return;
            }
            // Process the result from instrumentation.
            var worker = new InstrumentationProcesser();
            var firstList = worker.BeginProcess(args[0]);

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

            foreach (var element in secondList)
            {
                // ReSharper disable once UseStringInterpolation
                string row = string.Format("{0} {1} {2} {3} {4}", element.Function, element.Caller,
                    element.Secundum, element.NanoSecundum, element.Count);
                Console.WriteLine(row);
            }
            Console.ReadLine();
        }
    }
}
