// Copyright (c) .NET Foundation and contributors. All rights reserved.

namespace Microsoft.TemplateEngine.OweLib
{
    /// <summary>
    /// Factory of <see cref="IOweApi"/> instances.
    /// </summary>
    public interface IOweApiFactory
    {
        /// <summary>
        /// Describes the type of <see cref="IOweApi"/> instances produced by the factory.
        /// </summary>
        string Description { get; }


        /// <summary>
        /// Creates <see cref="IOweApi"/> instances.
        /// </summary>
        /// <returns></returns>
        IOweApi Create();
    }
}