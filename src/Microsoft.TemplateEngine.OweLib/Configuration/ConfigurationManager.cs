// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TemplateEngine.OweLib.Configuration
{
    [DataContract]
    internal sealed class ConfigurationManager
    {
        static ConfigurationManager()
        {
            string settingsFile = Path.Combine(Directory.GetCurrentDirectory(), "app.config.json");
            if (File.Exists(settingsFile))
            {
                string json = File.ReadAllText(settingsFile);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    try
                    {
                        AppSettings = JsonSerializer.Deserialize<ConfigurationManager>(json);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceWarning($"Failed to deserialize settings file.  Details: {e.Message}");
                    }
                }
            }
        }


        [IgnoreDataMember]
        internal static ConfigurationManager AppSettings { get; }

        [IgnoreDataMember]
        internal string this[string key]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return string.Empty;
                }

                AppSetting firstMatch = Settings.FirstOrDefault(x => x.Key == key);
                return firstMatch == null ? string.Empty : firstMatch.Value;
            }
        }

        [IgnoreDataMember]
        internal string this[AppKey key]
        {
            get
            {
                string keyString = key.ToString();
                AppSetting firstMatch = Settings.FirstOrDefault(x => x.Key == keyString);
                return firstMatch == null ? string.Empty : firstMatch.Value;
            }
        }

        [DataMember(Name = "settings")]
        internal List<AppSetting> Settings { get; set; }
    }
}