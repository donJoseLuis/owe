// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.Dotnet.Sdk
{
    internal sealed class WorkloadComponent : Component<VersionInfo>
    {
        internal WorkloadComponent(string workloadDirectory)
            : base(workloadDirectory)
        {
        }


        internal override string GetRootDirectory()
        {
            throw new NotImplementedException();
        }

        internal override VersionInfo[] GetMatchingItems(string workLoadName)
        {
            throw new NotImplementedException();
        }

        internal override async Task<VersionInfo[]> GetItemsAsync(CancellationToken ct = default)
        {
            return await Task.Factory.StartNew<VersionInfo[]>(() =>
            {
                List<VersionInfo> list = new List<VersionInfo>();
                foreach (DirectoryInfo dir in new DirectoryInfo(RootDirectory).GetDirectories())
                {
                    VersionInfo versionInfo = new VersionInfo(dir.Name, dir.FullName);
                    if (versionInfo.IsValid)
                    {
                        list.Add(versionInfo);
                    }
                }
                return list.ToArray();
            }).ConfigureAwait(false);
        }

        internal override bool AnyEqualOrNewer(Version version)
        {
            throw new NotImplementedException();
        }
    }
}