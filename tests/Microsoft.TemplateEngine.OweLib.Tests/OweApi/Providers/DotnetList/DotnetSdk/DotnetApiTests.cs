// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.Models;
using Microsoft.TemplateEngine.OweLib.Models.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.Dotnet.Sdk.Tests
{
    [TestClass]
    public class DotnetApiTests
    {
        [TestMethod]
        public void TestNominativeSdkParsing()
        {
            string listSdksOutcome = "3.1.301 [C:\\Program Files\\dotnet\\sdk]\r\n3.0.101 [C:\\Program Files\\dotnet\\sdk]\r\n";
            DotnetApi api = new DotnetApi();
            VersionInfo[] sdkVersions = api.Sdks.ParseDotnetListOutcome(listSdksOutcome, x => x.ToOweVersion());
            Assert.IsNotNull(sdkVersions);
            Assert.IsTrue(sdkVersions.Length == 2);
            Assert.IsFalse(sdkVersions.Any(x => !x.IsValid));
        }

        [TestMethod]
        public void TestNominativeRuntimeParsing()
        {
            string listRuntimesOutcome = "Microsoft.AspNetCore.All 2.1.19 [C:\\Program Files\\dotnet\\shared\\Microsoft.AspNetCore.All]\r\n" +
                "Microsoft.AspNetCore.App 2.1.19 [C:\\Program Files\\dotnet\\shared\\Microsoft.AspNetCore.App]\r\n";

            DotnetApi api = new DotnetApi();
            VersionInfo[] runtimes = api.Runtimes.ParseDotnetListOutcome(listRuntimesOutcome, x => x.ToWorkload());
            Assert.IsNotNull(runtimes);
            Assert.IsTrue(runtimes.Length == 2);
        }

        [TestMethod]
        [DataRow(100, false)]
        [DataRow(1, true)]
        public void TestNominativeEqualOrNewerFalse(int majorVersion, bool searchOutcome)
        {
            DotnetApi api = new DotnetApi();
            Assert.IsTrue(api.Sdks.AnyEqualOrNewer(new Version(majorVersion, 0, 0)) == searchOutcome);
        }

        [TestMethod]
        public void TestNominativeGetSdks()
        {
            DotnetApi api = new DotnetApi();
            VersionInfo sdk = api.Sdks.InstalledItems.First();
            Assert.IsNotNull(sdk);
            Assert.IsTrue(sdk.IsValid);
        }

        [TestMethod]
        public async Task TestNominativeGetTemplateFoldersAsync()
        {
            DotnetApi api = new DotnetApi();
            string path = Path.Combine(api.Templates.RootDirectory, "20.1.0");
            Directory.CreateDirectory(path);
            try
            {
                await api.Templates.ReloadItemsAsync().ConfigureAwait(false);
                VersionInfo templateFolders = api.Templates.InstalledItems.First();
                Assert.IsNotNull(templateFolders);
            }
            finally
            {
                Directory.Delete(path);
            }
        }

        [TestMethod]
        public void TestNegativeNullInputToOweApiMethods()
        {
            DotnetApi api = new DotnetApi();
            _ = Assert.ThrowsException<ArgumentNullException>(() => api.Runtimes.GetMatchingItems(string.Empty));
            _ = Assert.ThrowsException<ArgumentNullException>(() => api.Runtimes.GetMatchingItems(null));
            _ = Assert.ThrowsException<ArgumentNullException>(() => api.Sdks.AnyEqualOrNewer(null));
        }
    }
}