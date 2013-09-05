using System;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using LiveCoding.Extension.ViewModels;
using NUnit.Framework;

namespace LiveCoding.Extension.Tests
{
	[TestFixture]
	public sealed class AppDomainTests
	{
		[Test]
		public void LoadAppDomainAndCreateAnObject()
		{
			AppDomainSetup appDomainSetup = new AppDomainSetup();
			appDomainSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

			AppDomain domain = AppDomain.CreateDomain( "LiveCodingCompilation_" + Guid.NewGuid().ToString( "N" ), null, appDomainSetup, new PermissionSet( PermissionState.Unrestricted ), new StrongName[0] );

			CodeCompiler codeCompiler =
				(CodeCompiler)domain.CreateInstanceAndUnwrap( typeof( CodeCompiler ).Assembly.FullName, typeof( CodeCompiler ).FullName );

			Assert.NotNull( codeCompiler );
		}
	}
}