using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Models
{
    public class PwshCommandCompletion
    {
        public int CurrentMatchIndex { get; set; }
        public int ReplacementIndex { get; set; }
        public int ReplacementLength { get; set; }
        public List<PwshCompletionResult> CompletionMatches { get; set; }
    }


    public class PwshCompletionResult
    {
        public PwshCompletionResult() { }
        public PwshCompletionResult(string completionText, string listItemText, PwshCompletionResultType resultType, string toolTip)
        {
            CompletionText = completionText;
            ListItemText = listItemText;
            ResultType = resultType;
            ToolTip = toolTip;
        }

        public string CompletionText { get; set; }

        public string ListItemText { get; set; }

        public PwshCompletionResultType ResultType { get; set; }

        public string ToolTip { get; set; }
    }

    public enum PwshCompletionResultType
    {
        Command = 2,
        DynamicKeyword = 13,
        History = 1,
        Keyword = 12,
        Method = 6,
        Namespace = 10,
        ParameterName = 7,
        ParameterValue = 8,
        Property = 5,
        ProviderContainer = 4,
        ProviderItem = 3,
        Text = 0,
        Type = 11,
        Variable = 9
    }
}
