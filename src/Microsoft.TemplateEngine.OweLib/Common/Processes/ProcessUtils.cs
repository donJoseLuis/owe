// Copyright (c) .NET Foundation and contributors. All rights reserved.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TemplateEngine.OweLib.Common.Processes
{
    internal static class ProcessUtils
    {
        internal static async Task<string> LaunchProcessAndGetOutputAsync(string application, string arguments, CancellationToken ct = default)
        {
            _ = string.IsNullOrWhiteSpace(application) ? throw new ArgumentNullException(paramName: nameof(application)) : application;

            try
            {
                ct.ThrowIfCancellationRequested();
                using Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = application;
                if (!string.IsNullOrWhiteSpace(arguments))
                {
                    process.StartInfo.Arguments = arguments;
                }
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                return process.WaitForExit(10000) ? output : string.Empty;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return string.Empty;
            }
        }
    }
}