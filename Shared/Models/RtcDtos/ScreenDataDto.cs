using Remotely.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Remotely.Shared.Models.RtcDtos
{
    [DataContract]
    public class ScreenDataDto : BinaryDtoBase
    {
        public ScreenDataDto(string selectedScreen, string[] displayNames)
        {
            SelectedScreen = selectedScreen;
            DisplayNames = displayNames;
        }

        [DataMember(Name = "DisplayNames")]
        public string[] DisplayNames { get; }


        [DataMember(Name = "DtoType")]
        public new BinaryDtoType DtoType { get; } = BinaryDtoType.ScreenData;

        [DataMember(Name = "SelectedScreen")]
        public string SelectedScreen { get; }
    }
}
