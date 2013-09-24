using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LiveCoding.Core;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace LiveCoding.Extension.ViewModels
{
	internal sealed class CodeCompiler : MarshalByRefObject, ICodeCompiler
	{
		private readonly ScriptEngine _scriptEngine = new ScriptEngine();
		private Session _session;

		public void SetupScriptEngine( List<string> namespaces, List<string> references )
		{
			foreach ( string ns in namespaces )
			{
				_scriptEngine.ImportNamespace( ns );
			}

			foreach ( string reference in references )
			{
				if ( File.Exists( reference ) )
				{
					_scriptEngine.AddReference( Assembly.LoadFrom( reference ) );
				}
			}

			_session = _scriptEngine.CreateSession();
		}

		public object Compile( string code )
		{
			return _session.Execute( code );
		}

		public void SetLiveEventListener( ILiveEventListener listener )
		{
			VariablesTrackerFacade.SetListener( listener );
		}

		public void Dispose()
		{
			// do nothing
		}
	}
}