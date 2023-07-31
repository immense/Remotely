using System;
using System.Linq;
using System.Collections.Generic;

namespace Remotely.Tests.LoadTester;

public class CommandLineParser
{
    private static Dictionary<string, string>? _commandLineArgs;

    public static Dictionary<string, string> CommandLineArgs
    {
        get
        {
            if (_commandLineArgs is null)
            {
                _commandLineArgs = new Dictionary<string, string>();

                var args = Environment.GetCommandLineArgs();
                if (args?.Any() != true)
                {
                    return _commandLineArgs;
                }

                for (var i = 1; i < args.Length; i += 2)
                {
                    try
                    {
                        var key = args[i];
                        if (key != null)
                        {
                            if (!key.Contains("-"))
                            {
                                i -= 1;
                                continue;
                            }

                            key = key.Trim().Replace("-", "").ToLower();

                            _commandLineArgs.Add(key, args[i + 1]);
                        }
                    }
                    catch { }

                }
            }
            return _commandLineArgs;
        }
    }
}
