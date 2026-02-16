using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Remotely.Shared.Models.Dtos;


[DataContract]
public class FileDto
{
    private static readonly char[] InvalidPathChars = Path.GetInvalidFileNameChars();

    [DataMember(Name = "Buffer")]
    public byte[] Buffer { get; set; } = Array.Empty<byte>();

    [DataMember(Name = "FileName")]
    [MaxLength(255)]
    [RegularExpression(@"^[^\\/:*?""<>|]+$", ErrorMessage = "Invalid file name")]
    public string FileName 
    { 
        get => _fileName;
        set => _fileName = SanitizeFileName(value);
    }
    private string _fileName = string.Empty;

    private static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;
        
        var sanitized = fileName.Trim();
        foreach (var c in InvalidPathChars)
        {
            sanitized = sanitized.Replace(c, '_');
        }
        
        if (sanitized.StartsWith(".") || sanitized.Contains(".."))
            sanitized = sanitized.TrimStart('.');
        
        return sanitized;
    }

    [DataMember(Name = "MessageId")]
    public string MessageId { get; set; } = string.Empty;

    [DataMember(Name = "EndOfFile")]
    public bool EndOfFile { get; set; }

    [DataMember(Name = "StartOfFile")]
    public bool StartOfFile { get; set; }
}
