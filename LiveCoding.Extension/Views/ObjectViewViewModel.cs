using System.ComponentModel;
using GalaSoft.MvvmLight;
using LiveCoding.Extension.ViewModels.ObjectVisualizing;

namespace LiveCoding.Extension.Views
{
	internal sealed class ObjectViewViewModel : ViewModelBase
	{
		private ObjectViewModelHierarchy _root;
		public ObjectViewModelHierarchy Root
		{
			get { return _root; }
			set
			{
				if ( _root != null )
				{
					foreach ( var objectViewModel in _root.FirstGeneration )
					{
						objectViewModel.PropertyChanged -= OnChildPropertyChanged;
					}
				}

				_root = value;

				if ( _root != null )
				{
					foreach ( var objectViewModel in _root.FirstGeneration )
					{
						objectViewModel.PropertyChanged += OnChildPropertyChanged;
					}
				}

				RaisePropertyChanged( () => Root );
			}
		}

		private void OnChildPropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			if ( e.PropertyName != "IsExpanded" )
			{
				return;
			}
			ObjectViewModel child = (ObjectViewModel)sender;
			IsExpanded = child.IsExpanded;
		}

		private bool _isExpanded;

		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				_isExpanded = value;
				RaisePropertyChanged( () => IsExpanded );
			}
		}
	}
}