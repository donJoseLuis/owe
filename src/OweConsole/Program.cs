// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace OweConsole
{
    class Program
    {
        private const string SwitchInstall = "--i";

        private const string SwitchUninstall = "--u";

        private const string SwitchLocations = "--l";

        private const string SwitchHelp = "--h";

        private static readonly string AppName = Assembly.GetEntryAssembly().GetName().Name;

        private static IOweApi OweApi;

        private static string LogsDirectory;


        static void Main(string[] args)
        {
            _ = Trace.Listeners.Add(new ConsoleTraceListener());
            LogsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(LogsDirectory))
            {
                Directory.CreateDirectory(LogsDirectory);
            }
            _ = Trace.Listeners.Add(new FormattedMessageTextWriterTraceListener(Path.Combine(LogsDirectory, $"Log_{Guid.NewGuid()}.log"), "oweListener"));
            Trace.AutoFlush = true;

            ShowEmphasisMessage($"Loading {AppName}...");
            OweApi = OweUtils.DefaultOweFactory.Create();
            ShowEmphasisMessage($"{AppName} Loading completed.");
            Trace.WriteLine(string.Empty);

            ProcessArguments(args);
        }


        private static string HelpMessage =>
            $"{AppName}{Environment.NewLine}Manage the optional workload emulator (OWE){Environment.NewLine}" +
            $"WARNING: {AppName} may require to \"run as Administrator\" depending on the folders used.{Environment.NewLine}" +
            $"{Environment.NewLine}Usage Options:{Environment.NewLine}" +
            $"{SwitchInstall} [version (MAJOR.MINOR.PATCH)] : installs the specified OWE version. Example: {AppName} {SwitchInstall} 5.1.1{Environment.NewLine}" +
            $"{SwitchUninstall} [version (MAJOR.MINOR.PATCH)] : uninstalls the specified OWE version.  Example: {AppName} {SwitchUninstall} 5.1.1{Environment.NewLine}" +
            $"{SwitchLocations}                               : shows OWE's templates locations on disk. Example: {AppName} {SwitchLocations}{Environment.NewLine}" +
            $"{SwitchHelp}                               : displays {AppName} help.";


        private static void ShowEmphasisMessage(string message, bool logMessage = true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (logMessage)
            {
                Trace.WriteLine(message);
            }
            else
            {
                Console.WriteLine(message);
            }
            Console.ResetColor();
        }

        private static void ShowErrorAndExit(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine(message);
            }
            Console.ResetColor();
            Console.WriteLine(HelpMessage);
            Environment.Exit(1);
        }

        private static void ProcessArguments(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                ShowErrorAndExit("Invalid number of arguments.");
                return;
            }

            Action<string[]> method = args[0] switch
            {
                SwitchInstall => Install,
                SwitchLocations => GetTemplateLocations,
                SwitchUninstall => UnInstall,
                SwitchHelp => x => ShowErrorAndExit(string.Empty),
                _ => null
            };

            if (method == null)
            {
                ShowErrorAndExit($"Unsupported option {args[0]}.");
                return;
            }

            method(args);
        }

        private static void Install(string[] args)
        {
            ExecuteOweOperation(args, OweApi.TryInstall);
        }

        private static void UnInstall(string[] args)
        {
            ExecuteOweOperation(args, OweApi.TryUninstall);
        }

        private static void GetTemplateLocations(string[] args)
        {
            try
            {
                string [] locations = OweApi.GetOweTemplateLocations();
                if (locations != null && locations.Length > 0)
                {
                    Trace.WriteLine($"Locations: {string.Join(",", locations)}");
                }
                else
                {
                    Trace.WriteLine("No templates found on disk");
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
            finally
            {
                Trace.WriteLine("Execution completed.");
            }
        }

        private static void ExecuteOweOperation(string[] args, FuncPlus oweCommand)
        {
            if (args.Length < 2)
            {
                ShowErrorAndExit("Invoked without a required version.");
            }

            if (oweCommand(args[1], out _))
            {
                Trace.WriteLine("Execution completed.");
            }
        }
    }
}