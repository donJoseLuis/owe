// Copyright (c) .NET Foundation and contributors. All rights reserved.

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.TemplateEngine.OweLib.Common.Serialization
{
    /// <summary>
    /// Contains members that serialize and deserialize json.
    /// </summary>
    public static class JsonSerializer
    {
        /// <summary>
        /// Creates an <typeparamref name="TModel"/> instance from the specified json <see cref="string"/>.
        /// </summary>
        /// <typeparam name="TModel">Type of instance to deserialize into.</typeparam>
        /// <param name="json">Specified json <see cref="string"/>.</param>
        /// <returns><typeparamref name="TModel"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        public static TModel Deserialize<TModel>(string json)
        {
            _ = string.IsNullOrWhiteSpace(json) ? throw new ArgumentNullException(paramName: nameof(json)) : json;
            using MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(json));
            return (TModel)new DataContractJsonSerializer(typeof(TModel)).ReadObject(stream);
        }

        /// <summary>
        /// Serializes the specified <typeparamref name="TModel"/> instance to a json <see cref="string"/>.
        /// </summary>
        /// <typeparam name="TModel">Type of instance to serialize.</typeparam>
        /// <param name="instance">Specified <typeparamref name="TModel"/> instance.</param>
        /// <returns>Serialized json <see cref="string"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        public static string Serialize<TModel>(TModel instance)
        {
            _ = instance ?? throw new ArgumentNullException(paramName: nameof(instance));

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TModel));
            using MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, instance);
            stream.Position = 0;
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}