using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LiveCoding.Core
{
    public static class VariablesTracker
    {
        private static readonly List<ValueChange> _changes = new List<ValueChange>();

        public static IReadOnlyList<ValueChange> Changes
        {
            get { return _changes.AsReadOnly(); }
        }

        public static void ClearRecords()
        {
            _changes.Clear();
        }

        public static void AddValue(string variableName, object value, [CallerMemberName]string methodName = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            _changes.Add(new ValueChange
            {
                VariableName = variableName,
                FilePath = filePath,
                LineNumber = lineNumber,
                MethodName = methodName,
                Value = value,
                TimestampUtc = DateTime.UtcNow
            });
        }
    }
}
