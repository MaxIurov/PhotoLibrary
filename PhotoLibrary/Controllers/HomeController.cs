using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using PhotoLibrary.Models;
using System.Net;
using System.Data;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using PhotoLibrary.Common;
using PhotoLibrary.Models.ViewModels;
using PagedList;

namespace PhotoLibrary.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = "Home page.";
            var alb = await (from a in db.Albums
                             join ap in db.AlbumsToPhotos on a.AlbumID equals ap.AlbumID
                             group ap by new { a.AlbumID, a.Name, a.Description, a.UserID, a.User.UserName } into g
                             select new
                             {
                                 AlbumID = g.Key.AlbumID,
                                 Name = g.Key.Name,
                                 Description = g.Key.Description,
                                 UserID = g.Key.UserID,
                                 UserName = g.Key.UserName,
                                 NPhotos = g.Count()
                             }).ToListAsync();
            List<HomeAlbumViewModel> lhavm = new List<HomeAlbumViewModel>();
            foreach (var item in alb)
            {
                lhavm.Add(new HomeAlbumViewModel
                {
                    AlbumID = item.AlbumID,
                    Name = item.Name,
                    Description = item.Description,
                    UserID = item.UserID,
                    UserName = item.UserName,
                    NPhotos = item.NPhotos
                });
            }
            return View(lhavm);
        }
        public ActionResult About()
        {
            ViewBag.Message = "My application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "My contact page.";
            return View();
        }
        public async Task<ActionResult> Album(int? AlbumID, string sortOrder, string CurrentSort, int? page)
        {
            ViewBag.Message = "Album public page.";
            if (AlbumID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = await db.Albums.FindAsync(AlbumID);
            if (album == null)
            {
                return HttpNotFound();
            }

            int pageSize = 5;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            ViewBag.CurrentSort = sortOrder;

            sortOrder = String.IsNullOrEmpty(sortOrder) ? "Name" : sortOrder;

            var PhotoList = await (from p in db.Photos
                                   join ap in db.AlbumsToPhotos on p.PhotoID equals ap.PhotoID
                                   where (ap.AlbumID == album.AlbumID)
                                   select p).Distinct().ToListAsync();
            List<Photo> lp = new List<Photo>(PhotoList);
            IPagedList<Photo> pg_photo = null;
            switch (sortOrder)
            {
                case "Name":
                    if (sortOrder.Equals(CurrentSort))
                        pg_photo = lp.OrderByDescending
                                (m => m.Name).ToPagedList(pageIndex, pageSize);
                    else
                        pg_photo = lp.OrderBy
                                (m => m.Name).ToPagedList(pageIndex, pageSize);
                    break;
                case "Time":
                    if (sortOrder.Equals(CurrentSort))
                        pg_photo = lp.OrderByDescending
                                (m => m.TimeTaken).ToPagedList(pageIndex, pageSize);
                    else
                        pg_photo = lp.OrderBy
                                (m => m.TimeTaken).ToPagedList(pageIndex, pageSize);
                    break;
                case "Flash":
                    if (sortOrder.Equals(CurrentSort))
                        pg_photo = lp.OrderByDescending
                                (m => m.Flash).ToPagedList(pageIndex, pageSize);
                    else
                        pg_photo = lp.OrderBy
                                (m => m.Flash).ToPagedList(pageIndex, pageSize);
                    break;

                case "Default":
                    pg_photo = lp.OrderBy
                            (m => m.Name).ToPagedList(pageIndex, pageSize);
                    break;
            }
            ViewBag.AlbumID = album.AlbumID;
            ViewBag.UserName = album.User.UserName;
            ViewBag.AlbumName = album.Name;
            ViewBag.Description = album.Description;
            string directLink = string.Format("{0}://{1}{2}{3}{4}", Request.Url.Scheme,
                Request.Url.Authority, Url.Content("~"), @"Albs/", album.AlbumSlug);
            ViewBag.DirectLink = directLink;
            return View(pg_photo);
        }
        public async Task<ActionResult> ViewPhoto(int? PhotoID, int? AlbumID)
        {
            ViewBag.Message = "Photo public page.";
            string usrid = "Guest";
            if (User.Identity.IsAuthenticated)
            {
                usrid = User.Identity.GetUserId();
            }
            ViewBag.UserID = usrid;
            if (PhotoID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photos.FindAsync(PhotoID);
            if (photo == null)
            {
                return HttpNotFound();
            }
            if (AlbumID == null)
            {
                AlbumID = await (from p in db.Photos
                                 join ap in db.AlbumsToPhotos on p.PhotoID equals ap.PhotoID
                                 select ap.AlbumID).FirstAsync();
            }
            int backToAlbum = AlbumID.GetValueOrDefault(0);
            int liked = 0;
            if (usrid != "Guest")
            {
                liked = await db.LikePhotos.Where(x => x.UserID == usrid).Where(x => x.PhotoID == photo.PhotoID).
                    Select(x => x.Liked).FirstOrDefaultAsync();
            }
            int nLikes = await db.LikePhotos.Where(x => x.PhotoID == photo.PhotoID).Where(x => x.Liked == 1).CountAsync();
            int nDislikes = await db.LikePhotos.Where(x => x.PhotoID == photo.PhotoID).Where(x => x.Liked == -1).CountAsync();
            ViewPhotoViewModel vp = new ViewPhotoViewModel();
            vp.PhotoID = photo.PhotoID;
            vp.UserName = photo.User.UserName;
            vp.Name = photo.Name;
            vp.Image = photo.Image;
            vp.TimeTaken = photo.TimeTaken;
            vp.Location = photo.Location;
            vp.Device = photo.Device.Name;
            vp.Focus = photo.Focus;
            vp.Aperture = photo.Aperture;
            vp.Shutter = photo.Shutter;
            vp.ISO = photo.ISO;
            vp.Flash = photo.Flash;
            vp.UserID = photo.UserID;
            vp.BackToAlbumID = backToAlbum;
            vp.NLikes = nLikes;
            vp.NDislikes = nDislikes;
            vp.CanLike = ((usrid != photo.UserID) && (usrid != "Guest"));
            vp.ModelLiked = liked == 1;
            vp.ModelDisliked = liked == -1;
            return View(vp);
        }
        public async Task<ActionResult> SlugAction(string slug)
        {
            if (String.IsNullOrEmpty(slug))
            {
                return RedirectToAction("Index");
            }
            Album album = await db.Albums.Where(a => a.AlbumSlug == slug).FirstOrDefaultAsync();
            if (album == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Message = "Album public page.";
            int pageSize = 5;
            int pageIndex = 1;
            pageIndex = 1;
            ViewBag.CurrentSort = "Name";
            var PhotoList = await (from p in db.Photos
                                   join ap in db.AlbumsToPhotos on p.PhotoID equals ap.PhotoID
                                   where (ap.AlbumID == album.AlbumID)
                                   select p).Distinct().ToListAsync();
            List<Photo> lp = new List<Photo>(PhotoList);
            IPagedList<Photo> pg_photo = null;
            pg_photo = lp.OrderBy(p => p.Name).ToPagedList(pageIndex, pageSize);
            ViewBag.AlbumID = album.AlbumID;
            ViewBag.UserName = album.User.UserName;
            ViewBag.AlbumName = album.Name;
            ViewBag.Description = album.Description;
            string directLink = string.Format("{0}://{1}{2}{3}{4}", Request.Url.Scheme,
                Request.Url.Authority, Url.Content("~"), @"Albs/", album.AlbumSlug);
            ViewBag.DirectLink = directLink;
            return View("Album", pg_photo);
        }
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageLike(ViewPhotoViewModel m, string button_like, string button_dislike, string button_delete)
        {
            if (User.Identity.IsAuthenticated && m.CanLike)
            {
                string usrid = User.Identity.GetUserId();
                if (m.UserID != usrid)
                {
                    if (!String.IsNullOrEmpty(button_like))
                    {
                        var llp = await db.LikePhotos.Where(x => x.UserID == usrid).Where(x => x.PhotoID == m.PhotoID).ToListAsync();
                        db.LikePhotos.RemoveRange(llp);
                        LikePhoto lp = new LikePhoto();
                        lp.PhotoID = m.PhotoID;
                        lp.UserID = usrid;
                        lp.Liked = 1;
                        db.LikePhotos.Add(lp);
                        await db.SaveChangesAsync();
                        m.ModelLiked = true;
                        m.ModelDisliked = false;
                    }
                    if (!String.IsNullOrEmpty(button_dislike))
                    {
                        var llp = await db.LikePhotos.Where(x => x.UserID == usrid).Where(x => x.PhotoID == m.PhotoID).ToListAsync();
                        db.LikePhotos.RemoveRange(llp);
                        LikePhoto lp = new LikePhoto();
                        lp.PhotoID = m.PhotoID;
                        lp.UserID = usrid;
                        lp.Liked = -1;
                        db.LikePhotos.Add(lp);
                        await db.SaveChangesAsync();
                        m.ModelLiked = false;
                        m.ModelDisliked = true;
                    }
                    if (!String.IsNullOrEmpty(button_delete))
                    {
                        var llp = await db.LikePhotos.Where(x => x.UserID == usrid).Where(x => x.PhotoID == m.PhotoID).ToListAsync();
                        db.LikePhotos.RemoveRange(llp);
                        await db.SaveChangesAsync();
                        m.ModelLiked = false;
                        m.ModelDisliked = false;
                    }
                    int nLikes = await db.LikePhotos.Where(x => x.PhotoID == m.PhotoID).Where(x => x.Liked == 1).CountAsync();
                    int nDislikes = await db.LikePhotos.Where(x => x.PhotoID == m.PhotoID).Where(x => x.Liked == -1).CountAsync();
                    m.NLikes = nLikes;
                    m.NDislikes = nDislikes;
                    return PartialView("ManageLike", m);
                }
            }
            return HttpNotFound();
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