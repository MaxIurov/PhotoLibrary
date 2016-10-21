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
        public async void Save()
        {
            await db.SaveChangesAsync();
        }
    }
}
