using Remotely.Shared.Enums;

namespace Remotely.Shared.Models
{
    interface IDynamicDto
    {
        DynamicDtoType DtoType { get; }
    }
}
