using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PhotoLibrary.Models.ViewModels
{
    public class PhotoSmallViewModel
    {
        public int PhotoID { get; set; }
        public string Name { get; set; }
        [Display(Name = "Photo")]
        public byte[] Image { get; set; }
        public string Likes_Dislikes { get; set; }
    }
}