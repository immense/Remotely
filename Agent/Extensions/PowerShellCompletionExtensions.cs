using Remotely.Shared.Models;
using Remotely.Shared.Utilities;
using System.Linq;
using System.Management.Automation;

namespace Remotely.Agent.Extensions;

public static class PowerShellCompletionExtensions
{
    public static PwshCommandCompletion ToPwshCompletion(this CommandCompletion completion)
    {
        return new PwshCommandCompletion()
        {
            CurrentMatchIndex = completion.CurrentMatchIndex,
            ReplacementIndex = completion.ReplacementIndex,
            ReplacementLength = completion.ReplacementLength,
            CompletionMatches = completion.CompletionMatches
                .Select(x => new PwshCompletionResult(x.CompletionText,
                    x.ListItemText, 
                    EnumMapper.ToEnum<PwshCompletionResultType, CompletionResultType>(x.ResultType),
                    x.ToolTip))
                .ToList()
        };
       
    }
}
