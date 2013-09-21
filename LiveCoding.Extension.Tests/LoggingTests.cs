using NLog;
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
			Target t = new LogentriesTarget
			{
				Token = "18bd643b-3620-41b3-aff1-4a4d40079e59",
				Debug = true,
				HttpPut = true,
				Ssl = false,
				Name = "logentries",
				Layout = new SimpleLayout( "${date:format=ddd MMM dd} ${time:format=HH:mm:ss} ${date:format=zzz yyyy} [${machinename}] [${threadid}] ${logger} ${LEVEL} ${message} ${exception:format=tostring}" )
			};

			LogManager.Configuration = new LoggingConfiguration();

			LogManager.Configuration.AddTarget( "logentries", t );
			LogManager.Configuration.LoggingRules.Add( new LoggingRule( "*", LogLevel.Trace, t ) ); 
			
			Logger logger = LogManager.GetCurrentClassLogger();
			logger.Info( "123" );
		}
	}
}