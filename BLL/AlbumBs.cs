using DAL;
using BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AlbumBs
    {
        private AlbumDb objDb;
        public AlbumBs()
        {
            objDb = new AlbumDb();
        }
        public async Task<IEnumerable<Album>> GetAll()
        {
            return await objDb.GetAll();
        }
        public async Task<Album> GetByID(int id)
        {
            return await objDb.GetByID(id);
        }
        public void Insert(Album a)
        {
            objDb.Insert(a);
        }
        public void Delete(int id)
        {
            objDb.Delete(id);
        }
        public void Update(Album a)
        {
            objDb.Update(a);
        }
        public async Task<IEnumerable<Album>> GetNotEmpty()
        {
            return await objDb.GetNotEmpty();
        }
        public async Task<Album> GetBySlug(string slug)
        {
            return await objDb.GetBySlug(slug);
        }
    }
}
