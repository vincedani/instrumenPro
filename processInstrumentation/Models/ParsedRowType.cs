namespace processInstrumentation.Models
{
    internal class ParsedRowType
    {
        public string Function;
        public string Caller;

        public TimeAndCount FirstSide;
        public TimeAndCount SecondSide;

        public ParsedRowType()
        {
            FirstSide = new TimeAndCount();
            SecondSide = new TimeAndCount();
        }
    }
}
