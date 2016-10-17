using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoLibrary.Models
{
    public class LogMessage
    {
        public int LogMessageID { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}