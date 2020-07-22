// Copyright (c) .NET Foundation and contributors. All rights reserved.

using System.Runtime.Serialization;

namespace Microsoft.TemplateEngine.OweLib.Configuration
{
    [DataContract]
    internal sealed class AppSetting
    {
        [DataMember(Name = "key", IsRequired = true)]
        public string Key { get; set; }

        [DataMember(Name = "value", IsRequired = true)]
        public string Value { get; set; }

        public override string ToString()
        {
            return $"Key: {Key}, Value: {Value}";
        }
    }
}