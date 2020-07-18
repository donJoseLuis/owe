// Copyright (c) .NET Foundation and contributors. All rights reserved.

namespace Microsoft.TemplateEngine.OweLib.OweApi.Providers.DotnetList
{
    internal sealed class DotnetListOweApiFactory : IOweApiFactory
    {
        private readonly static string ApiType = typeof(IOweApi).Name;

        public string Description => $"Builds {ApiType} instances based on the output of \"dotnet list --list\".";


        public IOweApi Create()
        {
            return new DotnetListOweApi();
        }
    }
}