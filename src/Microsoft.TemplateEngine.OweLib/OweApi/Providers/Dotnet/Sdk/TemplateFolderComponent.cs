// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.Common.Extensions;
using Microsoft.TemplateEngine.OweLib.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.Dotnet.Sdk
{
    internal sealed class TemplateFolderComponent : Component<VersionInfo>
    {
        internal TemplateFolderComponent(string sdkRootDirectory)
            : base(sdkRootDirectory)
        {
        }


        internal override VersionInfo[] GetMatchingItems(string itemName)
        {
            throw new NotImplementedException();
        }

        internal override string GetRootDirectory()
        {
            throw new NotImplementedException();
        }

        internal override string GetRootDirectory(string state)
        {
            _ = string.IsNullOrWhiteSpace(state) ? throw new ArgumentNullException(paramName: nameof(state)) : state;
            _ = Directory.Exists(state) ? state : throw new DirectoryNotFoundException();
            return Path.Combine(new DirectoryInfo(state).Parent.FullName, "templates");
        }

        internal override async Task<VersionInfo[]> GetItemsAsync(CancellationToken ct = default)
        {
            return await Task.Factory.StartNew<VersionInfo[]>(() =>
            {
                return new DirectoryInfo(RootDirectory)
                .GetDirectories()
                .TransformEach(x => new VersionInfo(x.Name, x.FullName))
                .ToArray();
            }).ConfigureAwait(false);
        }

        internal override bool AnyEqualOrNewer(Version version)
        {
            throw new NotImplementedException();
        }
    }
}