using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PagedList;
using System.Web.Mvc;

namespace PhotoLibrary.Models.ViewModels
{
    public class PhotoSearchModel
    {
        public string Name { get; set; }
        public DateTime? TimeFrom{ get; set; }
        public DateTime? TimeTo { get; set; }
        public string Location { get; set; }
        public int? DeviceID { get; set; }
        public IEnumerable<SelectListItem> DeviceList { get; set; }
        public int? Focus { get; set; }
        public string Aperture { get; set; }
        public string Shutter { get; set; }
        public string ISO { get; set; }
        public bool? Flash { get; set; }
        public List<Photo> SearchResults { get; set; }
        public PhotoSearchModel()
        {
            SearchResults = new List<Photo>();
        }
    }
}