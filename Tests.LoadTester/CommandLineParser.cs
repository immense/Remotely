using System;
using System.Collections.Generic;

namespace Remotely.Tests.LoadTester
{
    public class CommandLineParser
    {
        private static Dictionary<string, string> commandLineArgs;
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
                                    i -= 1;
                                    continue;
                                }

                                key = key.Trim().Replace("-", "").ToLower();

                                commandLineArgs.Add(key, args[i + 1]);
                            }
                        }
                        catch { }

                    }
                }
                return commandLineArgs;
            }
        }
    }
}
