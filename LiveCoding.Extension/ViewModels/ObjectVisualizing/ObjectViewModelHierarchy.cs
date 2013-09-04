using System.Collections.ObjectModel;

namespace LiveCoding.Extension.ViewModels.ObjectVisualizing
{
	internal sealed class ObjectViewModelHierarchy
	{
		readonly ReadOnlyCollection<ObjectViewModel> _firstGeneration;

		public ObjectViewModelHierarchy( object rootObject )
		{
			ObjectViewModel o = new ObjectViewModel( rootObject );
			_firstGeneration = new ReadOnlyCollection<ObjectViewModel>( new[] { o } );
		}

		public ReadOnlyCollection<ObjectViewModel> FirstGeneration
		{
			get { return _firstGeneration; }
		}
	}
}