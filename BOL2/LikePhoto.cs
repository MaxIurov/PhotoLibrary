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
    
    public partial class LikePhoto
    {
        public int LikePhotoID { get; set; }
        public int Liked { get; set; }
        public int PhotoID { get; set; }
        public string UserID { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Photo Photo { get; set; }
    }
}
