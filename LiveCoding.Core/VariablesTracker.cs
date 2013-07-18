using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

        public static void AddValue(string variableName, object value, int originalLineNumber = 0, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            _changes.Add(new ValueChange
            {
                VariableName = variableName,
                FilePath = filePath,
                LineNumber = lineNumber,
				OriginalLineNumber = originalLineNumber,
                MethodName = methodName,
                Value = value,
                TimestampUtc = DateTime.UtcNow
            });
        }
    }
}
