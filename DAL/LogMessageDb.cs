using BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DAL
{
    public class LogMessageDb
    {
        private PhotoLibraryModel db;
        public LogMessageDb()
        {
            db = new PhotoLibraryModel();
        }
        public async Task Log(LogMessage message)
        {
            db.LogMessages.Add(message);
            await db.SaveChangesAsync();
        }
    }
}
