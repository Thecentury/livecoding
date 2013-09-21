using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace LiveCoding.Extension
{
	public static class NlogBootstrapper
	{
		private static volatile bool _initialized;
		private static readonly object _lock = new object();

		public static void Initialize()
		{
			if ( _initialized )
			{
				return;
			}
			lock ( _lock )
			{
				if ( _initialized )
				{
					return;
				}

				var layout = new SimpleLayout( "${level} [${machinename}] [${pad:padding=3:${threadid}}] ${longdate}	${logger:shortName=True} ${message} ${exception:format=ToString}" );
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

				_initialized = true;
			}
		}
	}
}