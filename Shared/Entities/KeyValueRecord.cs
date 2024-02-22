using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Entities;

public class KeyValueRecord
{
    [Key]
    public Guid Key { get; set; }
    public string? Value { get; set; }
}
