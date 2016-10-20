namespace BOL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Album
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Album()
        {
            AlbumToPhotoes = new HashSet<AlbumToPhoto>();
        }

        public int AlbumID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        public string AlbumSlug { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlbumToPhoto> AlbumToPhotoes { get; set; }
    }
}
