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

        private const string SwitchUpdate = "--p";

        private const string SwitchGetVersion = "--v";

        private const string SwitchLocation = "--l";

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
            $"WARNING: {AppName} must run as Administrator.{Environment.NewLine}" +
            $"{Environment.NewLine}Usage Options:{Environment.NewLine}" +
            $"{SwitchInstall} [version (MAJOR.MINOR.PATCH)] : installs the specified OWE version. Example: {AppName} {SwitchInstall} 5.1.1{Environment.NewLine}" +
            $"{SwitchUninstall} [version (MAJOR.MINOR.PATCH)] : uninstalls the specified OWE version.  Example: {AppName} {SwitchUninstall} 5.1.1{Environment.NewLine}" +
            $"{SwitchUpdate} [version (MAJOR.MINOR.PATCH)] : updates the specified OWE version.  Example: {AppName} {SwitchUpdate} 5.1.1{Environment.NewLine}" +
            $"{SwitchLocation} [version (MAJOR.MINOR.PATCH)] : shows specified OWE's location on disk. Example: {AppName} {SwitchLocation} 5.1.1{Environment.NewLine}" +
            $"{SwitchGetVersion}                               : lists all OWE installed versions.   Example: {AppName} {SwitchGetVersion}{Environment.NewLine}" +
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
                SwitchLocation => GetLocation,
                SwitchGetVersion => GetVersion,
                SwitchUninstall => UnInstall,
                SwitchUpdate => Update,
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

        private static void Update(string[] args)
        {
            ExecuteOweOperation(args, OweApi.TryUpdate);
        }

        private static void GetVersion(string[] args)
        {
            try
            {
                _ = OweApi.GetOweVersions(); ;
            }
            finally
            {
                Trace.WriteLine("Execution completed.");
            }
        }

        private static void GetLocation(string[] args)
        {
            if (args.Length < 2)
            {
                ShowErrorAndExit("Invoked without a specified version.");
            }

            try
            {
                _ = OweApi.GetOweLocation(args[1]);
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