//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BOL2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Photo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Photo()
        {
            this.AlbumToPhotoes = new HashSet<AlbumToPhoto>();
            this.LikePhotoes = new HashSet<LikePhoto>();
        }
    
        public int PhotoID { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public Nullable<System.DateTime> TimeTaken { get; set; }
        public string Location { get; set; }
        public Nullable<int> DeviceID { get; set; }
        public Nullable<int> Focus { get; set; }
        public string Aperture { get; set; }
        public string Shutter { get; set; }
        public string ISO { get; set; }
        public Nullable<bool> Flash { get; set; }
        public string UserID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AlbumToPhoto> AlbumToPhotoes { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Device Device { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LikePhoto> LikePhotoes { get; set; }
    }
}
