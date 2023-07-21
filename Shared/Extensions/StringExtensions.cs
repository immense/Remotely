using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Extensions;

public static class StringExtensions
{
    public static string SanitizeFileName(this string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return string.Empty;
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var validChars = fileName
            .Where(x => !invalidChars.Contains(x))
            .ToArray();

        return new string(validChars);
    }

    public static string SanitizeFileSystemPath(this string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        var invalidChars = Path.GetInvalidPathChars();
        var validChars = path
            .Where(x => !invalidChars.Contains(x))
            .ToArray();

        return new string(validChars);
    }
}
