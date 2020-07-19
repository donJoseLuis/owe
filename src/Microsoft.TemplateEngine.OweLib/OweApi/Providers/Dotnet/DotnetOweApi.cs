// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.OweApi.Providers.Dotnet.Sdk;
using Microsoft.TemplateEngine.OweLib.Models.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Microsoft.TemplateEngine.OweLib.Tests")]
namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.Dotnet
{
    internal sealed class DotnetOweApi : IOweApi
    {
        private const string OweTemplateNuGetFile = "OWE.Templates.1.0.0.nupkg";

        private static readonly Version MinSdkVersion = new Version(5, 0, 0);

        private readonly DotnetApi _dotnetApi = new DotnetApi();

        private readonly string _oweTemplateNuGetFullPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", OweTemplateNuGetFile);


        internal DotnetOweApi()
        {
            Trace.WriteLine($"{typeof(IOweApi).Name} instantiated.");
        }


        #region IOweApi
        public string OweName => "Microsoft.OptionalWorkloadEmulator";


        public bool TryInstall(string version, out string error)
        {
            return TryRunOweCommand(version, MethodBase.GetCurrentMethod().Name, InstallAndReturnErrorIfAny, out error);
        }

        public bool TryUpdate(string version, out string error)
        {
            return TryRunOweCommand(version, MethodBase.GetCurrentMethod().Name, UpdateAndReturnErrorIfAny, out error);
        }

        public bool TryUninstall(string version, out string error)
        {
            return TryRunOweCommand(version, MethodBase.GetCurrentMethod().Name, UninstallAndReturnErrorIfAny, out error);
        }

        public bool TryUninstallOwe(out string error)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            Trace.WriteLine($"{methodName}. Invoked.");

            try
            {
                error = UninstallAndReturnErrorIfAny(Path.Combine(_dotnetApi.Workloads.RootDirectory, OweName));
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                Trace.WriteLine($"{methodName}. {error}");
                return false;
            }
            _dotnetApi.Workloads.ReloadItemsAsync().Wait();
            Trace.WriteLine($"{methodName}. Completed execution.");
            return true;
        }

        public string[] GetOweVersions()
        {
            string[] versions = null;
            try
            {
                versions = _dotnetApi.Workloads.GetMatchingItems(OweName)
                ?.Where(x => x.IsValid)
                ?.Select(x => x.VersionHandle.ToString())
                ?.ToArray() ?? new string[] { };
            }
            catch (Exception e)
            {
                Trace.TraceError($"Failed to find OWE versions.  Reason: {e.Message}");
            }

            if (versions == null || versions.Length < 1)
            {
                Trace.WriteLine($"Failed to find OWE versions.  Reason: No installed versions of Workload {OweName}.");
            }
            else
            {
                Trace.WriteLine($"Installed {OweName} versions: {string.Join(",", versions)}");
            }
            return versions;
        }

        public string GetOweLocation(string version)
        {
            _ = string.IsNullOrWhiteSpace(version) ? throw new ArgumentNullException(paramName: nameof(version)) : version;

            string methodName = MethodBase.GetCurrentMethod().Name;
            Trace.WriteLine($"{methodName}. Called with {nameof(version)} {version ?? "no-version"}.");

            if (_dotnetApi.Workloads.GetMatchingItems(OweName).Any(x => x.VersionHandle.ToString() == version))
            {
                string directory = _dotnetApi.Workloads.ComputeItemDirectory(OweName, version);
                if (Directory.Exists(directory))
                {
                    Trace.WriteLine($"{methodName}. Location of workload {OweName} & version: {version} is {directory}.");
                    return directory;
                }
            }
            Trace.TraceError($"{methodName}. Workload {OweName} & version: {version} not found.");
            return string.Empty;
        }
        #endregion


        public override string ToString()
        {
            return $"OWE name: {OweName}";
        }


        #region unit testing hooks
        internal bool DisabledMinSdkVersionCheck { get; set; } = true;
        #endregion


        private bool TryRunOweCommand(string version, string methodName, Func<string, string> command, out string error)
        {
            Trace.WriteLine($"{methodName}. Invoked with parameter: \"{version ?? "no param"}\".");

            error = RunOweCommandAndReturnErrorIfAny(version, command);
            if (!string.IsNullOrWhiteSpace(error))
            {
                Trace.TraceError($"{methodName}. {error}.");
                return false;
            }
            _dotnetApi.Workloads.ReloadItemsAsync().Wait();
            Trace.WriteLine($"{methodName}. Completed execution for version {version}.");
            return true;
        }

        private string RunOweCommandAndReturnErrorIfAny(string version, Func<string, string> command)
        {
            try
            {
                version.ThrowIfInvalidateVersion();
                if (!DisabledMinSdkVersionCheck && !_dotnetApi.Sdks.AnyEqualOrNewer(MinSdkVersion))
                {
                    return "Installed SDK versions are older than 5.0";
                }
                return command(version);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private string InstallAndReturnErrorIfAny(string version)
        {
            string outcome = _dotnetApi.Workloads.InstallAndReturnErrorIfAny(OweName, version);
            if (!string.IsNullOrWhiteSpace(outcome))
            {
                return outcome;
            }

            outcome = _dotnetApi.Templates.InstallAndReturnErrorIfAny(string.Empty, version, false);
            if (!string.IsNullOrWhiteSpace(outcome))
            {
                return outcome;
            }
            string targetNuGetPath = Path.Combine(_dotnetApi.Templates.ComputeItemDirectory(version), OweTemplateNuGetFile);
            File.Copy(_oweTemplateNuGetFullPath, targetNuGetPath);

            return string.Empty;
        }

        private string UpdateAndReturnErrorIfAny(string version)
        {
            string versionedOweDirectory = _dotnetApi.Workloads.ComputeItemDirectory(OweName, version);
            if (!Directory.Exists(versionedOweDirectory))
            {
                return $"Inexistent workLoad directory, {versionedOweDirectory}.";
            }

            // TODO: what update behavior do we want during updates?
            return string.Empty;
        }

        // TODO: what should be the dotnet new uninstall behavior?
        private string UninstallAndReturnErrorIfAny(string version)
        {
            string outcome = _dotnetApi.Workloads.UninstallAndReturnErrorIfAny(OweName, version);
            if (!string.IsNullOrWhiteSpace(outcome))
            {
                return outcome;
            }

            string nuGetPath = Path.Combine(_dotnetApi.Templates.ComputeItemDirectory(version), OweTemplateNuGetFile);
            if (File.Exists(nuGetPath))
            {
                try
                {
                    File.Delete(nuGetPath);
                    return File.Exists(nuGetPath) ? $"Uninstallation failed.  Reason: failed to remove template NuGet {nuGetPath}" : string.Empty;
                }
                catch (Exception e)
                {
                    return $"Uninstallation failed.  Reason: {e.Message}";
                }
            }

            return string.Empty;
        }
    }
}