using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BOL
{
    public class UniqueAlbumNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PhotoLibraryModel db = new PhotoLibraryModel();
            string AlbumName = value.ToString();
            Album AlbValidate = (Album)validationContext.ObjectInstance;
            var dbAlb = db.Albums.Where(x => x.Name.ToLower() == AlbumName.ToLower()).SingleOrDefault();
            if (dbAlb == null) return ValidationResult.Success;
            if (dbAlb.AlbumID == AlbValidate.AlbumID) return ValidationResult.Success;
            return new ValidationResult("Album with this Name already exists.");
            //return base.IsValid(value, validationContext);
        }
    }
    public class UniqueAlbumSlugAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PhotoLibraryModel db = new PhotoLibraryModel();
            string AlbumSlug = value.ToString();
            Album AlbValidate = (Album)validationContext.ObjectInstance;
            var dbAlb = db.Albums.Where(x => x.AlbumSlug == AlbumSlug).SingleOrDefault();
            if (dbAlb == null) return ValidationResult.Success;
            if (dbAlb.AlbumID == AlbValidate.AlbumID) return ValidationResult.Success;
            return new ValidationResult("Album Slug must be unique.");
            //return base.IsValid(value, validationContext);
        }
    }
    public class AlbumValidation
    {
        [Required]
        public int AlbumID { get; set; }
        [Required]
        [StringLength(150)]
        [UniqueAlbumName]
        public string Name { get; set; }
        [Required]
        public string UserID { get; set; }
        [Required]
        [UniqueAlbumSlug]
        public string AlbumSlug { get; set; }
    }
    [MetadataType(typeof(AlbumValidation))]
    public partial class Album
    {
    }
}
