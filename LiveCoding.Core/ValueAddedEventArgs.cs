using System;

namespace LiveCoding.Core
{
	public sealed class ValueAddedEventArgs : EventArgs
	{
		public ValueAddedEventArgs( ValueChange addedValue )
		{
			AddedValue = addedValue;
		}

		public ValueChange AddedValue { get; private set; }
	}
}