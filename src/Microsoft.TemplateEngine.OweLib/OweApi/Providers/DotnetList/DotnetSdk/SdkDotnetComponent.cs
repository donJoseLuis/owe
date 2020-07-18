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
    internal sealed class SdkDotnetComponent : DotnetComponent<VersionInfo>
    {
        internal override VersionInfo[] GetMatchingItems(string workLoadName)
        {
            throw new NotImplementedException();
        }

        internal override string GetItemsdRootDirectory()
        {
            return string.Empty;
        }

        internal override async Task<VersionInfo[]> GetInstalledItemsAsync(CancellationToken ct = default)
        {
            return await GetDotnetListOutputAsync("sdks", outcome => ParseDotnetListOutcome(outcome, x => x.ToOweVersion()), ct).ConfigureAwait(false);
        }

        internal override async Task<VersionInfo[]> ReloadItemsAsync(CancellationToken ct = default)
        {
            VersionInfo[] items = await GetInstalledItemsAsync(ct).ConfigureAwait(false);
            Trace.WriteLine($"Loaded {items?.Length ?? 0} SDKs");
            return items;
        }

        internal override bool AnyEqualOrNewer(Version version)
        {
            _ = version ?? throw new ArgumentNullException(nameof(version));
            return InstalledItems.Any(x => x.VersionHandle >= version);
        }
    }
}
