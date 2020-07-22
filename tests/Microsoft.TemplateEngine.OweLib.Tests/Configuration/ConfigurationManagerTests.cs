// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Microsoft.TemplateEngine.OweLib.Configuration.Tests
{
    [TestClass]
    public class ConfigurationManagerTests
    {
        [TestMethod]
        public void TestNominativeSettingsReading()
        {
            foreach (string x in Enum.GetNames(typeof(AppKey)))
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[Enum.Parse<AppKey>(x)]));
            }
        }
    }
}