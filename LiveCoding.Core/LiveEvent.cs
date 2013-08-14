using System;
using System.Threading;

namespace LiveCoding.Core
{
	public abstract class LiveEvent
	{
		protected LiveEvent()
		{
			TimestampUtc = DateTime.UtcNow;
			ThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		public int ThreadId { get; private set; }

		public DateTime TimestampUtc { get; private set; }

		public string MethodName { get; set; }
	}
}