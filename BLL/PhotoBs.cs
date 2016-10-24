using DAL;
using BOL3;
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
        public void Insert(Photo p)
        {
            objDb.Insert(p);
        }
        public void Delete(int id)
        {
            objDb.Delete(id);
        }
        public void Update(Photo p)
        {
            objDb.Update(p);
        }
    }
}
