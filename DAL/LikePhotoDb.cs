using BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DAL
{
    public class LikePhotoDb
    {
        private PhotoLibraryModel db;
        public LikePhotoDb()
        {
            db = new PhotoLibraryModel();
        }
        public async void Like(string userID,int photoID)
        {
            var currentLike = await db.LikePhotoes.Where(x => x.UserID == userID).Where(x => x.PhotoID == photoID).ToListAsync();
            db.LikePhotoes.RemoveRange(currentLike);
            LikePhoto newLike = new LikePhoto();
            newLike.UserID = userID;
            newLike.PhotoID = photoID;
            newLike.Liked = 1;
            db.LikePhotoes.Add(newLike);
            await db.SaveChangesAsync();
        }
        public async void Dislike(string userID, int photoID)
        {
            var currentLike = await db.LikePhotoes.Where(x => x.UserID == userID).Where(x => x.PhotoID == photoID).ToListAsync();
            db.LikePhotoes.RemoveRange(currentLike);
            LikePhoto newLike = new LikePhoto();
            newLike.UserID = userID;
            newLike.PhotoID = photoID;
            newLike.Liked = -1;
            db.LikePhotoes.Add(newLike);
            await db.SaveChangesAsync();
        }
        public async void Delete(string userID, int photoID)
        {
            var currentLike = await db.LikePhotoes.Where(x => x.UserID == userID).Where(x => x.PhotoID == photoID).ToListAsync();
            db.LikePhotoes.RemoveRange(currentLike);
            await db.SaveChangesAsync();
        }
    }
}
