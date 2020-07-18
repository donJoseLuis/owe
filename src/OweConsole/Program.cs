// Copyright (c) .NET Foundation and contributors. All rights reserved.

using Microsoft.TemplateEngine.OweLib;
using System;
using System.Diagnostics;
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


        static Program()
        {
            _ = Trace.Listeners.Add(new ConsoleTraceListener());
            _ = Trace.Listeners.Add(new TextWriterTraceListener($"Log{Guid.NewGuid()}.log", "oweListener"));
            Trace.AutoFlush = true;
        }

        static void Main(string[] args)
        {
            OweApi = OweUtils.DefaultOweFactory.Create();
            ProcessArguments(args);
        }


        private static string HelpMessage => $"{AppName}{Environment.NewLine}Manage the emulated optional workload (OWE){Environment.NewLine}" +
            $"{Environment.NewLine}Options:{Environment.NewLine}" +
            $"{SwitchInstall} [version]: installs the specified OWE version. Example: {AppName} {SwitchInstall} 5.1.1{Environment.NewLine}" +
            $"{SwitchUninstall} [version]: uninstalls the specified OWE version.  Example: {AppName} {SwitchUninstall} 5.1.1{Environment.NewLine}" +
            $"{SwitchUpdate} [version]: updates the specified OWE version.  Example: {AppName} {SwitchUpdate} 5.1.1{Environment.NewLine}" +
            $"{SwitchGetVersion}: lists all OWE installed versions.   Example: {AppName} {SwitchGetVersion}{Environment.NewLine}" +
            $"{SwitchLocation} [version]: shows the location on disk for the spcified OWE. Example: {AppName} {SwitchLocation} 5.1.1{Environment.NewLine}" +
            $"{SwitchHelp}: displays {AppName} help.";


        private static void ShowErrorAndExit(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
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
            Trace.WriteLine($"{args[0]} invoked.");
            try
            {
                Trace.WriteLine(string.Join(",", OweApi.GetOweVersions()));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
            finally
            {
                Trace.WriteLine($"{args[0]} execution completed.");
            }
        }

        private static void GetLocation(string[] args)
        {
            if (args.Length < 2)
            {
                ShowErrorAndExit($"{args[0]} invoked without a specified version.");
            }

            Trace.WriteLine($"{args[0]} invoked with {args[1]}.");
            try
            {
                Trace.WriteLine(OweApi.GetOweLocation(args[1]));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
            finally
            {
                Trace.WriteLine($"{args[0]} execution completed.");
            }
        }

        private static void ExecuteOweOperation(string[] args, FuncPlus oweCommand)
        {
            if (args.Length < 2)
            {
                ShowErrorAndExit($"{args[0]} invoked without a specified version.");
            }

            Trace.WriteLine($"{args[0]} invoked with {args[1]}.");
            if (!oweCommand(args[1], out string error))
            {
                Trace.TraceError(error);
                return;
            }

            Trace.WriteLine($"{args[0]} execution completed.");
        }
    }
}