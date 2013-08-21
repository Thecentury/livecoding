namespace LiveCoding.Core.Internal
{
	internal abstract class SpecificLiveEventWatcher<T> : LiveEventWatcher where T : LiveEvent
	{
		public sealed override bool Accept( LiveEvent evt )
		{
			T castEvent = evt as T;
			if ( castEvent == null )
			{
				return false;
			}
			else
			{
				AcceptCore( castEvent );
				return true;
			}
		}

		protected abstract void AcceptCore( T evt );
	}
}