using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class ScreenDataDto : IDynamicDto
    {
        public ScreenDataDto(string selectedScreen, string[] displayNames)
        {
            SelectedScreen = selectedScreen;
            DisplayNames = displayNames;
        }

        [DataMember(Name = "DisplayNames")]
        public string[] DisplayNames { get; }


        [DataMember(Name = "DtoType")]
        public DynamicDtoType DtoType { get; } = DynamicDtoType.ScreenData;

        [DataMember(Name = "SelectedScreen")]
        public string SelectedScreen { get; }
    }
}
