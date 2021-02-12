using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Shared.Utilities
{
    public class JsonSerializerHelper
    {
        public static JsonSerializerOptions IndentedOptions { get; } = new JsonSerializerOptions() { WriteIndented = true };
        public static JsonSerializerOptions CaseInsensitiveOptions { get; } = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }
}
