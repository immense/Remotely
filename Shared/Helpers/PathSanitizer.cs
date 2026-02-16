namespace Remotely.Shared.Helpers;

public static class PathSanitizer
{
    private static readonly HashSet<char> InvalidPathChars = 
        Path.GetInvalidPathChars().ToHashSet();

    /// <summary>
    /// Sanitizes a file name by removing invalid characters and preventing path traversal.
    /// </summary>
    public static string SanitizeFileName(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        // Remove directory separators and parent path references
        var sanitized = fileName
            .Replace("..", "")
            .Replace(Path.DirectorySeparatorChar.ToString(), "")
            .Replace(Path.AltDirectorySeparatorChar.ToString(), "")
            .Replace('/', '\0')
            .Replace('\\', '\0');

        var invalidChars = Path.GetInvalidFileNameChars().ToHashSet();
        var validChars = sanitized.Where(x => !invalidChars.Contains(x));
        return new string(validChars.ToArray());
    }

    /// <summary>
    /// Sanitizes a path by removing invalid characters and preventing path traversal.
    /// </summary>
    public static string SanitizePath(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        // Normalize and prevent path traversal
        var normalizedPath = Path.GetFullPath(path);
        
        // Check for path traversal attempts
        if (normalizedPath.Contains(".."))
        {
            throw new ArgumentException("Path traversal attempt detected.", nameof(path));
        }

        var validChars = path.Where(x => !InvalidPathChars.Contains(x));
        return new string(validChars.ToArray());
    }

    /// <summary>
    /// Validates that a path is within an allowed directory.
    /// </summary>
    public static bool IsPathSafe(string requestedPath, string baseDirectory)
    {
        try
        {
            var baseDir = Path.GetFullPath(baseDirectory);
            var fullPath = Path.GetFullPath(requestedPath);
            return fullPath.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
