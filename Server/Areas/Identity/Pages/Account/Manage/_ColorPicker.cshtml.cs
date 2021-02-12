using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Remotely.Server.Areas.Identity.Pages.Account.Manage
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
