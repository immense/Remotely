using Remotely.Shared.Enums;
using System.Runtime.Serialization;

namespace Remotely.Shared.Models.RemoteControlDtos
{
    [DataContract]
    public class ScreenDataDto : BaseDto
    {
        public ScreenDataDto(string selectedScreen, string[] displayNames)
        {
            SelectedScreen = selectedScreen;
            DisplayNames = displayNames;
        }

        [DataMember(Name = "DisplayNames")]
        public string[] DisplayNames { get; }


        [DataMember(Name = "DtoType")]
        public new BaseDtoType DtoType { get; } = BaseDtoType.ScreenData;

        [DataMember(Name = "SelectedScreen")]
        public string SelectedScreen { get; }
    }
}
