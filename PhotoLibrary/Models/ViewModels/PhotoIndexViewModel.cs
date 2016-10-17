using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoLibrary.Models.ViewModels
{
    public class PhotoIndexViewModel
    {
        public string UserID { get; set; }
        public bool CanAddAlbum { get; set; }
        public bool CanAddPhoto { get; set; }
        public List<PhotoSmallViewModel> Photos { get; set; }
        public PhotoIndexViewModel()
        {
            Photos = new List<PhotoSmallViewModel>();
        }
    }
}