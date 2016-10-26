using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BaseBs
    {
        public PhotoBs photoBs { get; set; }
        public AlbumBs albumBs { get; set; }
        public UserBs userBs { get; set; }
        public LikePhotoBs likePhotoBs { get; set; }
        public LogMessageBs logMessageBs { get; set; }
        public BaseBs()
        {
            photoBs = new PhotoBs();
            albumBs = new AlbumBs();
            userBs = new UserBs();
            likePhotoBs = new LikePhotoBs();
            logMessageBs = new LogMessageBs();
        }
    }
}
