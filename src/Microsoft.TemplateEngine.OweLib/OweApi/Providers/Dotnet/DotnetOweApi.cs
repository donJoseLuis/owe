// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.OweApi.Providers.Dotnet.Sdk;
using Microsoft.TemplateEngine.OweLib.Models.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Microsoft.TemplateEngine.OweLib.Models;

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

        public bool TryUninstall(string version, out string error)
        {
            return TryRunOweCommand(version, MethodBase.GetCurrentMethod().Name, UninstallAndReturnErrorIfAny, out error);
        }

        public string[] GetOweTemplateLocations()
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            Trace.WriteLine($"{methodName}. Called.");
            List<string> list = new List<string>();
            foreach (VersionInfo version in _dotnetApi.Templates.InstalledItems)
            {
                string targetNuGetPath = Path.Combine(version.Location, OweTemplateNuGetFile);
                if (File.Exists(targetNuGetPath))
                {
                    list.Add(targetNuGetPath);
                }
                else
                {
                    Trace.TraceWarning($"{methodName}. Inexistent file: {targetNuGetPath}");
                }
            }
            Trace.TraceInformation($"{methodName}. Located {list.Count} templates.");
            return list.ToArray();
        }
        #endregion


        public override string ToString()
        {
            return $"OWE name: {OweName}";
        }


        #region unit testing hooks
        internal bool DisabledMinSdkVersionCheck { get; set; } = false;
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
            _dotnetApi.Templates.ReloadItemsAsync().Wait();
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