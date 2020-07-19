// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.Models;
using Microsoft.TemplateEngine.OweLib.Models.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.Dotnet.Sdk
{
    internal sealed class WorkloadComponent : Component<WorkloadVersionInfo>
    {
        internal override string GetRootDirectory()
        {
            if (InstalledItems == null || InstalledItems.Length < 1)
            {
                return string.Empty;
            }
            WorkloadVersionInfo runtime = InstalledItems.First();
            return runtime.Location.Replace(runtime.Name, string.Empty);
        }

        internal override string GetRootDirectory(string state)
        {
            throw new NotImplementedException();
        }

        internal override WorkloadVersionInfo[] GetMatchingItems(string workLoadName)
        {
            _ = string.IsNullOrWhiteSpace(workLoadName) ? throw new ArgumentNullException(paramName: nameof(workLoadName)) : workLoadName;
            return InstalledItems.Where(x => x.Name == workLoadName).ToArray();
        }

        internal override async Task<WorkloadVersionInfo[]> GetItemsAsync(CancellationToken ct = default)
        {
            return await GetDotnetListOutputAsync("runtimes", outcome => ParseDotnetListOutcome(outcome, x => x.ToWorkload()), ct).ConfigureAwait(false);
        }

        internal override bool AnyEqualOrNewer(Version version)
        {
            throw new NotImplementedException();
        }
    }
}