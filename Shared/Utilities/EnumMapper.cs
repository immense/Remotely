using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.Utilities
{
    public static class EnumMapper
    {
        public static ToType ToEnum<ToType, FromType>(FromType fromValue)
            where ToType : Enum
            where FromType : Enum
        {
            return (ToType)Enum.Parse(typeof(ToType), fromValue.ToString());
        }
    }
}
