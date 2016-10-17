using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoLibrary.Models
{
    public class PhotoValidator : AbstractValidator<Photo>
    {
        public PhotoValidator()
        {
            RuleFor(x => x.UserID).NotEmpty().WithMessage("Photo must have UserID");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Photo Name is required.").Length(0, 150);
        }
    }
    public class AlbumValidator : AbstractValidator<Album>
    {
        public AlbumValidator()
        {
            RuleFor(x => x.UserID).NotEmpty().WithMessage("Album must have UserID");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Album Name is required.").Length(0, 150);
            RuleFor(x => x.Name).Must(BeUniqueName).WithMessage("Album already exists.");
            RuleFor(x => x.AlbumSlug).Must(BeUniqueSlug).WithMessage("Album Slug must be unique.");
        }
        private bool BeUniqueName(Album alb,string name)
        {
            var dbAlb = new ApplicationDbContext().Albums.
                Where(x => x.Name.ToLower() == name.ToLower()).SingleOrDefault();
            if (dbAlb == null) return true;
            return dbAlb.AlbumID == alb.AlbumID;
        }
        private bool BeUniqueSlug(Album alb, string slug)
        {
            var dbAlb = new ApplicationDbContext().Albums.
                Where(x => x.AlbumSlug == slug).SingleOrDefault();
            if (dbAlb == null) return true;
            return dbAlb.AlbumID == alb.AlbumID;
        }
    }
}