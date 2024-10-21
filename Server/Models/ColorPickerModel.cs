using System.ComponentModel;

namespace Remotely.Server.Models;

public class ColorPickerModel
{
    [DisplayName("Red")]
    public byte Red { get; set; }

    [DisplayName("Green")]
    public byte Green { get; set; }

    [DisplayName("Blue")]
    public byte Blue { get; set; }
}
