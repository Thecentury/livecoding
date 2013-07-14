using System;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;

namespace LiveCoding.Extension.VisualStudio
{
	internal static class ProjectHelper
	{
		public static Project GetContainingProject( string fileName )
		{
			if ( String.IsNullOrEmpty( fileName ) )
			{
				return null;
			}

			var dte2 = (DTE2)Package.GetGlobalService( typeof( SDTE ) );
			if ( dte2 == null )
			{
				return null;
			}

			var prjItem = dte2.Solution.FindProjectItem( fileName );
			if ( prjItem != null )
			{
				return prjItem.ContainingProject;
			}

			return null;
		}

		public static VSProject GetReferences( this Project project )
		{
			return (VSProject)project.Object;
		}
	}
}