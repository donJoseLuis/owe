// Copyright (c) .NET Foundation and contributors. All rights reserved.

using System;
using System.Collections.Generic;

namespace Microsoft.TemplateEngine.OweLib.Common.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static IEnumerable<TTransformType> TransformEach<TModel, TTransformType>(this IEnumerable<TModel> models, Func<TModel, TTransformType> transform)
        {
            _ = transform ?? throw new ArgumentNullException(paramName: nameof(transform));
            List<TTransformType> list = new List<TTransformType>();
            foreach (TModel item in models)
            {
                list.Add(transform(item));
            }
            return list;
        }
    }
}