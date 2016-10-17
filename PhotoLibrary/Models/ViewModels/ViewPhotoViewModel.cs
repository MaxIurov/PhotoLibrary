﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PhotoLibrary.Models.ViewModels
{
    public class ViewPhotoViewModel
    {
        [Key]
        public int PhotoID { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public string Name { get; set; }
        [Display(Name = "Photo")]
        public byte[] Image { get; set; }
        [Display(Name = "Time Taken")]
        public string TimeTakenStr { get; set; }
        public string Location { get; set; }
        public string Device { get; set; }
        public int? Focus { get; set; }
        public string Aperture { get; set; }
        public string Shutter { get; set; }
        public string ISO { get; set; }
        public bool? Flash { get; set; }
        public string UserID { get; set; }
        public int BackToAlbumID { get; set; }
        public int NLikes { get; set; }
        public int NDislikes { get; set; }
        public bool CanLike { get; set; }
        public bool ModelLiked { get; set; }
        public bool ModelDisliked { get; set; }
    }
}