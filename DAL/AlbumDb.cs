using BOL3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DAL
{
    public class AlbumDb
    {
        private PhotoLibraryModel db;
        public AlbumDb()
        {
            db = new PhotoLibraryModel();
        }
        public async Task<IEnumerable<Album>> GetAll()
        {
            return await db.Albums.ToListAsync();
        }
        public async Task<Album> GetByID(int id)
        {
            return await db.Albums.FindAsync(id);
        }
        public void Insert(Album a)
        {
            db.Albums.Add(a);
            Save();
        }
        public async void Delete(int id)
        {
            Album a = await db.Albums.FindAsync(id);
            db.Albums.Remove(a);
            Save();
        }
        public void Update(Album a)
        {
            db.Entry(a).State = EntityState.Modified;
            Save();
        }
        public async void Save()
        {
            await db.SaveChangesAsync();
        }
        public async Task<IEnumerable<Album>> GetNotEmpty()
        {
            return await (from a in db.Albums
                          join ap in db.AlbumToPhotoes on a.AlbumID equals ap.AlbumID
                          select a).Distinct().ToListAsync();
            //return await db.Albums.ToListAsync();
        }
    }
}
