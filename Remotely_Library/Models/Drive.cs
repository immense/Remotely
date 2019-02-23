using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace Remotely_Library.Models
{
    public class Drive
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public DriveType DriveType { get; set; }
        public string RootDirectory { get; internal set; }
        public string Name { get; internal set; }
        public string DriveFormat { get; internal set; }
        public double FreeSpace { get; internal set; }
        public double TotalSize { get; internal set; }
        public string VolumeLabel { get; internal set; }
        public string MachineID { get; set; }
    }
}
