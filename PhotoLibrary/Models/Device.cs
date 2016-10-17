using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PhotoLibrary.Models
{
    public class Device
    {
        public int DeviceID { get; set; }
        [Display(Name ="Device")]
        public string Name { get; set; }
    }
}