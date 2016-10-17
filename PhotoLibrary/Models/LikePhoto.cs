using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoLibrary.Models
{
    public class LikePhoto
    {
        public int LikePhotoID { get; set; }
        //will be 1 if liked and -1 if disliked so i dont have to deal with nulls later
        public int Liked { get; set; }
        public int PhotoID { get; set; }
        public virtual Photo Photo { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}