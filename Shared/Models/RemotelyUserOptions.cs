using Remotely.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Remotely.Shared.Models
{
    public class RemotelyUserOptions
    {
        [Display(Name = "Display Name")]
        [StringLength(100)]
        public string DisplayName { get; set; }

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
