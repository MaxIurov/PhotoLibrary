using BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DAL
{
    public class UserDb
    {
        private PhotoLibraryModel db;
        public UserDb()
        {
            db = new PhotoLibraryModel();
        }
        public async Task<IEnumerable<AspNetUser>> GetAll()
        {
            return await db.AspNetUsers.ToListAsync();
        }
        public async Task<AspNetUser> GetByID(string userId)
        {
            return await db.AspNetUsers.FindAsync(userId);
        }
    }
}
