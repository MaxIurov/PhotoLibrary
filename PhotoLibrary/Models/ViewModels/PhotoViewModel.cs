using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoLibrary.Models.ViewModels
{
    public class PhotoViewModel
    {
        public int PhotoID { get; set; }
        public string Name { get; set; }
        [Display(Name = "Photo")]
        public byte[] Image { get; set; }
        [Display(Name ="Time Taken")]
        public DateTime? TimeTaken { get; set; }
        [Display(Name = "Time Taken")]
        public string TimeTakenStr { get; set; }
        public string Location { get; set; }
        public int? DeviceID { get; set; }
        public IEnumerable<SelectListItem> DeviceList { get; set; }
        public int? Focus { get; set; }
        public string Aperture { get; set; }
        public string Shutter { get; set; }
        public string ISO { get; set; }
        public bool? Flash { get; set; }
        public string UserID { get; set; }
        public List<CheckBoxViewModel> Albums { get; set; }
        public PhotoViewModel()
        {
            Albums = new List<CheckBoxViewModel>();
        }
    }
}