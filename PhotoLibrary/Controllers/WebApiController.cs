using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BLL;
using BOL;
using System.Threading.Tasks;

namespace PhotoLibrary.Controllers
{
    public class WebApiController : ApiController
    {
        private HomeBs objBs = new HomeBs();
        public async Task<List<PhotoIndexBs>> GetAll()
        {
            return await objBs.GetAllPhotos();
        }
    }
}
