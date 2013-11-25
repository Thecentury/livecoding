using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using LiveCoding.Core;
using NLog;

namespace LiveCoding.Extension.ViewModels
{
	internal sealed class AppDomainCodeCompiler : ICodeCompiler
	{
		private AppDomain _domain;
		private CodeCompiler _compiler;
		private List<string> _namespaces;
		private List<string> _references;
		private readonly List<string> _preludes = new List<string>(); 

		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public void Dispose()
		{
			_logger.Debug( "Unloading domain '{0}'", _domain.FriendlyName );
			_compiler = null;
			AppDomain.Unload( _domain );
		}

		public void SetupScriptEngine( List<string> namespaces, List<string> references )
		{
			if ( namespaces == null )
			{
				throw new ArgumentNullException( "namespaces" );
			}
			if ( references == null )
			{
				throw new ArgumentNullException( "references" );
			}

			_namespaces = namespaces;
			_references = references;

			CreateDomainAndCompiler( namespaces, references );
		}

		private void CreateDomainAndCompiler( List<string> namespaces, List<string> references )
		{
			AppDomainSetup appDomainSetup = new AppDomainSetup
			{
				ApplicationBase = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ),
				ShadowCopyFiles = "true",
				LoaderOptimization = LoaderOptimization.MultiDomain
			};

			string appDomainName = "LiveCodingCompilation_" + Guid.NewGuid().ToString( "N" );

			_logger.Debug( "Going to create new app domain '{0}'", appDomainName );
	
			_domain = AppDomain.CreateDomain( appDomainName, 
				AppDomain.CurrentDomain.Evidence, appDomainSetup, new PermissionSet( PermissionState.Unrestricted ), new StrongName[0] );

			_compiler = _domain.CreateInstanceAndUnwrap<CodeCompiler>();
			_compiler.SetupScriptEngine( namespaces, references );
		}

		public object Compile( string code, bool isPrelude )
		{
			if ( code == null )
			{
				throw new ArgumentNullException( "code" );
			}

			try
			{
				if ( isPrelude )
				{
					_preludes.Add( code );
				}

				return _compiler.Compile( code, isPrelude );
			}
			catch ( RemotingException exc )
			{
				var logger = LogManager.GetCurrentClassLogger();
				logger.WarnException( string.Format( "Compile( '{0}' ) failed", code ), exc );
				CreateDomainAndCompiler( _namespaces, _references );

				foreach ( string prelude in _preludes )
				{
					_compiler.Compile( prelude, true );
				}

				return _compiler.Compile( code, isPrelude );
			}
		}

		public void SetLiveEventListener( ILiveEventListener listener )
		{
			var listenerSetter = new ListenerSetter( listener );
			_domain.DoCallBack( listenerSetter.SetListener );
		}
	}
}