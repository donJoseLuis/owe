// Copyright (c) .NET Foundation and contributors. All rights reserved.

using System.Diagnostics;
using System.Linq;

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.Dotnet.Sdk
{
    internal sealed class DotnetApi
    {
        internal DotnetApi()
        {
            Workloads = new WorkloadComponent();
            Sdks = new SdkComponent();
            Templates = new TemplateFolderComponent(Sdks.InstalledItems?.First()?.Location);
        }


        internal WorkloadComponent Workloads { get; private set; }

        internal SdkComponent Sdks { get; private set; }

        internal TemplateFolderComponent Templates { get; private set; }


        public override string ToString()
        {
            return $"Workloads: {Workloads}, SDKs: {Sdks}, Templates: {Templates}";
        }
    }
}