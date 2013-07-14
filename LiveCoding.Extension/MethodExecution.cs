using GalaSoft.MvvmLight;

namespace LiveCoding.Extension
{
    internal sealed class MethodExecution : ViewModelBase
    {
        private MethodExecutionState _state;

        public MethodExecutionState State
        {
            get { return _state; }
            set
            {
                if ( _state != value )
                {
                    _state = value;
                    RaisePropertyChanged( () => State );
                }
            }
        }
    }
}