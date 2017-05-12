using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using processInstrumentation.Models;
// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable AccessToModifiedClosure

namespace processInstrumentation
{
    internal class InstrumentationProcesser
    {
        public List<KeyValuePair<string, InstrumentRowType>> BeginProcess(string fileName)
        {
            var functions = new List<KeyValuePair<string, InstrumentRowType>>();
            var fileStream = new FileStream(fileName, FileMode.Open);

            using (var reader = new StreamReader(fileStream))
            {
                string row;
                while ((row = reader.ReadLine()) != null)
                {
                    try
                    {
                        ProcessRow(functions, row);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(string.Concat("An exception has thrown: ", e.Message,
                            " Row: ", row));
                    }
                } 
            }
            return functions;
        }

        public List<NormalizedRowType> NormalizeResults(List<NormalizedRowType> primaryResult)
        {
            var normalizedResults = new List<NormalizedRowType>();

            foreach (var element in primaryResult)
            {
                var rowsWhereThisCalled = primaryResult.Where(x => x.Caller.Equals(element.Function));

                if (rowsWhereThisCalled.Any())
                {
                    foreach (var callSide in rowsWhereThisCalled)
                    {
                        element.Secundum -= callSide.Secundum;
                        element.NanoSecundum -= callSide.NanoSecundum;
                    }
                }
                normalizedResults.Add(element);
            }
            return normalizedResults;
        }

        private static void ProcessRow(ICollection<KeyValuePair<string, InstrumentRowType>> functions, string row)
        {
            var columns = row.Split(' ');
            string key = string.Concat(columns[1], " ", columns[2]);

            switch (columns[0])
            {
                // "e 0x2518de0 0x306e4b6 1494409547 824698315"
                case "e":
                    var enumerable = functions.Where(x => x.Key.Equals(key));
                    if (enumerable.Any())
                    {
                        OutputTime time;
                        time.Secundum = ulong.Parse(columns[3]);
                        time.NanoSecundum = ulong.Parse(columns[4]);

                        enumerable.First().Value.Stack.Push(time);
                    }
                    else
                    {
                        var temp = new InstrumentRowType();
                        OutputTime time;
                        time.Secundum = ulong.Parse(columns[3]);
                        time.NanoSecundum = ulong.Parse(columns[4]);
                        temp.Stack.Push(time);

                        functions.Add(new KeyValuePair<string, InstrumentRowType>(key, temp));
                    }
                    break;
                // x 0x2518de0 0x306e4b6 1494409547 824777303
                case "x":
                    var iEnumerable = functions.Where(x => x.Key.Equals(key));
                    if (iEnumerable.Any())
                    {
                        var temp = iEnumerable.First();
                        var time = temp.Value.Stack.Pop();
                        temp.Value.Count++;

                        ulong nanoSeconds;
                        ulong seconds = ulong.Parse(columns[3]) - time.Secundum;
                        ulong currentNs = ulong.Parse(columns[4]);

                        if (currentNs > time.NanoSecundum)
                        {
                            nanoSeconds = currentNs - time.NanoSecundum;
                        }
                        else
                        {
                            nanoSeconds = (999999999 - time.NanoSecundum) + currentNs;
                        }

                        ulong overflowCheck = temp.Value.Summary.NanoSecundum + nanoSeconds;
                        if (overflowCheck > 999999999)
                        {
                            temp.Value.Summary.Secundum += seconds + 1;
                            temp.Value.Summary.NanoSecundum = overflowCheck - 999999999;
                        }
                        else
                        {
                            temp.Value.Summary.NanoSecundum += nanoSeconds;
                            temp.Value.Summary.Secundum += seconds;
                        }
                    }
                    break;
            }
        }
    }
}
