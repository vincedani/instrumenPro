using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using processInstrumentation.Models;

namespace processInstrumentation
{
    internal class Parser
    {
        public IEnumerable<ParsedRowType> Parse(string firstFileName, string secondFileName)
        {
            var parsedList = new List<ParsedRowType>();

            using (var firstSide = new FileStream(firstFileName, FileMode.Open))
            {
                using (var reader = new StreamReader(firstSide))
                {
                    string row;
                    while ((row = reader.ReadLine()) != null)
                    {
                        var columns = row.Split('^');

                        parsedList.Add(new ParsedRowType
                        {
                            FirstSide =
                            {
                                Count = Convert.ToUInt64(columns[0].Trim()),
                                Secundum = Convert.ToUInt64(columns[3].Trim()),
                                NanoSecundum = Convert.ToUInt64(columns[4].Trim())
                            },
                            Function = columns[1].Trim(),
                            Caller = columns[2].Trim()
                        });
                    }
                }
            }

            using (var firstSide = new FileStream(secondFileName, FileMode.Open))
            {
                using (var reader = new StreamReader(firstSide))
                {
                    string row;
                    while ((row = reader.ReadLine()) != null)
                    {
                        var columns = row.Split('^');
                        string function = columns[1].Trim();
                        string caller = columns[2].Trim();

                        var elementInList =
                            parsedList.Where(x => x.Function.Equals(function) && x.Caller.Equals(caller));

                        if (elementInList.Any())
                        {
                            var element = elementInList.First();
                            element.SecondSide.Count = Convert.ToUInt64(columns[0].Trim());
                            element.SecondSide.Secundum = Convert.ToUInt64(columns[3].Trim());
                            element.SecondSide.NanoSecundum = Convert.ToUInt64(columns[4].Trim());
                        }
                        else
                        {
                            parsedList.Add(new ParsedRowType
                            {
                                SecondSide = 
                                {
                                    Count = Convert.ToUInt64(columns[0].Trim()),
                                    Secundum = Convert.ToUInt64(columns[3].Trim()),
                                    NanoSecundum = Convert.ToUInt64(columns[4].Trim())
                                },
                                Function = columns[1].Trim(),
                                Caller = columns[2].Trim()
                            });
                        }
                    }
                }
            }

            return parsedList.OrderBy(x => x.FirstSide.Secundum).ThenBy(x => x.FirstSide.NanoSecundum);
        }
    }
}
