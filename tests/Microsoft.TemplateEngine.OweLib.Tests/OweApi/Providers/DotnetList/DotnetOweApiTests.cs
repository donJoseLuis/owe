// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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

            Assert.IsTrue(api.TryInstall(version, out string error), error);
            Assert.IsTrue(string.IsNullOrWhiteSpace(error), error);
            Assert.IsTrue(api.GetOweVersions().Any(x => x == version), $"workload {api.OweName} version {version} does not exist.");

            Assert.IsTrue(api.TryUninstall(version, out error), error);
            Assert.IsTrue(string.IsNullOrWhiteSpace(error), error);
            Assert.IsFalse(api.GetOweVersions().Any(x => x == version));
            Assert.IsTrue(string.IsNullOrWhiteSpace(api.GetOweLocation(version)));
        }
    }
}