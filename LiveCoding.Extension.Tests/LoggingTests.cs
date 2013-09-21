using System;
using System.Threading;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NUnit.Framework;

namespace LiveCoding.Extension.Tests
{
	[TestFixture]
	public sealed class LoggingTests
	{
		[Test]
		public void WriteToLog()
		{
			var layout = new SimpleLayout( "${date:format=ddd MMM dd} ${time:format=HH:mm:ss} ${date:format=zzz yyyy} [${machinename}] [${threadid}] ${logger} ${LEVEL} ${message} ${exception:format=tostring}" );
			Target t = new LogentriesTarget
			{
				Token = "18bd643b-3620-41b3-aff1-4a4d40079e59",
				Debug = true,
				HttpPut = false,
				Ssl = false,
				Name = "logentries",
				Layout = layout,
			};

			var configuration = new LoggingConfiguration();

			configuration.AddTarget( "logentries", t );
			configuration.LoggingRules.Add( new LoggingRule( "*", LogLevel.Trace, t ) );

			LogManager.Configuration = configuration;

			Logger logger = LogManager.GetCurrentClassLogger();

			t.WriteAsyncLogEvent( new AsyncLogEventInfo( new LogEventInfo( LogLevel.Info, logger.Name, "123" ), e =>
			{
				Console.WriteLine( e );
			} ) );

			for ( int i = 0; i < 10; i++ )
			{
				logger.Info( "123" );
			}

			LogManager.Flush();

			Thread.Sleep( 2000 );
		}
	}
}