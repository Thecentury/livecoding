using System;
using System.ComponentModel;

namespace LiveCoding.Extension.ViewModels
{
	public abstract class MethodExecutionStateBase : ISupportInitialize
	{
		private MethodExecutionViewModel _owner;

		public MethodExecutionViewModel Owner
		{
			get { return _owner; }
			set
			{
				if ( value == null )
				{
					throw new ArgumentNullException( "value" );
				}
				_owner = value;
			}
		}

		public virtual void Enter()
		{
		}

		public virtual void Exit()
		{
		}

		public virtual void ExecuteMainAction()
		{
		}

		public abstract MethodExecutionState State { get; }

		public virtual void BeginInit()
		{
		}

		public virtual void EndInit()
		{
		}
	}
}