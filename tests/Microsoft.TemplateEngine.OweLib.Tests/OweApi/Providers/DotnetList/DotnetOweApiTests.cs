// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.Dotnet.Tests
{
    [TestClass]
    public class DotnetOweApiTests
    {
        [TestMethod]
        public void TetsNominativeTryInstallAndUninstall()
        {
            string version = "9.1.0";
            DotnetOweApi api = new DotnetOweApi
            {
                DisabledMinSdkVersionCheck = true
            };

            try
            {
                Assert.IsTrue(api.TryInstall(version, out string error), error);
                Assert.IsTrue(string.IsNullOrWhiteSpace(error), error);
                Assert.IsTrue(api.GetOweTemplateLocations().Length > 0);
            }
            finally
            {
                Assert.IsTrue(api.TryUninstall(version, out string error), error);
                Assert.IsTrue(string.IsNullOrWhiteSpace(error), error);
                Assert.IsTrue(api.GetOweTemplateLocations().Length == 0);
            }
        }
    }
}