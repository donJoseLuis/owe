// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.Models;
using Microsoft.TemplateEngine.OweLib.Models.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.DotnetList.DotnetSdk
{
    internal sealed class WorkloadDotnetComponent : DotnetComponent<WorkloadVersionInfo>
    {
        internal override string GetItemsdRootDirectory()
        {
            if (InstalledItems == null || InstalledItems.Length < 1)
            {
                return string.Empty;
            }
            WorkloadVersionInfo runtime = InstalledItems.First();
            return runtime.Location.Replace(runtime.Name, string.Empty);
        }

        internal override WorkloadVersionInfo[] GetMatchingItems(string workLoadName)
        {
            _ = string.IsNullOrWhiteSpace(workLoadName) ? throw new ArgumentNullException(paramName: nameof(workLoadName)) : workLoadName;
            return InstalledItems.Where(x => x.Name == workLoadName).ToArray();
        }

        internal override async Task<WorkloadVersionInfo[]> GetInstalledItemsAsync(CancellationToken ct = default)
        {
            return await GetDotnetListOutputAsync("runtimes", outcome => ParseDotnetListOutcome(outcome, x => x.ToWorkload()), ct).ConfigureAwait(false);
        }

        internal override async Task<WorkloadVersionInfo[]> ReloadItemsAsync(CancellationToken ct = default)
        {
            WorkloadVersionInfo[] items = await GetInstalledItemsAsync(ct).ConfigureAwait(false);
            Trace.WriteLine($"Loaded {items?.Length ?? 0} runtimes (workloads)");
            return items;
        }

        internal override bool AnyEqualOrNewer(Version version)
        {
            throw new NotImplementedException();
        }
    }
}