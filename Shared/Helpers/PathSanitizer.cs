using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Immense.RemoteControl.Shared.Helpers;

public static class PathSanitizer
{
    /// <summary>
    /// Sanitizes a file name by removing invalid characters.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string SanitizeFileName(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        var invalidChars = Path.GetInvalidFileNameChars().ToHashSet();
        var validChars = fileName.Where(x => !invalidChars.Contains(x));
        return new string(validChars.ToArray());
    }

    /// <summary>
    /// Sanitizes a path by removing invalid characters.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string SanitizePath(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        var invalidChars = Path.GetInvalidPathChars().ToHashSet();
        var validChars = path.Where(x => !invalidChars.Contains(x));
        return new string(validChars.ToArray());
    }
}
