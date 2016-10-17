using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PhotoLibrary.Models
{
    [Validator(typeof(AlbumValidator))]
    public class Album
    {
        public int AlbumID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<AlbumToPhoto> AlbumsToPhotos { get; set; }
        public string AlbumSlug { get; set; }
        public string GetAlbumSlug()
        {
            string str = this.Name.ToLower();
            // get rid of hyphens so the route doesn’t break
            str = str.Replace("-", " ");
            // invalid chars          
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space  
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = Regex.Replace(str, @"\s", "_");
            //what if slug already exists?
            string slug = str;
            bool AllOK = false;
            int counter = 1;
            do
            {
                var dbAlb = new ApplicationDbContext().Albums.Where(x => x.AlbumSlug == slug).SingleOrDefault();
                if (dbAlb == null)
                {
                    AllOK = true;
                }
                else
                {
                    slug = str + counter.ToString();
                }
                counter++;
            }
            while (!AllOK);
            return slug;
        }
    }
}