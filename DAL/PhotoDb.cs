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
        public void Insert(Photo p)
        {
            db.Photos.Add(p);
            Save();
        }
        public async void Delete(int id)
        {
            Photo p = await db.Photos.FindAsync(id);
            Save();
        }
        public void Update(Photo p)
        {
            db.Entry(p).State = EntityState.Modified;
            Save();
        }
        public async void Save()
        {
            await db.SaveChangesAsync();
        }
    }
}
