using System.IO;

namespace Remotely.Shared.Models
{
    public class Drive
    {
        public DriveType DriveType { get; set; }
        public string RootDirectory { get; set; }
        public string Name { get; set; }
        public string DriveFormat { get; set; }
        public double FreeSpace { get; set; }
        public double TotalSize { get; set; }
        public string VolumeLabel { get; set; }
    }
}
