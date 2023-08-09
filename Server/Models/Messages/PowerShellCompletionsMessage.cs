using Remotely.Shared.Enums;
using Remotely.Shared.Models;

namespace Remotely.Server.Models.Messages;

public record PowerShellCompletionsMessage(PwshCommandCompletion Completion, CompletionIntent Intent);