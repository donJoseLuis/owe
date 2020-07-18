// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.Models;
using Microsoft.TemplateEngine.OweLib.Models.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.DotnetList.DotnetSdk.Tests
{
    [TestClass]
    public class DotnetApiTests
    {
        [TestMethod]
        public void TestNominativeOweVersionParsing()
        {
            string listSdksOutcome = "3.1.301 [C:\\Program Files\\dotnet\\sdk]\r\n3.0.101 [C:\\Program Files\\dotnet\\sdk]\r\n";
            DotnetApi api = new DotnetApi();
            VersionInfo[] sdkVersions = api.Sdks.ParseDotnetListOutcome(listSdksOutcome, x => x.ToOweVersion());
            Assert.IsNotNull(sdkVersions);
            Assert.IsTrue(sdkVersions.Length == 2);
            Assert.IsFalse(sdkVersions.Any(x => !x.IsValid));
        }

        [TestMethod]
        public void TestNominativeWorkloadVersionWrapperParsing()
        {
            string listRuntimesOutcome = "Microsoft.AspNetCore.All 2.1.19 [C:\\Program Files\\dotnet\\shared\\Microsoft.AspNetCore.All]\r\n" +
                "Microsoft.AspNetCore.App 2.1.19 [C:\\Program Files\\dotnet\\shared\\Microsoft.AspNetCore.App]\r\n";

            DotnetApi api = new DotnetApi();
            WorkloadVersionInfo[] runtimes = api.Workloads.ParseDotnetListOutcome(listRuntimesOutcome, x => x.ToWorkload());
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
        public void TestNominativeGetWorkloadVersionWrappers()
        {
            DotnetApi api = new DotnetApi();
            WorkloadVersionInfo workload = api.Workloads.InstalledItems.First();
            WorkloadVersionInfo[] items = api.Workloads.GetMatchingItems(workload.Name);
            Assert.IsNotNull(items);
            Assert.IsTrue(items.Length > 0);
        }

        [TestMethod]
        public void TestNegativeNullInput()
        {
            DotnetApi api = new DotnetApi();
            _ = Assert.ThrowsException<ArgumentNullException>(() => api.Workloads.GetMatchingItems(string.Empty));
            _ = Assert.ThrowsException<ArgumentNullException>(() => api.Workloads.GetMatchingItems(null));
            _ = Assert.ThrowsException<ArgumentNullException>(() => api.Sdks.AnyEqualOrNewer(null));
        }
    }
}