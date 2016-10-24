using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BOL
{
    public class PhotoValidation
    {
        [Required]
        public int PhotoID { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        [Required]
        public string UserID { get; set; }
    }
    [MetadataType(typeof(PhotoValidation))]
    public partial class Photo
    {
    }
}
