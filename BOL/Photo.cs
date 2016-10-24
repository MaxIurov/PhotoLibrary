namespace BOL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Photo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Photo()
        {
            AlbumToPhotoes = new HashSet<AlbumToPhoto>();
            LikePhotoes = new HashSet<LikePhoto>();
        }

        public int PhotoID { get; set; }

        public string Name { get; set; }
        [Display(Name = "Photo")]
        public byte[] Image { get; set; }

        public DateTime? TimeTaken { get; set; }

        public string Location { get; set; }

        public int? DeviceID { get; set; }

        public int? Focus { get; set; }

        public string Aperture { get; set; }

        public string Shutter { get; set; }

        public string ISO { get; set; }

        public bool? Flash { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlbumToPhoto> AlbumToPhotoes { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Device Device { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LikePhoto> LikePhotoes { get; set; }
    }
}
