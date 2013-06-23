namespace LiveCoding.Core
{
    public sealed class ValueChange
    {
        public string VariableName { get; set; }

        public object Value { get; set; }

        public string MethodName { get; set; }

        public int LineNumber { get; set; }

        public string FilePath { get; set; }
    }
}