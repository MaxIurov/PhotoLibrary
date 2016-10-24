using BOL3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class HomeBs:BaseBs
    {
        public async Task<List<AlbumNotEmptyBs>> GetNotEmptyAlbums()
        {
            IEnumerable<Album> alb = await albumBs.GetNotEmpty();
            List<AlbumNotEmptyBs> retAlb = new List<AlbumNotEmptyBs>();
            foreach (var i in alb)
            {
                AlbumNotEmptyBs ane = new AlbumNotEmptyBs();
                ane.AlbumID = i.AlbumID;
                ane.Name = i.Name;
                ane.Description = i.Description;
                ane.UserName = i.AspNetUser.UserName;
                ane.UserID = i.UserID;
                ane.NPhotos = i.AlbumToPhotoes.Count;
                retAlb.Add(ane);
            }
            return retAlb;
        }
    }
}
