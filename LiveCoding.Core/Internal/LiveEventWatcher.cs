using System;

namespace LiveCoding.Core.Internal
{
	internal abstract class LiveEventWatcher : IDisposable
	{
		public abstract bool Accept( LiveEvent evt );

		public virtual void Dispose()
		{
		}
	}
}