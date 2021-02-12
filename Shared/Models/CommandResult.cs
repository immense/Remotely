using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Remotely.Shared.Models
{
    public class CommandResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ID { get; set; }
        public string CommandMode { get; set; }
        public string CommandText { get; set; }
        public string SenderUserID { get; set; }
        public string SenderConnectionID { get; set; }
        public string[] TargetDeviceIDs { get; set; }

        [NotMapped]
        public ICollection<PSCoreCommandResult> PSCoreResults { get; set; } = new List<PSCoreCommandResult>();
        [NotMapped]
        public ICollection<GenericCommandResult> CommandResults { get; set; } = new List<GenericCommandResult>();

        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;

        [JsonIgnore]
        [IgnoreDataMember]
        public Organization Organization { get; set; }

        public string OrganizationID { get; set; }
    }
}
