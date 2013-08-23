using System;

namespace LiveCoding.Extension.ViewModels
{
	public sealed class CannotExecuteException : Exception
	{
		public CannotExecuteException() { }

		public CannotExecuteException( string message ) : base( message ) { }
	}
}