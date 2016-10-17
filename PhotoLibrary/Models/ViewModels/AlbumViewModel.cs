using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoLibrary.Models.ViewModels
{
    public class AlbumViewModel
    {
        public int AlbumID { get; set; }
        public string Name { get; set; }
        public string UserID { get; set; }
        public string Description { get; set;}
        public bool CanAddAlbum { get; set; }
        public bool CanAddPhoto { get; set; }
        public List<PhotoCheckBoxViewModel> Photos { get; set; }
        public AlbumViewModel()
        {
            Photos = new List<PhotoCheckBoxViewModel>();
        }
    }
}