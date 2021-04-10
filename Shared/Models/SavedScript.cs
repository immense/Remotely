using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Remotely.Shared.Models
{
    public class SavedScript
    {
        [Required]
        public string Content { get; set; }

        [JsonIgnore]
        public RemotelyUser Creator { get; set; }

        public string CreatorId { get; set; }

        [StringLength(200)]
        public string FolderPath { get; set; }

        public bool GenerateAlertOnError { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public bool IsPublic { get; set; }

        public bool IsQuickScript { get; set; }

        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public Organization Organization { get; set; }

        public string OrganizationID { get; set; }

        public bool SendEmailOnError { get; set; }

        [EmailAddress]
        public string SendErrorEmailTo { get; set; }

        public ScriptingShell Shell { get; set; }
    }
}
