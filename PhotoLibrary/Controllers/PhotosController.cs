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
            List<Photo> photos = await db.Photos.Where(u => u.User.Id == usrid).ToListAsync();
            PhotoIndexViewModel photoIndex = new PhotoIndexViewModel();
            photoIndex.UserID = usrid;
            var thisUser = await db.Users.Where(u => u.Id == usrid).SingleOrDefaultAsync();
            photoIndex.CanAddAlbum = thisUser.CanAddAlbum.GetValueOrDefault(false);
            photoIndex.CanAddPhoto = thisUser.CanAddPhoto.GetValueOrDefault(false);
            List<PhotoSmallViewModel> photoListItem = new List<PhotoSmallViewModel>();
            foreach (var item in photos)
            {
                int nLikes = await db.LikePhotos.Where(x => x.PhotoID == item.PhotoID).Where(x => x.Liked == 1).CountAsync();
                int nDislikes = await db.LikePhotos.Where(x => x.PhotoID == item.PhotoID).Where(x => x.Liked == -1).CountAsync();
                string l_d = nLikes.ToString() + "/" + nDislikes.ToString();
                photoListItem.Add(
                    new PhotoSmallViewModel
                    {
                        PhotoID = item.PhotoID,
                        Name = item.Name,
                        Image = item.Image,
                        Likes_Dislikes = l_d
                    }
                );
            }
            photoIndex.Photos = photoListItem;
            return View(photoIndex);
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
            Photo photo = new Photo() { UserID = usrid };
            return View(photo);
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
            PhotoViewModel editPhoto = new PhotoViewModel();
            editPhoto.PhotoID = photo.PhotoID;
            editPhoto.Name = photo.Name;
            editPhoto.Image = photo.Image;
            editPhoto.TimeTaken = photo.TimeTaken;
            editPhoto.TimeTakenStr = timeTaken;
            editPhoto.Location = photo.Location;
            editPhoto.DeviceID = photo.DeviceID;
            editPhoto.DeviceList = sl;
            editPhoto.Focus = photo.Focus;
            editPhoto.Aperture = photo.Aperture;
            editPhoto.Shutter = photo.Shutter;
            editPhoto.ISO = photo.ISO;
            editPhoto.Flash = photo.Flash;
            editPhoto.Albums = lAlbums;
            editPhoto.UserID = usrid;
            return View(editPhoto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PhotoViewModel editPhoto)
        {
            DateTime? modelTimeTaken = null;
            if (!string.IsNullOrWhiteSpace(editPhoto.TimeTakenStr))
            {
                DateTime timeTaken;
                if (DateTime.TryParseExact(editPhoto.TimeTakenStr, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out timeTaken))
                {
                    modelTimeTaken = timeTaken;
                }
                else
                {
                    ModelState.AddModelError("", "Wrong Time Taken.");
                }
            }
            Photo photo = new Photo();
            photo.PhotoID = editPhoto.PhotoID;
            photo.Name = editPhoto.Name;
            photo.Image = editPhoto.Image;
            photo.TimeTaken = modelTimeTaken;
            photo.Location = editPhoto.Location;
            photo.DeviceID = editPhoto.DeviceID;
            photo.Focus = editPhoto.Focus;
            photo.Aperture = editPhoto.Aperture;
            photo.Shutter = editPhoto.Shutter;
            photo.ISO = editPhoto.ISO;
            photo.Flash = editPhoto.Flash;
            photo.UserID = editPhoto.UserID;
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
                foreach (var item in editPhoto.Albums)
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
            editPhoto.DeviceList = new SelectList(devices, "DeviceID", "Name", "Empty");
            return View(editPhoto);
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
        public async Task<ActionResult> NameSearch(PhotoSearchModel model)
        {
            string usrid = "Guest";
            if (User.Identity.IsAuthenticated)
            {
                usrid = User.Identity.GetUserId();
            }
            List<Device> devices = await db.Devices.ToListAsync();
            SelectList sl = new SelectList(devices, "DeviceID", "Name", "Empty");
            model.DeviceList = sl;
            string searchName = "";
            if (!string.IsNullOrWhiteSpace(model.Name)) searchName = model.Name;
            SqlParameter param1 = new SqlParameter("@UserID", usrid);
            SqlParameter param2 = new SqlParameter("@SString", searchName);
            var results = await db.Photos.SqlQuery("[dbo].[sp_SearchPhoto] @UserID,@SString", param1, param2).ToListAsync();
            model.SearchResults = results;
            return View("PhotoSearch", model);
        }
        [AllowAnonymous]
        public async Task<ActionResult> AllSearch(PhotoSearchModel model)
        {
            DateTime? modelTimeFrom = null;
            if (!string.IsNullOrWhiteSpace(model.TimeFrom))
            {
                DateTime timeFrom;
                if (DateTime.TryParseExact(model.TimeFrom, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out timeFrom))
                {
                    modelTimeFrom = timeFrom;
                }
                else
                {
                    ModelState.AddModelError("", "Wrong TimeFrom.");
                }
            }
            DateTime? modelTimeTo = null;
            if (!string.IsNullOrWhiteSpace(model.TimeTo))
            {
                DateTime timeTo;
                if (DateTime.TryParseExact(model.TimeTo, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out timeTo))
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
            model.DeviceList = sl;
            string searchName = "";
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                searchName = model.Name;
            }
            List<Photo> searchResults = new List<Photo>();
            if (ModelState.IsValid)
            {
                searchResults = await (from p in db.Photos
                                       join ap in db.AlbumsToPhotos on p.PhotoID equals ap.PhotoID into pGroup
                                       from p_ap in pGroup.DefaultIfEmpty()
                                       where ((p.UserID == usrid) | (p_ap != null)) &
                                       (p.Name.Contains(searchName)) &
                                       ((p.TimeTaken >= modelTimeFrom) | (modelTimeFrom == null)) & 
                                       ((p.TimeTaken <= modelTimeTo) | (modelTimeTo == null)) &
                                       ((p.Location.Contains(model.Location)) | (model.Location == null) | (model.Location == "")) &
                                       ((p.DeviceID == model.DeviceID) | (model.DeviceID == 1)) &
                                       ((p.Focus == model.Focus) | (model.Focus == null)) &
                                       ((p.Aperture == model.Aperture) | (model.Aperture == null) | (model.Aperture == "")) &
                                       ((p.Shutter == model.Shutter) | (model.Shutter == null) | (model.Shutter == "")) &
                                       ((p.ISO == model.ISO) | (model.ISO == null) | (model.ISO == "")) &
                                       ((p.Flash == model.Flash) | (model.Flash == null))
                                       select p).Distinct().ToListAsync();
            }
            model.SearchResults = searchResults;
            return View("PhotoSearch", model);
        }
        [AllowAnonymous]
        public async Task<ActionResult> FastSearch(string photonametosearch)
        {
            string usrid = "Guest";
            if (User.Identity.IsAuthenticated)
            {
                usrid = User.Identity.GetUserId();
            }
            List<Photo> searchResults = new List<Photo>();
            if (!string.IsNullOrWhiteSpace(photonametosearch))
            {
                SqlParameter param1 = new SqlParameter("@UserID", usrid);
                SqlParameter param2 = new SqlParameter("@SString", photonametosearch);
                searchResults = await db.Photos.SqlQuery("[dbo].[sp_SearchPhoto] @UserID,@SString", param1, param2).ToListAsync();
            }
            return PartialView("FastSearch", searchResults);
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