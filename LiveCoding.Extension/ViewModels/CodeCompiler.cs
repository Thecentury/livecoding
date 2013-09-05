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
		private string _copyDirectory;

		public void SetupScriptEngine( List<string> namespaces, List<string> references )
		{
			Guid sessionId = Guid.NewGuid();

			foreach ( string ns in namespaces )
			{
				_scriptEngine.ImportNamespace( ns );
			}

			_copyDirectory = Path.Combine( Path.GetTempPath(), sessionId.ToString() );
			Directory.CreateDirectory( _copyDirectory );

			foreach ( string reference in references )
			{
				if ( File.Exists( reference ) )
				{
					string originalFileName = Path.GetFileName( reference );

					string fullCopyPath = Path.Combine( _copyDirectory, originalFileName );

					File.Copy( reference, fullCopyPath );

					_scriptEngine.AddReference( Assembly.LoadFrom( fullCopyPath ) );
				}
			}

			_session = _scriptEngine.CreateSession();
		}

		public void Compile( string code )
		{
			_session.Execute( code );
		}

		public string TempDirectory
		{
			get { return _copyDirectory; }
		}
	}
}