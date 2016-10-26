using DAL;
using BOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class LogMessageBs
    {
        private LogMessageDb objDb;
        public LogMessageBs()
        {
            objDb = new LogMessageDb();
        }
        public async Task Log(LogMessage message)
        {
            await objDb.Log(message);
        }
    }
}
