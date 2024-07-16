using System.CommandLine;
using CommunityToolkit.Diagnostics;
using Immense.RemoteControl.Desktop.Shared.Enums;

namespace Immense.RemoteControl.Desktop.Shared.Startup;
public static class CommandProvider
{
    /// <summary>
    /// Creates a <see cref="Command"/> for starting the remote control client.
    /// </summary>
    /// <param name="isRootCommand">Whether to create a <see cref="RootCommand"/> or <see cref="Command"/>.</param>
    /// <param name="commandLineDescription">The description for the command.</param>
    /// <param name="commandName">The name used to invoke the command.  Required if not a root command.</param>
    /// <returns></returns>
    public static Command CreateRemoteControlCommand(
        bool isRootCommand,
        string commandLineDescription,
        string commandName = "")
    {
        Command? rootCommand;

        if (isRootCommand)
        {
            rootCommand = new RootCommand(commandLineDescription);
        }
        else
        {
            Guard.IsNotNullOrWhiteSpace(commandName);
            rootCommand = new Command(commandName, commandLineDescription);
        }

        var hostOption = new Option<string>(
            new[] { "-h", "--host" },
            "The hostname of the server to which to connect (e.g. https://example.com).");
        rootCommand.AddOption(hostOption);

        var modeOption = new Option<AppMode>(
            new[] { "-m", "--mode" },
            () => AppMode.Attended,
            "The remote control mode to use.  Either Attended, Unattended, or Chat.");
        rootCommand.AddOption(modeOption);


        var pipeNameOption = new Option<string>(
            new[] { "-p", "--pipe-name" },
            "When AppMode is Chat, this is the pipe name used by the named pipes server.");
        pipeNameOption.AddValidator((context) =>
        {
            if (context.GetValueForOption(modeOption) == AppMode.Chat &&
                string.IsNullOrWhiteSpace(context.GetValueOrDefault<string>()))
            {
                context.ErrorMessage = "A pipe name must be specified when AppMode is Chat.";
            }
        });
        rootCommand.AddOption(pipeNameOption);

        var sessionIdOption = new Option<string>(
           new[] { "-s", "--session-id" },
           "In Unattended mode, this unique session ID will be assigned to this connection and " +
           "shared with the server.  The connection can then be found in the RemoteControlSessionCache " +
           "using this ID.");
        rootCommand.AddOption(sessionIdOption);

        var accessKeyOption = new Option<string>(
            new[] { "-a", "--access-key" },
            "In Unattended mode, secures access to the connection using the provided key.");
        rootCommand.AddOption(accessKeyOption);

        var requesterNameOption = new Option<string>(
            new[] { "-r", "--requester-name" },
               "The name of the technician requesting to connect.");
        rootCommand.AddOption(requesterNameOption);

        var organizationNameOption = new Option<string>(
            new[] { "-o", "--org-name" },
            "The organization name of the technician requesting to connect.");
        rootCommand.AddOption(organizationNameOption);

        var relaunchOption = new Option<bool>(
            "--relaunch",
            "Used to indicate that process is being relaunched from a previous session " +
            "and should notify viewers when it's ready.");
        rootCommand.AddOption(relaunchOption);

        var viewersOption = new Option<string>(
            "--viewers",
            "Used with --relaunch.  Should be a comma-separated list of viewers' " +
            "SignalR connection IDs.");
        rootCommand.AddOption(viewersOption);

        var elevateOption = new Option<bool>(
            "--elevate",
            "Must be called from a Windows service.  The process will relaunch " +
            "itself in the console session with elevated rights.");
        rootCommand.AddOption(elevateOption);

        return rootCommand;
    }
}
