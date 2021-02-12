using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Remotely.Agent.Installer.Win.Utilities
{
    public class CommandLineParser
    {
        private static Dictionary<string, string> commandLineArgs;
        private static bool _invalidArgumentFound;

        public static Dictionary<string, string> CommandLineArgs
        {
            get
            {
                if (commandLineArgs is null)
                {
                    commandLineArgs = new Dictionary<string, string>();

                    var args = Environment.GetCommandLineArgs();
                    for (var i = 1; i < args.Length; i += 2)
                    {
                        try
                        {
                            var key = args?[i];
                            if (key != null)
                            {
                                if (!key.Contains("-"))
                                {
                                    _invalidArgumentFound = true;
                                    i -= 1;
                                    continue;
                                }
                                key = key.Trim().Replace("-", "").ToLower();
                                if (i + 1 == args.Length)
                                {
                                    commandLineArgs.Add(key, "true");
                                    continue;
                                }
                                var value = args[i + 1];
                                if (value != null)
                                {
                                    if (value.StartsWith("-"))
                                    {
                                        commandLineArgs.Add(key, "true");
                                        i -= 1;
                                    }
                                    else
                                    {
                                        commandLineArgs.Add(key, args[i + 1].Trim());
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Write(ex);
                        }

                    }
                }
                return commandLineArgs;
            }
        }

        internal static void VerifyArguments()
        {
            if (_invalidArgumentFound)
            {
                Logger.Write("Command line arguments are invalid.");
                MessageBoxEx.Show("Command line arguments are invalid.", "Invalid Arguments", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
