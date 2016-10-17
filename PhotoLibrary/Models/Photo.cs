using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhotoLibrary.Models
{
    [Validator(typeof(PhotoValidator))]
    public class Photo
    {
        public int PhotoID { get; set; }
        public string Name { get; set; }
        [Display(Name = "Photo")]
        public byte[] Image { get; set; }
        public DateTime? TimeTaken { get; set; }
        public string Location { get; set; }
        public int? DeviceID { get; set; }
        public virtual Device Device { get; set; }
        public int? Focus { get; set; }
        public string Aperture { get; set; }
        public string Shutter { get; set; }
        public string ISO { get; set; }
        public bool? Flash { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<AlbumToPhoto> AlbumsToPhotos { get; set; }

    }
}