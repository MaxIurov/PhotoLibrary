namespace BOL3
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AlbumToPhoto
    {
        public int AlbumToPhotoID { get; set; }

        public int AlbumID { get; set; }

        public int PhotoID { get; set; }

        public virtual Album Album { get; set; }

        public virtual Photo Photo { get; set; }
    }
}
