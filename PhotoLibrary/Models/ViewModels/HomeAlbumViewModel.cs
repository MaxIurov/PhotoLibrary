﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoLibrary.Models.ViewModels
{
    public class HomeAlbumViewModel
    {
        public int AlbumID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public int NPhotos { get; set; }
    }
}