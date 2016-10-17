using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhotoLibrary.Models;
using PhotoLibrary.Models.ViewModels;
using Microsoft.AspNet.Identity;

namespace PhotoLibrary.Controllers
{
    [Authorize]
    public class AlbumsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public async Task<ActionResult> Index()
        {
            string usrid = User.Identity.GetUserId();
            ViewBag.UserID = usrid;
            List<Album> la = await db.Albums.Where(u => u.User.Id == usrid).ToListAsync();
            AlbumIndexViewModel aivm = new AlbumIndexViewModel();
            aivm.UserID = usrid;
            var thisUser = await db.Users.Where(u => u.Id == usrid).SingleOrDefaultAsync();
            aivm.CanAddAlbum = thisUser.CanAddAlbum.GetValueOrDefault(false);
            aivm.CanAddPhoto = thisUser.CanAddPhoto.GetValueOrDefault(false);
            aivm.Albums = la;
            return View(aivm);
        }
        public async Task<ActionResult> Open(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = await db.Albums.FindAsync(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            var PhotoList = await (from p in db.Photos
                                   join ap in db.AlbumsToPhotos on p.PhotoID equals ap.PhotoID
                                   where (p.UserID == album.UserID) & (ap.AlbumID == album.AlbumID)
                                   select new
                                   {
                                       p.PhotoID,
                                       p.Name,
                                       p.Image,
                                       Checked = false
                                   }).ToListAsync();
            AlbumViewModel alb_vm = new AlbumViewModel();
            alb_vm.AlbumID = album.AlbumID;
            alb_vm.Name = album.Name;
            alb_vm.Description = album.Description;
            alb_vm.UserID = album.UserID;
            alb_vm.CanAddAlbum = album.User.CanAddAlbum.GetValueOrDefault(false);
            alb_vm.CanAddPhoto = album.User.CanAddPhoto.GetValueOrDefault(false);
            var MyCheckBoxList = new List<PhotoCheckBoxViewModel>();
            foreach (var item in PhotoList)
            {
                MyCheckBoxList.Add(new PhotoCheckBoxViewModel { Id = item.PhotoID, Name = item.Name, Image = item.Image, Checked = item.Checked });
            }
            alb_vm.Photos = MyCheckBoxList;
            return View(alb_vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Open(AlbumViewModel m)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in db.AlbumsToPhotos)
                {
                    if (item.AlbumID == m.AlbumID)
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }
                }
                foreach (var item in m.Photos)
                {
                    if (!item.Checked)
                    {
                        db.AlbumsToPhotos.Add(new AlbumToPhoto() { AlbumID = m.AlbumID, PhotoID = item.Id });
                    }
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Open", new { id = m.AlbumID });
            }
            return View(m);
        }
        public ActionResult Create()
        {
            string usrid = User.Identity.GetUserId();
            ViewBag.UserID = usrid;
            Album m = new Album() { UserID = usrid };
            return View(m);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Description,UserID")] Album album)
        {
            album.AlbumSlug = album.GetAlbumSlug();
            if (ModelState.IsValid)
            {
                string usrid = User.Identity.GetUserId();
                //check if CanAddAlbum
                var dbUser = await db.Users.Where(u => u.Id == usrid).FirstAsync();
                if (dbUser.CanAddAlbum.GetValueOrDefault(false))
                {
                    db.Albums.Add(album);
                    var rslt = await db.SaveChangesAsync();
                    //calc CanAddAlbum
                    if (rslt > 0)
                    {
                        if (User.IsInRole("FreeUser"))
                        {
                            bool canAddAlbum = false;
                            int nAlbums = await db.Albums.CountAsync(a => a.UserID == usrid);
                            string settings = await db.Settings.Where(s => s.SettingsID == "FreeUserAlbums").Select(s => s.Value).FirstAsync();
                            if (nAlbums < int.Parse(settings)) canAddAlbum = true;
                            dbUser.CanAddAlbum = canAddAlbum;
                            db.Entry(dbUser).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(album);
        }
        public async Task<ActionResult> Edit(int? id)
        {
            string usrid = User.Identity.GetUserId();
            ViewBag.UserID = usrid;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = await db.Albums.FindAsync(id);
            if (album == null)
            {
                return HttpNotFound();
            }

            if (album.UserID != usrid)
            {
                return HttpNotFound();
            }
            return View(album);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "AlbumID,Name,Description,UserID")] Album album)
        {
            album.AlbumSlug = album.GetAlbumSlug();
            if (ModelState.IsValid)
            {
                db.Entry(album).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(album);
        }
        public async Task<ActionResult> Delete(int? id)
        {
            string usrid = User.Identity.GetUserId();
            ViewBag.UserID = usrid;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = await db.Albums.FindAsync(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            if (album.UserID != usrid)
            {
                return HttpNotFound();
            }
            return View(album);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string usrid = User.Identity.GetUserId();
            ViewBag.UserID = usrid;
            Album album = await db.Albums.FindAsync(id);
            db.Albums.Remove(album);
            int rslt = await db.SaveChangesAsync();
            //calc CanAddAlbum
            if (rslt > 0)
            {
                if (User.IsInRole("FreeUser"))
                {
                    var dbUser = await db.Users.Where(u => u.Id == usrid).FirstAsync();
                    bool canAddAlbum = false;
                    int nAlbums = await db.Albums.CountAsync(a => a.UserID == usrid);
                    string settings = await db.Settings.Where(s => s.SettingsID == "FreeUserAlbums").Select(s => s.Value).FirstAsync();
                    if (nAlbums < int.Parse(settings)) canAddAlbum = true;
                    dbUser.CanAddAlbum = canAddAlbum;
                    db.Entry(dbUser).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            return View(id);
        }
        public async Task<ActionResult> AddPhoto(int? id)
        {
            string usrid = User.Identity.GetUserId();
            ViewBag.UserID = usrid;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = await db.Albums.FindAsync(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            if (album.UserID != usrid)
            {
                return HttpNotFound();
            }
            var PhotoList = await (from p in db.Photos
                                   join ap in db.AlbumsToPhotos on new { p.PhotoID, album.AlbumID } equals new { ap.PhotoID, ap.AlbumID } into t
                                   from a in t.DefaultIfEmpty()
                                   where (p.UserID == album.UserID) & (a == null)
                                   select new
                                   {
                                       p.PhotoID,
                                       p.Name,
                                       p.Image,
                                       Checked = false
                                   }).Distinct().ToListAsync();
            AlbumViewModel alb_vm = new AlbumViewModel();
            alb_vm.AlbumID = album.AlbumID;
            alb_vm.Name = album.Name;
            alb_vm.Description = album.Description;
            alb_vm.UserID = album.UserID;
            alb_vm.CanAddAlbum = album.User.CanAddAlbum.GetValueOrDefault(false);
            alb_vm.CanAddPhoto = album.User.CanAddPhoto.GetValueOrDefault(false);
            var MyCheckBoxList = new List<PhotoCheckBoxViewModel>();
            foreach (var item in PhotoList)
            {
                MyCheckBoxList.Add(new PhotoCheckBoxViewModel { Id = item.PhotoID, Name = item.Name, Image = item.Image, Checked = item.Checked });
            }
            alb_vm.Photos = MyCheckBoxList;
            return View(alb_vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoto(AlbumViewModel m)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in m.Photos)
                {
                    if (item.Checked)
                    {
                        db.AlbumsToPhotos.Add(new AlbumToPhoto() { AlbumID = m.AlbumID, PhotoID = item.Id });
                    }
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Open", new { id = m.AlbumID });
            }
            return View(m);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
