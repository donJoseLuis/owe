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
    internal sealed class SdkComponent : Component<VersionInfo>
    {
        internal override VersionInfo[] GetMatchingItems(string itemName)
        {
            throw new NotImplementedException();
        }

        internal override string GetRootDirectory()
        {
            return string.Empty;
        }

        internal override async Task<VersionInfo[]> GetItemsAsync(CancellationToken ct = default)
        {
            return await GetDotnetListOutputAsync("sdks", outcome => ParseDotnetListOutcome(outcome, x => x.ToOweVersion()), ct).ConfigureAwait(false);
        }

        internal override bool AnyEqualOrNewer(Version version)
        {
            _ = version ?? throw new ArgumentNullException(nameof(version));
            return InstalledItems.Any(x => x.VersionHandle >= version);
        }
    }
}