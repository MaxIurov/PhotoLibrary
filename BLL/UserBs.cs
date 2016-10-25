using DAL;
using BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class UserBs
    {
        private UserDb objDb;
        public UserBs()
        {
            objDb = new UserDb();
        }
        public async Task<IEnumerable<AspNetUser>> GetAll()
        {
            return await objDb.GetAll();
        }
        public async Task<AspNetUser> GetByID(string id)
        {
            return await objDb.GetByID(id);
        }
    }
}
