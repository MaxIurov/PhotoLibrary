namespace BOL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PhotoLibraryModel : DbContext
    {
        public PhotoLibraryModel()
            : base("name=PhotoLibraryConnection")
        {
        }

        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<AlbumToPhoto> AlbumToPhotoes { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<LikePhoto> LikePhotoes { get; set; }
        public virtual DbSet<LogMessage> LogMessages { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Albums)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.UserID);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.LikePhotoes)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.UserID);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Photos)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.UserID);
        }
    }
}
