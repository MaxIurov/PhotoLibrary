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
        public async Task Like(string userID,int photoID)
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
        public async Task Dislike(string userID, int photoID)
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
        public async Task Delete(string userID, int photoID)
        {
            var currentLike = await db.LikePhotoes.Where(x => x.UserID == userID).Where(x => x.PhotoID == photoID).ToListAsync();
            db.LikePhotoes.RemoveRange(currentLike);
            await db.SaveChangesAsync();
        }
        public async Task<int> LikeNumber(int photoID)
        {
            return await db.LikePhotoes.Where(x => x.PhotoID == photoID).Where(x => x.Liked == 1).CountAsync();
        }
        public async Task<int> DislikeNumber(int photoID)
        {
            return await db.LikePhotoes.Where(x => x.PhotoID == photoID).Where(x => x.Liked == -1).CountAsync();
        }
    }
}
