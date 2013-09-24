using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LiveCoding.Core;
using LiveCoding.Extension.Extensions;
using NLog;

namespace LiveCoding.Extension.ViewModels
{
	internal sealed class LoggingCodeCompiler : ICodeCompiler
	{
		private readonly ICodeCompiler _inner;
		private readonly Logger _logger;

		public LoggingCodeCompiler( [NotNull] ICodeCompiler inner )
		{
			if ( inner == null )
			{
				throw new ArgumentNullException( "inner" );
			}
			_inner = inner;
			_logger = LogManager.GetLogger( inner.GetType().FullName );
		}

		public void Dispose()
		{
			_inner.Dispose();
		}

		public void SetupScriptEngine( List<string> namespaces, List<string> references )
		{
			_logger.Trace( "Namespaces: {0}", namespaces.Join( ", " ) );
			_logger.Trace( "References: {0}", references.Join( ", " ) );

			_inner.SetupScriptEngine( namespaces, references );
		}

		public object Compile( string code )
		{
			_logger.Trace( "Compiling '{0}'", code );

			return _inner.Compile( code );
		}

		public void SetLiveEventListener( ILiveEventListener listener )
		{
			_inner.SetLiveEventListener( listener );
		}
	}
}