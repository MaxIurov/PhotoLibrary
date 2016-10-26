using DAL;
using BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class LikePhotoBs
    {
        private LikePhotoDb objDb;
        public LikePhotoBs()
        {
            objDb = new LikePhotoDb();
        }
        public async Task Like(string userId,int photoId)
        {
            await objDb.Like(userId, photoId);
        }
        public async Task Dislike(string userId, int photoId)
        {
            await objDb.Dislike(userId, photoId);
        }
        public async Task Delete(string userId, int photoId)
        {
            await objDb.Delete(userId, photoId);
        }
        public async Task<int> LikeNumber(int photoId)
        {
            return await objDb.LikeNumber(photoId);
        }
        public async Task<int> DislikeNumber(int photoId)
        {
            return await objDb.DislikeNumber(photoId);
        }
    }
}
