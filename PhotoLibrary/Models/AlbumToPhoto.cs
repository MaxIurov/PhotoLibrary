using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoLibrary.Models
{
    public class AlbumToPhoto
    {
        public int AlbumToPhotoID { get; set; }
        public int AlbumID { get; set; }
        public int PhotoID { get; set; }
        public virtual Album Album { get; set; }
        public virtual Photo Photo { get; set; }
    }
}