// Copyright (c) .NET Foundation and contributors. All rights reserved.

using System;

namespace Microsoft.TemplateEngine.OweLib.Models.Extensions
{
    internal static class StringExtensions
    {
        internal static VersionInfo ToOweVersion(this string item)
        {
            string[] tokens = item.Split(" [");
            return tokens.Length < 2 ?
                new VersionInfo(string.Empty, string.Empty) :
                new VersionInfo(tokens[0], tokens[1].Replace("[", string.Empty).Replace("]", string.Empty));
        }

        internal static RuntimeVersionInfo ToWorkload(this string item)
        {
            string[] tokens = item.Split(" [");
            string[] tokens2 = tokens[0].Split(" ");
            return new RuntimeVersionInfo(tokens2[0],
                tokens2[1],
                tokens[1].Replace("[", string.Empty).Replace("]", string.Empty));
        }

        internal static void ThrowIfInvalidateVersion(this string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(paramName: nameof(version));
            }
            else if (!Version.TryParse(version, out Version _))
            {
                throw new InvalidCastException($"Invalid version: {version}, expected format is MAJOR.MINOR.PATCH");
            }
        }
    }
}