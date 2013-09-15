using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using JetBrains.Annotations;
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

		public void Dispose()
		{
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

			_domain = AppDomain.CreateDomain( "LiveCodingCompilation_" + Guid.NewGuid().ToString( "N" ),
				AppDomain.CurrentDomain.Evidence, appDomainSetup, new PermissionSet( PermissionState.Unrestricted ), new StrongName[0] );

			_compiler = _domain.CreateInstanceAndUnwrap<CodeCompiler>();
			_compiler.SetupScriptEngine( namespaces, references );
		}

		public void Compile( string code )
		{
			if ( code == null )
			{
				throw new ArgumentNullException( "code" );
			}

			try
			{
				_compiler.Compile( code );
			}
			catch ( RemotingException exc )
			{
				var logger = LogManager.GetCurrentClassLogger();
				logger.WarnException( string.Format( "Compile( '{0}' ) failed", code ), exc );
				CreateDomainAndCompiler( _namespaces, _references );

				_compiler.Compile( code );
			}
		}

		public void SetLiveEventListener( ILiveEventListener listener )
		{
			var listenerSetter = new ListenerSetter( listener );
			_domain.DoCallBack( listenerSetter.SetListener );
		}
	}
}