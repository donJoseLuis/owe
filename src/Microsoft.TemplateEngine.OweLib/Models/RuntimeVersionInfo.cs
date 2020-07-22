// Copyright (c) .NET Foundation and contributors. All rights reserved.

using System;

namespace Microsoft.TemplateEngine.OweLib.Models
{
    /// <summary>
    /// Abstraction of a work load.
    /// </summary>
    public sealed class RuntimeVersionInfo : VersionInfo
    {
        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="workloadName"><see cref="string"/> workload name.</param>
        /// <param name="version"><see cref="string"/> string representation of a <see cref="Version"/> instance.</param>
        /// <param name="location"><see cref="string"/> path on disk where the workload is installed.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the .ctor paramters is null.</exception>
        public RuntimeVersionInfo(string workloadName, string version, string location)
            :base(version, location)
        {
            Name = workloadName ?? throw new ArgumentNullException(nameof(workloadName));
        }


        /// <summary>
        /// Workload name.
        /// </summary>
        public string Name { get; }


        public override string ToString()
        {
            return $"Workload: \"{Name ?? "no name"}\", {base.ToString()}";
        }
    }
}