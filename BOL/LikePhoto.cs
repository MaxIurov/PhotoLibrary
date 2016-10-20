namespace BOL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LikePhoto
    {
        public int LikePhotoID { get; set; }

        public int Liked { get; set; }

        public int PhotoID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Photo Photo { get; set; }
    }
}
