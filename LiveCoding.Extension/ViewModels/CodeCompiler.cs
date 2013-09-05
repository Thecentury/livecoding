using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace LiveCoding.Extension.ViewModels
{
	internal sealed class CodeCompiler : MarshalByRefObject
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

		public void Compile( string code )
		{
			_session.Execute( code );
		}
	}
}