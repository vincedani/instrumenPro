using System.Collections.Generic;

namespace processInstrumentation.Models
{
    internal class InstrumentRowType
    {
        public Stack<OutputTime> Stack;
        public OutputTime Summary;
        public uint Count;

        public InstrumentRowType()
        {
            Stack = new Stack<OutputTime>();
        }
    }
}
