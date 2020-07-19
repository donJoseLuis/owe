// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.Common.Extensions;
using Microsoft.TemplateEngine.OweLib.Common.Processes;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.Dotnet.Sdk
{
    internal abstract class Component<TModel>
    {
        internal Component()
        {
            ReloadItemsAsync().Wait();
            RootDirectory = GetRootDirectory();
        }

        internal Component(string state)
        {
            RootDirectory = GetRootDirectory(state);
            ReloadItemsAsync().Wait();
        }


        internal string RootDirectory { get; private set; }

        internal TModel[] InstalledItems { get; set; }


        internal string InstallAndReturnErrorIfAny(string name, string version, bool failIfDirectoryExists = true)
        {
            string directory = ComputeItemDirectory(name, version);
            if (!Directory.Exists(directory))
            {
                try
                {
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(directory);
                    return !directoryInfo.Exists ? $"Installation failed.  Reason: failed to create directory, {directory}" : string.Empty;
                }
                catch (UnauthorizedAccessException e)
                {
                    Trace.WriteLine(e.StackTrace);
                    return $"Installation failed.  Reason: {e.Message}.  Application must be run as Administrator";
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.StackTrace);
                    return $"Installation failed.  Reason: {e.Message}";
                }
            }
            return failIfDirectoryExists ? $"Installation failed.  Reason: directory already exists, {directory}" : string.Empty;
        }

        internal string UninstallAndReturnErrorIfAny(string name, string version)
        {
            string directory = ComputeItemDirectory(name, version);
            if (Directory.Exists(directory))
            {
                try
                {
                    Directory.Delete(directory, true);
                    return Directory.Exists(directory) ?
                        $"Uninstallation failed.  Reason: Failed to delete directory, {directory}." :
                        string.Empty;
                }
                catch (Exception e)
                {
                    return $"Uninstallation failed.  Reason: {e.Message}";
                }
            }
            return $"Uninstallation failed.  Reason: inexistent directory, {directory}.";
        }

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

        internal string ComputeItemDirectory(string version)
        {
            return ComputeItemDirectory(string.Empty, version);
        }

        internal string ComputeItemDirectory(string itemName, string version)
        {
            return string.IsNullOrWhiteSpace(itemName) ?
                Path.Combine(RootDirectory, version):
                Path.Combine(RootDirectory, itemName, version);
        }

        internal async Task ReloadItemsAsync(CancellationToken ct = default)
        {
            InstalledItems = await GetItemsAsync(ct).ConfigureAwait(false);
            Trace.WriteLine($"{InstalledItems?.Length ?? 0} {GetType().Name} item{((InstalledItems?.Length ?? 0) == 1 ? string.Empty : "s")} loaded.");
        }


        internal abstract TModel[] GetMatchingItems(string itemName);

        internal abstract string GetRootDirectory();

        internal abstract string GetRootDirectory(string state);

        internal abstract Task<TModel[]> GetItemsAsync(CancellationToken ct = default);

        internal abstract bool AnyEqualOrNewer(Version version);


        protected async Task<TModel[]> GetDotnetListOutputAsync(string argument, Func<string, TModel[]> modelParser, CancellationToken ct = default)
        {
            string outcome = await ProcessUtils.LaunchProcessAndGetOutputAsync("dotnet", $"--list-{argument}", ct).ConfigureAwait(false);
            return modelParser(outcome);
        }


        public override string ToString()
        {
            return $"{InstalledItems?.Length ?? 0} items";
        }
    }
}