// Copyright (c) .NET Foundation and contributors. All rights reserved.

namespace Microsoft.TemplateEngine.OweLib
{
    // TODO: test template installation
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
        /// Try-pattern for updating an existing specified version of the emulated optional workload.
        /// </summary>
        /// <param name="version"><see cref="string"/> specified version of the emulated optional workload.</param>
        /// <param name="error"><see cref="string"/> describes an ocurred error, if any.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        bool TryUpdate(string version, out string error);

        /// <summary>
        /// Try-pattern for uninstalling a specified version of the emulated optional workload.
        /// </summary>
        /// <param name="version"><see cref="string"/> specified version of the emulated optional workload.</param>
        /// <param name="error"><see cref="string"/> describes an ocurred error, if any.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        bool TryUninstall(string version, out string error);

        /// <summary>
        /// Try-pattern for uninstalling the <see cref="OweName"/> workload.
        /// </summary>
        /// <param name="error"><see cref="string"/> describes an ocurred error, if any.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        bool TryUninstallOwe(out string error);

        /// <summary>
        /// Exposes <see cref="string"/> array of installed versions of the emulated optional workload.
        /// </summary>
        /// <returns><see cref="string"/> array of installed OWE versions.</returns>
        string[] GetOweVersions();

        /// <summary>
        /// Exposes <see cref="string"/> location of the specified version of the emulated optional workload.
        /// </summary>
        /// <param name="version"><see cref="string"/> specified version of the emulated optional workload.</param>
        /// <returns><see cref="string"/> location of the specified OWE version.</returns>
        string GetOweLocation(string version);
    }
}