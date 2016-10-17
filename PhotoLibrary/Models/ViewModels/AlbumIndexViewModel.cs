using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoLibrary.Models.ViewModels
{
    public class AlbumIndexViewModel
    {
        public string UserID { get; set; }
        public bool CanAddAlbum { get; set; }
        public bool CanAddPhoto { get; set; }
        public List<Album> Albums { get; set; }
        public AlbumIndexViewModel()
        {
            Albums = new List<Album>();
        }
    }
}