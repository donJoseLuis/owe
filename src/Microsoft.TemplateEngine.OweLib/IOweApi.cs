// Copyright (c) .NET Foundation and contributors. All rights reserved.

namespace Microsoft.TemplateEngine.OweLib
{
    /// <summary>
    /// Optional workload emulator features.
    /// </summary>
    public interface IOweApi
    {
        /// <summary>
        /// Name of emulated optional workload.
        /// </summary>
        string OweName { get; }


        /// <summary>
        /// Try-pattern for installing a specified version of the emulated optional workload.
        /// </summary>
        /// <param name="version"><see cref="string"/> specified version of the emulated optional workload.</param>
        /// <param name="error"><see cref="string"/> describes an ocurred error, if any.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        bool TryInstall(string version, out string error);

        /// <summary>
        /// Try-pattern for uninstalling a specified version of the emulated optional workload.
        /// </summary>
        /// <param name="version"><see cref="string"/> specified version of the emulated optional workload.</param>
        /// <param name="error"><see cref="string"/> describes an ocurred error, if any.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        bool TryUninstall(string version, out string error);

        /// <summary>
        /// Exposes the full path <see cref="string[]"/> to all OWE templates.
        /// </summary>
        /// <returns>full path to all OWE templates.</returns>
        string[] GetOweTemplateLocations();
    }
}