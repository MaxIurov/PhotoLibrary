using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PhotoLibrary.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<AlbumToPhoto> AlbumsToPhotos { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<LogMessage> LogMessages { get; set; }
        public DbSet<LikePhoto> LikePhotos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Photo>().HasMany<Album>(p => p.Albums).WithMany(a => a.Photos).
            //    Map(ap => { ap.MapLeftKey("PhotoID"); ap.MapRightKey("AlbumID"); ap.ToTable("AlbumPhoto"); });
            base.OnModelCreating(modelBuilder);
        }
        public ApplicationDbContext():base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}