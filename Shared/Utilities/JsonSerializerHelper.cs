using System.Text.Json;

namespace Remotely.Shared.Utilities;

public class JsonSerializerHelper
{
    public static JsonSerializerOptions IndentedOptions { get; } = new JsonSerializerOptions() { WriteIndented = true };
    public static JsonSerializerOptions CaseInsensitiveOptions { get; } = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
}
