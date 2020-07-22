// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.Configuration;
using System.IO;
using System.Linq;

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.Dotnet.Sdk
{
    internal sealed class DotnetApi
    {
        internal DotnetApi()
        {
            static string GetValue(AppKey key, string defaultValue)
            {
                string value = ConfigurationManager.AppSettings[key];
                return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
            }

            Runtimes = new RuntimeComponent();
            Sdks = new SdkComponent();
            string firstSdkLocation = Sdks.InstalledItems?.First()?.Location;

            Templates = new TemplateComponent(GetValue(AppKey.TemplatesDirectory, Path.Combine(new DirectoryInfo(firstSdkLocation).Parent.FullName, "templates")));
            Workloads = new WorkloadComponent(GetValue(AppKey.WorkloadDirectory, firstSdkLocation));
        }


        internal WorkloadComponent Workloads { get; private set; }

        internal RuntimeComponent Runtimes { get; private set; }

        internal SdkComponent Sdks { get; private set; }

        internal TemplateComponent Templates { get; private set; }


        public override string ToString()
        {
            return $"Workloads: {Workloads}, SDKs: {Sdks}, Templates: {Templates}";
        }
    }
}