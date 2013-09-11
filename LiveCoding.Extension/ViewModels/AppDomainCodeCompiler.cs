using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using LiveCoding.Core;

namespace LiveCoding.Extension.ViewModels
{
	internal sealed class AppDomainCodeCompiler : ICodeCompiler
	{
		private AppDomain _domain;
		private CodeCompiler _compiler;

		public void Dispose()
		{
			_compiler = null;
			AppDomain.Unload( _domain );
		}

		public void SetupScriptEngine( List<string> namespaces, List<string> references )
		{
			AppDomainSetup appDomainSetup = new AppDomainSetup
			{
				ApplicationBase = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ),
				ShadowCopyFiles = "true",
				LoaderOptimization = LoaderOptimization.MultiDomain
			};

			_domain = AppDomain.CreateDomain( "LiveCodingCompilation_" + Guid.NewGuid().ToString( "N" ),
				AppDomain.CurrentDomain.Evidence, appDomainSetup, new PermissionSet( PermissionState.Unrestricted ), new StrongName[ 0 ] );

			_compiler = _domain.CreateInstanceAndUnwrap<CodeCompiler>();
			_compiler.SetupScriptEngine( namespaces, references );
		}

		public void Compile( string code )
		{
			_compiler.Compile( code );
		}

		public void SetLiveEventListener( ILiveEventListener listener )
		{
			var listenerSetter = new ListenerSetter( listener );
			_domain.DoCallBack( listenerSetter.SetListener );
		}
	}
}