using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Remotely.Server.Services
{
    public class PascalCase : JsonNamingPolicy
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
