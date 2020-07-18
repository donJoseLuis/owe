// Copyright (c) .NET Foundation and contributors. All rights reserved.

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.DotnetList.DotnetSdk
{
    internal sealed class DotnetApi
    {
        internal DotnetApi()
        {
            Workloads = new WorkloadDotnetComponent();
            Sdks = new SdkDotnetComponent();
        }


        internal WorkloadDotnetComponent Workloads { get; private set; }

        internal SdkDotnetComponent Sdks { get; private set; }
    }
}