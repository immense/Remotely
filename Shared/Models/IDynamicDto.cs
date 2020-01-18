using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remotely.Shared.Models
{
    interface IDynamicDto
    {
        DynamicDtoType DtoType { get; }
    }
}
