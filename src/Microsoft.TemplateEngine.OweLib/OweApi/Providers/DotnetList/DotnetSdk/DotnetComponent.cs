// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.Common.Extensions;
using Microsoft.TemplateEngine.OweLib.Common.Processes;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.DotnetList.DotnetSdk
{
    internal abstract class DotnetComponent<TModel>
    {
        internal DotnetComponent()
        {
            InstalledItems = ReloadItemsAsync().Result;
            InstallDirectory = GetItemsdRootDirectory();
        }


        internal string InstallDirectory { get; private set; }

        internal TModel[] InstalledItems { get; private set; }


        internal abstract TModel[] GetMatchingItems(string itemName);

        internal abstract string GetItemsdRootDirectory();

        internal abstract Task<TModel[]> GetInstalledItemsAsync(CancellationToken ct = default);

        internal abstract Task<TModel[]> ReloadItemsAsync(CancellationToken ct = default);

        internal abstract bool AnyEqualOrNewer(Version version);


        internal TModel[] ParseDotnetListOutcome(string outcome, Func<string, TModel> converter)
        {
            if (string.IsNullOrWhiteSpace(outcome))
            {
                return new TModel[] { };
            }

            return outcome
                .Split("\r\n")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                ?.TransformEach<string, TModel>(x => converter(x))
                ?.ToArray();
        }

        internal string ComputeItemDirectory(string itemName, string targetedVersion)
        {
            return Path.Combine(InstallDirectory, itemName, targetedVersion);
        }


        protected async Task<TModel[]> GetDotnetListOutputAsync(string argument, Func<string, TModel[]> modelParser, CancellationToken ct = default)
        {
            string outcome = await ProcessUtils.LaunchProcessAndGetOutputAsync("dotnet", $"--list-{argument}", ct).ConfigureAwait(false);
            return modelParser(outcome);
        }
    }
}