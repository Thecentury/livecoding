using System;
using System.Reactive;
using NUnit.Framework;

namespace LiveCoding.Core.Tests
{
	[TestFixture]
	public sealed class ForLoopHandlerTests
	{
		[Test]
		public void Workbench()
		{
			var observer = Observer.Create<ForLoopInfo>( l =>
			{
				Console.WriteLine( "loop started" );
				l.Iterations.Subscribe( i =>
				{
					Console.WriteLine( "iteration started" );
					i.EventsDuringIteration.Subscribe( e =>
					{
						Console.WriteLine( e );
					}, () => Console.WriteLine( "iteration finished" ) );
				}, () => Console.WriteLine( "loop finished" ) );
			}, () => Console.WriteLine( "no more loops" ) );
			ForLoopHandler handler = new ForLoopHandler( observer );
			handler.Accept( new ValueChange() );
			var forLoopStartedEvent = new ForLoopStartedEvent();
			handler.Accept( forLoopStartedEvent );
			handler.Accept( new ForLoopIterationEvent( forLoopStartedEvent.LoopId, 1 ) );
			handler.Accept( new ValueChange { OriginalValue = 1 } );
			handler.Accept( new ForLoopIterationEvent( forLoopStartedEvent.LoopId, 2 ) );
			handler.Accept( new ValueChange() );
			handler.Accept( new ForLoopFinishedEvent( forLoopStartedEvent.LoopId ) );
		}
	}
}
