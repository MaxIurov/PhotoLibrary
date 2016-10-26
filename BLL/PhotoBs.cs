using DAL;
using BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class PhotoBs
    {
        private PhotoDb objDb;
        public PhotoBs()
        {
            objDb = new PhotoDb();
        }
        public async Task<IEnumerable<Photo>> GetAll()
        {
            return await objDb.GetAll();
        }
        public async Task<Photo> GetByID(int id)
        {
            return await objDb.GetByID(id);
        }
        public async Task Insert(Photo photo)
        {
            await objDb.Insert(photo);
        }
        public async Task Delete(int id)
        {
            await objDb.Delete(id);
        }
        public async Task Update(Photo photo)
        {
            await objDb.Update(photo);
        }
        public async Task<IEnumerable<Photo>> GetByAlbumID(int id)
        {
            return await objDb.GetByAlbumID(id);
        }
    }
}
