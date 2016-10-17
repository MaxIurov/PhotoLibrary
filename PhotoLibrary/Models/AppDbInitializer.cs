using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace PhotoLibrary.Models
{
    //public class AppDbInitializer:DropCreateDatabaseAlways<ApplicationDbContext>
    public class AppDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var role1 = new IdentityRole { Name = "Admin" };
            var role2 = new IdentityRole { Name = "FreeUser" };
            var role3 = new IdentityRole { Name = "PremiumUser" };
            roleManager.Create(role1);
            roleManager.Create(role2);
            roleManager.Create(role3);

            var admin = new ApplicationUser { UserName = "Admin", Email = "admin@mail.com", CanAddAlbum = true, CanAddPhoto = true };
            string pass = "P@ssword1";
            var res = userManager.Create(admin, pass);
            if (res.Succeeded)
            {
                userManager.AddToRole(admin.Id, role1.Name);
                userManager.AddToRole(admin.Id, role2.Name);
                userManager.AddToRole(admin.Id, role3.Name);
            }
            Settings s1 = new Settings { SettingsID = "FreeUserPhotos", Value = "10" };
            Settings s2 = new Settings { SettingsID = "FreeUserAlbums", Value = "2" };
            context.Settings.Add(s1);
            context.Settings.Add(s2);
            Device d0 = new Device { Name = "Empty" };
            Device d1 = new Device { Name = "Sony" };
            Device d2 = new Device { Name = "Nikon" };
            Device d3 = new Device { Name = "Fuji" };
            Device d4 = new Device { Name = "Panasonic" };
            Device d5 = new Device { Name = "Canon" };
            context.Devices.Add(d0);
            context.Devices.Add(d1);
            context.Devices.Add(d2);
            context.Devices.Add(d3);
            context.Devices.Add(d4);
            context.Devices.Add(d5);
            //add stored procedure
            string sqlString = @"
                DECLARE @CreateOrAlter VARCHAR(3000)
                IF EXISTS (SELECT * FROM sys.objects WHERE type='P' AND name='sp_SearchPhoto')
                BEGIN
                    SET @CreateOrAlter='ALTER PROCEDURE '
                END
                ELSE
                BEGIN
                    SET @CreateOrAlter='CREATE PROCEDURE '
                END
                EXEC(
                @CreateOrAlter+'[dbo].[sp_SearchPhoto](@UserID NVARCHAR(255),@SString NVARCHAR(255))
                AS
                BEGIN
                    SELECT distinct p.* FROM dbo.Photos p
                    left join dbo.AlbumToPhotoes t on p.PhotoID=t.PhotoID
                    where ((p.Name like ''%''+@SString+''%'') or (@SString='''')) and ((t.AlbumID is not null) or (@UserID=p.UserID))
                END
                ')";
            context.Database.ExecuteSqlCommand(sqlString);

            base.Seed(context);
        }
    }
}