// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Microsoft.TemplateEngine.OweLib.Common.Processes.Tests
{
    [TestClass]
    public class ProcessUtilsTests
    {
        [TestMethod]
        public async Task TestNominativeProcessLaunchAsync()
        {
            string outcome = await ProcessUtils.LaunchProcessAndGetOutputAsync("dotnet", "--list-sdks").ConfigureAwait(false);
            Assert.IsFalse(string.IsNullOrWhiteSpace(outcome));
        }
    }
}