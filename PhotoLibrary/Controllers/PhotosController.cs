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
using PagedList;
using System.Data.SqlClient;
using PhotoLibrary.Common;
using System.Globalization;

namespace PhotoLibrary.Controllers
{
    [Authorize]
    public class PhotosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public async Task<ActionResult> Index()
        {
            string usrid = User.Identity.GetUserId();
            ViewBag.UserID = usrid;
            List<Photo> lp = await db.Photos.Where(u => u.User.Id == usrid).ToListAsync();
            PhotoIndexViewModel pivm = new PhotoIndexViewModel();
            pivm.UserID = usrid;
            var thisUser = await db.Users.Where(u => u.Id == usrid).SingleOrDefaultAsync();
            pivm.CanAddAlbum = thisUser.CanAddAlbum.GetValueOrDefault(false);
            pivm.CanAddPhoto = thisUser.CanAddPhoto.GetValueOrDefault(false);
            List<PhotoSmallViewModel> lpsvm = new List<PhotoSmallViewModel>();
            foreach (var item in lp)
            {
                int nLikes = await db.LikePhotos.Where(x => x.PhotoID == item.PhotoID).Where(x => x.Liked == 1).CountAsync();
                int nDislikes = await db.LikePhotos.Where(x => x.PhotoID == item.PhotoID).Where(x => x.Liked == -1).CountAsync();
                string l_d = nLikes.ToString() + "/" + nDislikes.ToString();
                PhotoSmallViewModel psvm = new PhotoSmallViewModel
                { PhotoID = item.PhotoID, Name = item.Name, Image = item.Image, Likes_Dislikes = l_d };
                lpsvm.Add(psvm);
            }
            pivm.Photos = lpsvm;
            return View(pivm);
        }
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }
        public ActionResult Create()
        {
            string usrid = User.Identity.GetUserId();
            ViewBag.UserID = usrid;
            Photo m = new Photo() { UserID = usrid };
            return View(m);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PhotoID,Name,UserID")] Photo photo, HttpPostedFileBase image1)
        {
            if (ChkImage(image1))
            {
                string usrid = User.Identity.GetUserId();
                var dbUser = await db.Users.Where(u => u.Id == usrid).FirstAsync();
                if (dbUser.CanAddPhoto.GetValueOrDefault(false))
                {
                    photo.Image = new byte[image1.ContentLength];
                    image1.InputStream.Read(photo.Image, 0, image1.ContentLength);
                    Device dev = await db.Devices.FirstOrDefaultAsync(x => x.Name == "Empty");
                    int devID = 1;
                    if (dev != null) devID = dev.DeviceID;
                    photo.DeviceID = devID;
                    if (ModelState.IsValid)
                    {
                        db.Photos.Add(photo);
                        var rslt = await db.SaveChangesAsync();
                        //calc CanAddPhoto
                        if (rslt > 0)
                        {
                            if (User.IsInRole("FreeUser"))
                            {
                                bool canAddPhoto = false;
                                int nPhotos = await db.Photos.CountAsync(p => p.UserID == usrid);
                                string settings = await db.Settings.Where(s => s.SettingsID == "FreeUserPhotos").Select(s => s.Value).FirstAsync();
                                if (nPhotos < int.Parse(settings)) canAddPhoto = true;
                                dbUser.CanAddPhoto = canAddPhoto;
                                db.Entry(dbUser).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            ModelState.AddModelError("", "Something wrong with the file.");
            return View(photo);
        }
        private bool ChkImage(HttpPostedFileBase f)
        {
            if (f != null)
            {
                if ((f.ContentType.ToLower() == "image/jpeg") && (f.ContentLength > 0) &&
                    (f.ContentLength <= 500 * 1024) && (f.FileName.ToLower().EndsWith(".jpg")))
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<ActionResult> Edit(int? id)
        {
            string usrid = User.Identity.GetUserId();
            ViewBag.UserID = usrid;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }

            if (photo.UserID != usrid)
            {
                return HttpNotFound();
            }
            List<Device> devices = await db.Devices.ToListAsync();
            SelectList sl = new SelectList(devices, "DeviceID", "Name", "Empty");
            List<CheckBoxViewModel> lAlbums = new List<CheckBoxViewModel>();
            var AlbumList = await (from a in db.Albums
                                   join ap in db.AlbumsToPhotos on new { a.AlbumID, photo.PhotoID } equals new { ap.AlbumID, ap.PhotoID } into t
                                   from p in t.DefaultIfEmpty()
                                   where (a.UserID == usrid)
                                   select new
                                   {
                                       a.AlbumID,
                                       a.Name,
                                       Checked = (p.PhotoID == photo.PhotoID)
                                   }).Distinct().ToListAsync();
            foreach (var item in AlbumList)
            {
                lAlbums.Add(new CheckBoxViewModel { Id = item.AlbumID, Name = item.Name, Checked = item.Checked });
            }
            string timeTaken = "";
            if (photo.TimeTaken.HasValue)
            {
                DateTime time = photo.TimeTaken.Value;
                timeTaken = time.ToString("dd.MM.yyyy HH:mm");
            }
            PhotoViewModel pvm = new PhotoViewModel();
            pvm.PhotoID = photo.PhotoID;
            pvm.Name = photo.Name;
            pvm.Image = photo.Image;
            pvm.TimeTaken = photo.TimeTaken;
            pvm.TimeTakenStr = timeTaken;
            pvm.Location = photo.Location;
            pvm.DeviceID = photo.DeviceID;
            pvm.DeviceList = sl;
            pvm.Focus = photo.Focus;
            pvm.Aperture = photo.Aperture;
            pvm.Shutter = photo.Shutter;
            pvm.ISO = photo.ISO;
            pvm.Flash = photo.Flash;
            pvm.Albums = lAlbums;
            pvm.UserID = usrid;
            return View(pvm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PhotoViewModel photovm)
        {
            DateTime? modelTimeTaken = null;
            if (photovm.TimeTakenStr != "")
            {
                DateTime timeTaken;
                if (DateTime.TryParseExact(photovm.TimeTakenStr, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out timeTaken))
                {
                    modelTimeTaken = timeTaken;
                }
                else
                {
                    ModelState.AddModelError("", "Wrong Time Taken.");
                }
            }
            Photo photo = new Photo();
            photo.PhotoID = photovm.PhotoID;
            photo.Name = photovm.Name;
            photo.Image = photovm.Image;
            photo.TimeTaken = modelTimeTaken;
            photo.Location = photovm.Location;
            photo.DeviceID = photovm.DeviceID;
            photo.Focus = photovm.Focus;
            photo.Aperture = photovm.Aperture;
            photo.Shutter = photovm.Shutter;
            photo.ISO = photovm.ISO;
            photo.Flash = photovm.Flash;
            photo.UserID = photovm.UserID;
            if (ModelState.IsValid)
            {
                db.Entry(photo).State = EntityState.Modified;
                foreach (var item in db.AlbumsToPhotos)
                {
                    if (item.PhotoID == photo.PhotoID)
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }
                }
                foreach (var item in photovm.Albums)
                {
                    if (item.Checked)
                    {
                        db.AlbumsToPhotos.Add(new AlbumToPhoto() { AlbumID = item.Id, PhotoID = photo.PhotoID });
                    }
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            List<Device> devices = await db.Devices.ToListAsync();
            photovm.DeviceList = new SelectList(devices, "DeviceID", "Name", "Empty");
            return View(photovm);
        }
        public async Task<ActionResult> Delete(int? id)
        {
            string usrid = User.Identity.GetUserId();
            ViewBag.UserID = usrid;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            if (photo.UserID != usrid)
            {
                return HttpNotFound();
            }
            return View(photo);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string usrid = User.Identity.GetUserId();
            ViewBag.UserID = usrid;
            Photo photo = await db.Photos.FindAsync(id);
            db.Photos.Remove(photo);
            int rslt = await db.SaveChangesAsync();
            //calc CanAddPhoto
            if (rslt > 0)
            {
                if (User.IsInRole("FreeUser"))
                {
                    var dbUser = await db.Users.Where(u => u.Id == usrid).FirstAsync();
                    bool canAddPhoto = false;
                    int nAlbums = await db.Photos.CountAsync(a => a.UserID == usrid);
                    string settings = await db.Settings.Where(s => s.SettingsID == "FreeUserPhotos").Select(s => s.Value).FirstAsync();
                    if (nAlbums < int.Parse(settings)) canAddPhoto = true;
                    dbUser.CanAddPhoto = canAddPhoto;
                    db.Entry(dbUser).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            return View(id);
        }
        [AllowAnonymous]
        public async Task<ActionResult> PhotoSearch()
        {
            string usrid = "Guest";
            if (User.Identity.IsAuthenticated)
            {
                usrid = User.Identity.GetUserId();
            }
            List<Device> devices = await db.Devices.ToListAsync();
            Device dev = await db.Devices.FirstOrDefaultAsync(x => x.Name == "Empty");
            int devID = 1;
            if (dev != null) devID = dev.DeviceID;
            SelectList sl = new SelectList(devices, "DeviceID", "Name", "Empty");
            PhotoSearchModel model = new PhotoSearchModel();
            model.DeviceList = sl;
            model.DeviceID = devID;
            return View(model);
        }
        [AllowAnonymous]
        public async Task<ActionResult> NameSearch(PhotoSearchModel m)
        {
            string usrid = "Guest";
            if (User.Identity.IsAuthenticated)
            {
                usrid = User.Identity.GetUserId();
            }
            List<Device> devices = await db.Devices.ToListAsync();
            SelectList sl = new SelectList(devices, "DeviceID", "Name", "Empty");
            m.DeviceList = sl;
            string searchName = "";
            if (!string.IsNullOrWhiteSpace(m.Name)) searchName = m.Name;
            SqlParameter param1 = new SqlParameter("@UserID", usrid);
            SqlParameter param2 = new SqlParameter("@SString", searchName);
            var results = await db.Photos.SqlQuery("[dbo].[sp_SearchPhoto] @UserID,@SString", param1, param2).ToListAsync();
            m.SearchResults = results;
            return View("PhotoSearch", m);
        }
        [AllowAnonymous]
        public async Task<ActionResult> AllSearch(PhotoSearchModel m)
        {
            DateTime? modelTimeFrom = null;
            if (m.TimeFrom != "")
            {
                DateTime timeFrom;
                if (DateTime.TryParseExact(m.TimeFrom, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out timeFrom))
                {
                    modelTimeFrom = timeFrom;
                }
                else
                {
                    ModelState.AddModelError("", "Wrong TimeFrom.");
                }
            }
            DateTime? modelTimeTo = null;
            if (m.TimeTo != "")
            {
                DateTime timeTo;
                if (DateTime.TryParseExact(m.TimeTo, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out timeTo))
                {
                    modelTimeTo = timeTo;
                }
                else
                {
                    ModelState.AddModelError("", "Wrong TimeTo.");
                }
            }
            string usrid = "Guest";
            if (User.Identity.IsAuthenticated)
            {
                usrid = User.Identity.GetUserId();
            }
            List<Device> devices = await db.Devices.ToListAsync();
            SelectList sl = new SelectList(devices, "DeviceID", "Name", "Empty");
            m.DeviceList = sl;
            string searchName = "";
            if (!string.IsNullOrWhiteSpace(m.Name)) searchName = m.Name;
            List<Photo> results = await (from p in db.Photos
                                         join ap in db.AlbumsToPhotos on p.PhotoID equals ap.PhotoID into pGroup
                                         from p_ap in pGroup.DefaultIfEmpty()
                                         where ((p.UserID == usrid) | (p_ap != null)) &
                                         (p.Name.Contains(searchName)) &
                                         ((p.TimeTaken >= modelTimeFrom) | (modelTimeFrom == null)) & ((p.TimeTaken <= modelTimeTo) | (modelTimeTo == null)) &
                                         ((p.Location.Contains(m.Location)) | (m.Location == null) | (m.Location == "")) &
                                         ((p.DeviceID == m.DeviceID) | (m.DeviceID == 1)) &
                                         ((p.Focus == m.Focus) | (m.Focus == null)) &
                                         ((p.Aperture == m.Aperture) | (m.Aperture == null) | (m.Aperture == "")) &
                                         ((p.Shutter == m.Shutter) | (m.Shutter == null) | (m.Shutter == "")) &
                                         ((p.ISO == m.ISO) | (m.ISO == null) | (m.ISO == "")) &
                                         ((p.Flash == m.Flash) | (m.Flash == null))
                                         select p).Distinct().ToListAsync();
            m.SearchResults = results;
            return View("PhotoSearch", m);
        }
        [AllowAnonymous]
        public async Task<ActionResult> FastSearch(string photonametosearch)
        {
            string usrid = "Guest";
            if (User.Identity.IsAuthenticated)
            {
                usrid = User.Identity.GetUserId();
            }
            List<Photo> foundphotos = new List<Photo>();
            if (!string.IsNullOrWhiteSpace(photonametosearch))
            {
                SqlParameter param1 = new SqlParameter("@UserID", usrid);
                SqlParameter param2 = new SqlParameter("@SString", photonametosearch);
                foundphotos = await db.Photos.SqlQuery("[dbo].[sp_SearchPhoto] @UserID,@SString", param1, param2).ToListAsync();
            }
            return PartialView("FastSearch", foundphotos);
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
