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

        internal static WorkloadVersionInfo ToWorkload(this string item)
        {
            string[] tokens = item.Split(" [");
            string[] tokens2 = tokens[0].Split(" ");
            return new WorkloadVersionInfo(tokens2[0],
                tokens2[1],
                tokens[1].Replace("[", string.Empty).Replace("]", string.Empty));
        }

        internal static string ValidateVersionAndReturnErrorIfAny(this string targetedVersion)
        {
            if (string.IsNullOrWhiteSpace(targetedVersion))
            {
                return "Null targeted version";
            }
            else if (!Version.TryParse(targetedVersion, out Version _))
            {
                return $"Invalid version: {targetedVersion}, expected format is MAJOR.MINOR.PATCH";
            }

            return string.Empty;
        }
    }
}