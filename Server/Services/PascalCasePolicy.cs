using System.Linq;
using System.Text.Json;

namespace Remotely.Server.Services
{
    public class PascalCasePolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            var first = name.First().ToString().ToUpper();

            return first + new string(name.Skip(1).ToArray());
        }
    }
}
