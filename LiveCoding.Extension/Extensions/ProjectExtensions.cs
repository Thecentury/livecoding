using System.IO;
using EnvDTE;

namespace LiveCoding.Extension.Extensions
{
	public static class ProjectExtensions
	{
		public static string GetOutputFullPath( this Project project )
		{
			Configuration activeConfiguration = project.ConfigurationManager.ActiveConfiguration;
			string outputName = project.Properties.Item( "OutputFileName" ).Value.ToString();
			string outputPath = activeConfiguration.Properties.Item( "OutputPath" ).Value.ToString();
			string projectPath = project.Properties.Item( "FullPath" ).Value.ToString();
			string projectOutputFullPath = Path.Combine( projectPath, outputPath, outputName );

			return projectOutputFullPath;
		}
	}
}