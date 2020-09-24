using Remotely.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Remotely.Shared.Models
{
    public class RemotelyUserOptions
    {
        [Display(Name = "Console Prompt")]
        [StringLength(5)]
        public string ConsolePrompt { get; set; } = "~>";

        [Display(Name = "Web Shortcut")]
        [StringLength(10)]
        public string CommandModeShortcutWeb { get; set; } = "/web";
        [Display(Name = "PS Core Shortcut")]
        [StringLength(10)]
        public string CommandModeShortcutPSCore { get; set; } = "/pscore";
        [Display(Name = "Windows PS Shortcut")]
        [StringLength(10)]
        public string CommandModeShortcutWinPS { get; set; } = "/winps";
        [Display(Name = "CMD Shortcut")]
        [StringLength(10)]
        public string CommandModeShortcutCMD { get; set; } = "/cmd";
        [Display(Name = "Bash Shortcut")]
        [StringLength(10)]
        public string CommandModeShortcutBash { get; set; } = "/bash";

        [Display(Name = "Theme")]
        public Theme Theme { get; set; }
    }
}
