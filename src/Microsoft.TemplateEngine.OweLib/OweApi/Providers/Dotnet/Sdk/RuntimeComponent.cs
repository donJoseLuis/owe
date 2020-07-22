// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.Models;
using Microsoft.TemplateEngine.OweLib.Models.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.Dotnet.Sdk
{
    internal sealed class RuntimeComponent : Component<RuntimeVersionInfo>
    {
        internal override string GetRootDirectory()
        {
            if (InstalledItems == null || InstalledItems.Length < 1)
            {
                return string.Empty;
            }
            RuntimeVersionInfo runtime = InstalledItems.First();
            return runtime.Location.Replace(runtime.Name, string.Empty);
        }

        internal override RuntimeVersionInfo[] GetMatchingItems(string runtimeName)
        {
            _ = string.IsNullOrWhiteSpace(runtimeName) ? throw new ArgumentNullException(paramName: nameof(runtimeName)) : runtimeName;
            return InstalledItems.Where(x => x.Name == runtimeName).ToArray();
        }

        internal override async Task<RuntimeVersionInfo[]> GetItemsAsync(CancellationToken ct = default)
        {
            return await GetDotnetListOutputAsync("runtimes", outcome => ParseDotnetListOutcome(outcome, x => x.ToWorkload()), ct).ConfigureAwait(false);
        }

        internal override bool AnyEqualOrNewer(Version version)
        {
            throw new NotImplementedException();
        }
    }
}