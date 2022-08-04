using System;
using System.ComponentModel.DataAnnotations;

namespace Remotely.Shared.ViewModels
{
    public class InviteViewModel
    {
        public string ID { get; set; }
        public bool IsAdmin { get; set; }
        public DateTimeOffset DateSent { get; set; }
        [EmailAddress(ErrorMessage = "The true field is not a valid e-mail address.")]
        public string InvitedUser { get; set; }
    }
}
