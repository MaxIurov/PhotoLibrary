using BOL;
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
        public async Task Insert(Album album)
        {
            db.Albums.Add(album);
            await Save();
        }
        public async Task Delete(int id)
        {
            Album album = await db.Albums.FindAsync(id);
            db.Albums.Remove(album);
            await Save();
        }
        public async Task Update(Album album)
        {
            db.Entry(album).State = EntityState.Modified;
            await Save();
        }
        public async Task Save()
        {
            await db.SaveChangesAsync();
        }
        public async Task<IEnumerable<Album>> GetNotEmpty()
        {
            return await (from a in db.Albums
                          join ap in db.AlbumToPhotoes on a.AlbumID equals ap.AlbumID
                          select a).Distinct().ToListAsync();
        }
        public async Task<Album> GetBySlug(string slug)
        {
            return await db.Albums.Where(a => a.AlbumSlug == slug).FirstOrDefaultAsync();
        }
    }
}
