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

using BLL;

namespace PhotoLibrary.Controllers
{
    public class HomeController : Controller
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        private HomeBs objBs = new HomeBs();
        public ActionResult PhotoList()
        {
            ViewBag.Message = "Web Api Photo List";
            return View();
        }
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = "Home page.";
            IEnumerable<AlbumNotEmptyBs> AlbumsWithPhotoes = await objBs.GetNotEmptyAlbums();
            return View(AlbumsWithPhotoes);
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
            BOL.Album album = await objBs.albumBs.GetByID(AlbumID.Value);
            if (album == null)
            {
                return HttpNotFound();
            }

            int pageSize = 5;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            ViewBag.CurrentSort = sortOrder;
            sortOrder = String.IsNullOrEmpty(sortOrder) ? "Name" : sortOrder;
            IEnumerable<BOL.Photo> Photos = await objBs.photoBs.GetByAlbumID(AlbumID.Value);
            IPagedList<BOL.Photo> pg_photo = null;
            switch (sortOrder)
            {
                case "Name":
                    if (sortOrder.Equals(CurrentSort))
                        pg_photo = Photos.OrderByDescending
                                (m => m.Name).ToPagedList(pageIndex, pageSize);
                    else
                        pg_photo = Photos.OrderBy
                                (m => m.Name).ToPagedList(pageIndex, pageSize);
                    break;
                case "Time":
                    if (sortOrder.Equals(CurrentSort))
                        pg_photo = Photos.OrderByDescending
                                (m => m.TimeTaken).ToPagedList(pageIndex, pageSize);
                    else
                        pg_photo = Photos.OrderBy
                                (m => m.TimeTaken).ToPagedList(pageIndex, pageSize);
                    break;
                case "Flash":
                    if (sortOrder.Equals(CurrentSort))
                        pg_photo = Photos.OrderByDescending
                                (m => m.Flash).ToPagedList(pageIndex, pageSize);
                    else
                        pg_photo = Photos.OrderBy
                                (m => m.Flash).ToPagedList(pageIndex, pageSize);
                    break;

                case "Default":
                    pg_photo = Photos.OrderBy
                            (m => m.Name).ToPagedList(pageIndex, pageSize);
                    break;
            }
            ViewBag.AlbumID = album.AlbumID;
            ViewBag.UserName = album.AspNetUser.UserName;
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
            int photoID = (PhotoID.HasValue) ? PhotoID.Value : 0;
            int albumID = (AlbumID.HasValue) ? AlbumID.Value : 0;
            PhotoViewBs photo = await objBs.GetPhoto(photoID, albumID, usrid);
            return View(photo);
        }
        public async Task<ActionResult> SlugAction(string slug)
        {
            if (String.IsNullOrEmpty(slug))
            {
                return RedirectToAction("Index");
            }
            BOL.Album album = await objBs.albumBs.GetBySlug(slug);
            if (album == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Message = "Album public page.";
            int pageSize = 5;
            int pageIndex = 1;
            pageIndex = 1;
            ViewBag.CurrentSort = "Name";
            IEnumerable<BOL.Photo> Photos = await objBs.photoBs.GetByAlbumID(album.AlbumID);
            IPagedList<BOL.Photo> pg_photo = null;
            pg_photo = Photos.OrderBy(p => p.Name).ToPagedList(pageIndex, pageSize);
            ViewBag.AlbumID = album.AlbumID;
            ViewBag.UserName = album.AspNetUser.UserName;
            ViewBag.AlbumName = album.Name;
            ViewBag.Description = album.Description;
            string directLink = string.Format("{0}://{1}{2}{3}{4}", Request.Url.Scheme,
                Request.Url.Authority, Url.Content("~"), @"Albs/", album.AlbumSlug);
            ViewBag.DirectLink = directLink;
            return View("Album", pg_photo);
        }
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageLike(PhotoViewBs m, string button_like, string button_dislike, string button_delete)
        {
            if (User.Identity.IsAuthenticated && m.CanLike)
            {
                string usrid = User.Identity.GetUserId();
                if (m.UserID != usrid)
                {
                    if (!String.IsNullOrEmpty(button_like))
                    {
                        await objBs.likePhotoBs.Like(usrid, m.PhotoID);
                        m.ModelLiked = true;
                        m.ModelDisliked = false;
                    }
                    if (!String.IsNullOrEmpty(button_dislike))
                    {
                        await objBs.likePhotoBs.Dislike(usrid, m.PhotoID);
                        m.ModelLiked = false;
                        m.ModelDisliked = true;
                    }
                    if (!String.IsNullOrEmpty(button_delete))
                    {
                        await objBs.likePhotoBs.Delete(usrid, m.PhotoID);
                        m.ModelLiked = false;
                        m.ModelDisliked = false;
                    }
                    m.NLikes = await objBs.likePhotoBs.LikeNumber(m.PhotoID);
                    m.NDislikes = await objBs.likePhotoBs.DislikeNumber(m.PhotoID);
                    return PartialView("ManageLike", m);
                }
            }
            return HttpNotFound();
        }
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}