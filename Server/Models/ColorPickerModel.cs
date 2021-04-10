using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Models
{
    public class ColorPickerModel
    {
        [DisplayName("Red")]
        public byte Red { get; set; }

        [DisplayName("Green")]
        public byte Green { get; set; }

        [DisplayName("Blue")]
        public byte Blue { get; set; }
    }
}
