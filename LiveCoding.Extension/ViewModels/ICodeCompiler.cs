using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LiveCoding.Core;

namespace LiveCoding.Extension.ViewModels
{
	internal interface ICodeCompiler : IDisposable
	{
		void SetupScriptEngine( [NotNull] List<string> namespaces, [NotNull] List<string> references );

		object Compile( [NotNull] string code, bool isPrelude );

		void SetLiveEventListener( ILiveEventListener listener );
	}
}