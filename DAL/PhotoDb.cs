using BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DAL
{
    public class PhotoDb
    {
        private PhotoLibraryModel db;
        public PhotoDb()
        {
            db = new PhotoLibraryModel();
        }
        public async Task<IEnumerable<Photo>> GetAll()
        {
            return await db.Photos.ToListAsync();
        }
        public async Task<Photo> GetByID(int id)
        {
            return await db.Photos.FindAsync(id);
        }
        public async Task Insert(Photo photo)
        {
            db.Photos.Add(photo);
            await Save();
        }
        public async Task Delete(int id)
        {
            Photo p = await db.Photos.FindAsync(id);
            db.Photos.Remove(p);
            await Save();
        }
        public async Task Update(Photo photo)
        {
            db.Entry(photo).State = EntityState.Modified;
            await Save();
        }
        public async Task Save()
        {
            await db.SaveChangesAsync();
        }
        public async Task<IEnumerable<Photo>> GetByAlbumID(int id)
        {
            return await (from p in db.Photos
                          join ap in db.AlbumToPhotoes on p.PhotoID equals ap.PhotoID
                          where (ap.AlbumID == id)
                          select p).Distinct().ToListAsync();
        }
    }
}
