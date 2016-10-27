using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhotoLibrary.Models;
using BOL;

namespace PhotoLibrary.Helpers
{
    public static class ImageHelper
    {
        public static MvcHtmlString Image(this HtmlHelper html, BOL.Photo p, int size)
        {
            return Image(html, p.Name, p.Image, size);
        }
        public static MvcHtmlString Image(this HtmlHelper html, PhotoLibrary.Models.Photo p, int size)
        {
            return Image(html, p.Name, p.Image, size);
        }
        public static MvcHtmlString Image(this HtmlHelper html, string name, byte[] image, int size)
        {
            //size 0-3 -> bigger
            string mystyle;
            switch (size)
            {
                case 0:
                    mystyle = "max-width:50px;max-height:50px";
                    break;
                case 1:
                    mystyle = "max-width:110px;max-height:110px";
                    break;
                case 2:
                    mystyle = "max-width:200px;max-height:200px";
                    break;
                case 3:
                    mystyle = "max-width:360px;max-height:360px";
                    break;
                default:
                    mystyle = "max-width:50px;max-height:50px";
                    break;
            }
            var base64 = Convert.ToBase64String(image);
            var imgsrc = string.Format("data:image/jpg;base64,{0}", base64);
            TagBuilder img = new TagBuilder("img");
            img.MergeAttribute("src", imgsrc);
            img.MergeAttribute("alt", name);
            img.MergeAttribute("style", mystyle);
            return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
        }
    }
}