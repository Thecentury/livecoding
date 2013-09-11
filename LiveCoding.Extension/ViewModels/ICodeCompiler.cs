using System;
using System.Collections.Generic;
using LiveCoding.Core;

namespace LiveCoding.Extension.ViewModels
{
	internal interface ICodeCompiler : IDisposable
	{
		void SetupScriptEngine( List<string> namespaces, List<string> references );

		void Compile( string code );

		void SetLiveEventListener( ILiveEventListener listener );
	}
}