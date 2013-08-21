using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LiveCoding.Core
{
	public sealed class ForLoopInfo
	{
		public ForLoopInfo( int loopStartLineNumber )
		{
			LoopStartLineNumber = loopStartLineNumber;
			_iterations.Subscribe( i => IterationsCached.Add( i ) );
		}

		public int LoopStartLineNumber { get; private set; }

		private readonly Subject<ForLoopIteration> _iterations = new Subject<ForLoopIteration>();
		private readonly List<ForLoopIteration> _iterationsCached = new List<ForLoopIteration>(); 

		internal IObserver<ForLoopIteration> IterationsInternal
		{
			get { return _iterations; }
		}

		public IObservable<ForLoopIteration> Iterations
		{
			get { return _iterations; }
		}

		internal List<ForLoopIteration> IterationsCached
		{
			get { return _iterationsCached; }
		}
	}
}