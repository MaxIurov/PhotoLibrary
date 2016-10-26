using BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class HomeBs:BaseBs
    {
        public async Task<List<AlbumNotEmptyBs>> GetNotEmptyAlbums()
        {
            IEnumerable<Album> Albums = await albumBs.GetNotEmpty();
            List<AlbumNotEmptyBs> NotEmptyAlbums = new List<AlbumNotEmptyBs>();
            foreach (var i in Albums)
            {
                AlbumNotEmptyBs NotEmptyAlbum = new AlbumNotEmptyBs();
                NotEmptyAlbum.AlbumID = i.AlbumID;
                NotEmptyAlbum.Name = i.Name;
                NotEmptyAlbum.Description = i.Description;
                NotEmptyAlbum.UserName = i.AspNetUser.UserName;
                NotEmptyAlbum.UserID = i.UserID;
                NotEmptyAlbum.NPhotos = i.AlbumToPhotoes.Count;
                NotEmptyAlbums.Add(NotEmptyAlbum);
            }
            return NotEmptyAlbums;
        }
        public async Task<PhotoViewBs> GetPhoto(int photoId,int albumId,string userId)
        {
            PhotoViewBs photoView = new PhotoViewBs();
            if (photoId != 0)
            {
                Photo photo = await photoBs.GetByID(photoId);
                if (userId == "") userId = "Guest";
                string photoCreator = photo.AspNetUser.UserName;
                if (albumId == 0)
                {
                    var album = photo.AlbumToPhotoes.First();
                    albumId = (album != null) ? album.AlbumID : 0;
                }
                int nLikes = photo.LikePhotoes.Where(x => x.Liked == 1).Count();
                int nDislikes = photo.LikePhotoes.Where(x => x.Liked == -1).Count();
                int liked = 0;
                if (userId != "Guest")
                {
                    liked = photo.LikePhotoes.Where(x => x.UserID == userId).Select(x => x.Liked).FirstOrDefault();
                }
                photoView.PhotoID = photoId;
                photoView.UserName = photoCreator;
                photoView.Name = photo.Name;
                photoView.Image = photo.Image;
                photoView.TimeTaken = photo.TimeTaken;
                photoView.Location = photo.Location;
                photoView.Device = photo.Device.Name;
                photoView.Focus = photo.Focus;
                photoView.Aperture = photo.Aperture;
                photoView.Shutter = photo.Shutter;
                photoView.ISO = photo.ISO;
                photoView.Flash = photo.Flash;
                photoView.UserID = photo.UserID;
                photoView.BackToAlbumID = albumId;
                photoView.NLikes = nLikes;
                photoView.NDislikes = nDislikes;
                photoView.CanLike = ((userId != photo.UserID) && (userId != "Guest"));
                photoView.ModelLiked = (liked == 1);
                photoView.ModelDisliked = (liked == -1);
            }
            return photoView;
        }
        public async Task<List<PhotoIndexBs>> GetAllPhotos()
        {
            IEnumerable<Photo> Photos = await photoBs.GetAll();
            List<PhotoIndexBs> AllPhotos = new List<PhotoIndexBs>();
            foreach (var i in Photos)
            {
                PhotoIndexBs Photo = new PhotoIndexBs();
                Photo.PhotoID = i.PhotoID;
                Photo.UserName = i.AspNetUser.UserName;
                Photo.Name = i.Name;
                Photo.Image = i.Image;
                Photo.TimeTaken = i.TimeTaken;
                Photo.Location = i.Location;
                Photo.Device = i.Device.Name;
                Photo.Focus = i.Focus;
                Photo.Aperture = i.Aperture;
                Photo.Shutter = i.Shutter;
                Photo.ISO = i.ISO;
                Photo.Flash = i.Flash;
                Photo.UserID = i.UserID;
                Photo.NLikes = i.LikePhotoes.Where(x => x.Liked == 1).Count();
                Photo.NDislikes = i.LikePhotoes.Where(x => x.Liked == -1).Count();
                AllPhotos.Add(Photo);
            }
            return AllPhotos;
        }
    }
}
