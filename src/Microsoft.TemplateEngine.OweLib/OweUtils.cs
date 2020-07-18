// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.OweApi.Providers.DotnetList;

namespace Microsoft.TemplateEngine.OweLib
{
    /// <summary>
    /// Singleton access to a set of OWE library features.
    /// </summary>
    public static class OweUtils
    {
        /// <summary>
        /// .cctor
        /// </summary>
        static OweUtils()
        {
            DefaultOweFactory = new DotnetListOweApiFactory();
        }


        /// <summary>
        /// A default <see cref="IOweApiFactory"/> instance.
        /// </summary>
        public static IOweApiFactory DefaultOweFactory { get; }
    }
}