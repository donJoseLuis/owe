// Copyright (c) .NET Foundation and contributors. All rights reserved.

using System;
using System.IO;

namespace Microsoft.TemplateEngine.OweLib.Models
{
    /// <summary>
    /// Wraps <see cref="Version"/> with additional location information.
    /// </summary>
    public class VersionInfo
    {
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="version"><see cref="string"/> representation of the version.</param>
        /// <param name="location"><see cref="string"/> full path fo the versioned item's location.</param>
        public VersionInfo(string version, string location)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
            IsValid = Version.TryParse(version, out Version versionHandle) && Directory.Exists(location);
            VersionHandle = versionHandle;
        }


        /// <summary>
        /// True if the version is valid and the location exists.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// <see cref="Version"/> handle for the component.
        /// </summary>
        public Version VersionHandle { get; }

        /// <summary>
        /// Versioned item's location on disk.
        /// </summary>
        public string Location { get; }


        public override string ToString()
        {
            return $"Version: \"{VersionHandle?.ToString() ?? "no version"}\", ({Location ?? "no location"})";
        }
    }
}