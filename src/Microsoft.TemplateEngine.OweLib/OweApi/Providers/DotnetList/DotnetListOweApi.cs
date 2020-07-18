// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.OweApi.Providers.DotnetList.DotnetSdk;
using Microsoft.TemplateEngine.OweLib.Models.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Microsoft.TemplateEngine.OweLib.Tests")]
namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.DotnetList
{
    internal sealed class DotnetListOweApi : IOweApi
    {
        private static readonly Version MinSdkVersion = new Version(5, 0, 0);

        private readonly DotnetApi _dotnetApi = new DotnetApi();


        #region IOweApi
        public string OweName => "Microsoft.OptionalWorkloadEmulator";


        public bool TryInstall(string targetedVersion, out string error)
        {
            return TryRunOweCommand(targetedVersion, MethodBase.GetCurrentMethod().Name, InstallAndReturnErrorIfAny, out error);
        }

        public bool TryUpdate(string targetedVersion, out string error)
        {
            return TryRunOweCommand(targetedVersion, MethodBase.GetCurrentMethod().Name, UpdateAndReturnErrorIfAny, out error);
        }

        public bool TryUninstall(string targetedVersion, out string error)
        {
            return TryRunOweCommand(targetedVersion, MethodBase.GetCurrentMethod().Name, UninstallAndReturnErrorIfAny, out error);
        }

        public bool TryUninstallOwe(out string error)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            Trace.WriteLine($"{methodName}. Invoked.");

            try
            {
                string fullPathDirectory = Path.Combine(_dotnetApi.Workloads.InstallDirectory, OweName);
                error = UninstallAndReturnErrorIfAny(Directory.Exists(fullPathDirectory), fullPathDirectory);
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
            return _dotnetApi.Workloads.GetMatchingItems(OweName)
                ?.Where(x => x.IsValid)
                ?.Select(x => x.VersionHandle.ToString())
                ?.ToArray();
        }

        public string GetOweLocation(string targetedVersion)
        {
            _ = string.IsNullOrWhiteSpace(targetedVersion) ? throw new ArgumentNullException(paramName: nameof(targetedVersion)) : targetedVersion;

            string methodName = MethodBase.GetCurrentMethod().Name;
            Trace.WriteLine($"{methodName}. Called with {nameof(targetedVersion)} {targetedVersion ?? "no-version"}.");

            if (_dotnetApi.Workloads.GetMatchingItems(OweName).Any(x => x.VersionHandle.ToString() == targetedVersion))
            {
                string directory = _dotnetApi.Workloads.ComputeItemDirectory(OweName, targetedVersion);
                if (Directory.Exists(directory))
                {
                    Trace.WriteLine($"{methodName}. Found directory {directory} for workload: {OweName} & version: {targetedVersion}.");
                    return directory;
                }
            }
            Trace.WriteLine($"{methodName}. No directory found for workload: {OweName} & version: {targetedVersion}.");
            return string.Empty;
        }
        #endregion

        public override string ToString()
        {
            return $"OWE name: {OweName}";
        }

        #region unit testing hooks
        internal bool DisableMinimumSdkVerification { get; set; }
        #endregion


        private bool TryRunOweCommand(string targetedVersion, string methodName, Func<bool, string, string> command, out string error)
        {
            Trace.WriteLine($"{methodName}. Invoked with parameter: \"{targetedVersion ?? "no param"}\".");

            error = RunOweCommandAndReturnErrorIfAny(targetedVersion, command);
            if (!string.IsNullOrWhiteSpace(error))
            {
                Trace.WriteLine($"{methodName}. {error}");
                return false;
            }
            _dotnetApi.Workloads.ReloadItemsAsync().Wait();
            Trace.WriteLine($"{methodName}. Completed execution for version {targetedVersion}.");
            return true;
        }

        private string InstallAndReturnErrorIfAny(bool directoryExists, string fullPathDirectory)
        {
            if (!directoryExists)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(fullPathDirectory);
                directoryInfo.Create();
                return directoryInfo.Exists ? string.Empty : $"Installation failed.  Reason: Failed create directory {directoryInfo.FullName}.";
            }
            return $"WorkLoad directory already exists, {fullPathDirectory}.";
        }

        private string UpdateAndReturnErrorIfAny(bool directoryExists, string fullPathDirectory)
        {
            if (!directoryExists)
            {
                return $"Inexistent workLoad directory, {fullPathDirectory}.";
            }

            // TODO: what update behavior do we want during updates?
            return string.Empty;
        }

        private string UninstallAndReturnErrorIfAny(bool directoryExists, string fullPathDirectory)
        {
            if (directoryExists)
            {
                try
                {
                    Directory.Delete(fullPathDirectory, true);
                    return string.Empty;
                }
                catch (Exception e)
                {
                    return $"Failed to remove directory {fullPathDirectory}.  Reason: {e.Message}";
                }
            }
            return $"Inexistent workLoad directory, {fullPathDirectory}.";
        }

        private string RunOweCommandAndReturnErrorIfAny(string targetedVersion, Func<bool, string, string> command)
        {
            string error = targetedVersion.ValidateVersionAndReturnErrorIfAny();
            if (!string.IsNullOrWhiteSpace(error))
            {
                return error;
            }
            else if (!DisableMinimumSdkVerification && !_dotnetApi.Sdks.AnyEqualOrNewer(MinSdkVersion))
            {
                return "Installed SDK versions are older than 5.0";
            }

            try
            {
                string fullPathDirectory = _dotnetApi.Workloads.ComputeItemDirectory(OweName, targetedVersion);
                return command(Directory.Exists(fullPathDirectory), fullPathDirectory);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}