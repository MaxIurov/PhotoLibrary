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
        public async Task Insert(Album a)
        {
            await objDb.Insert(a);
        }
        public async Task Delete(int id)
        {
            await objDb.Delete(id);
        }
        public async Task Update(Album a)
        {
            await objDb.Update(a);
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
