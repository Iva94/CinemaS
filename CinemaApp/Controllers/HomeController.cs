using CinemaApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace CinemaApp.Controllers
{
    public class HomeController : Controller
    {
        IEnumerable<string> Images { get; set; }
        private cinemaDatabaseEntities db = new cinemaDatabaseEntities();
        public ActionResult Index()
        {
            ViewBag.Images = Directory.EnumerateFiles(Server.MapPath("~/HomeImages")).Select(fn => "~/HomeImages/" + Path.GetFileName(fn));

            IList<Movie> movies = new List<Movie>(db.Movies.Where(m => m.IsAnnouncement == false));
            ViewData["movies"] = movies;

            IList<Movie> announcements = new List<Movie>(db.Movies.Where(m => m.IsAnnouncement == true));
            ViewData["announcements"] = announcements;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UploadPhotosSlideShow(HttpPostedFileBase ImageFile)
        {
            if (ImageFile != null)
            {
                string fileName = Path.GetFileName(ImageFile.FileName);
                string path = Path.Combine(Server.MapPath("~/HomeImages"), fileName);
                ImageFile.SaveAs(path);
            }
            return RedirectToAction("Index");
        }

        public ActionResult RemovePhotoFromSlideShow(string image)
        {
            if(image != null)
            {
                string path = Server.MapPath(image);
                
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    ViewBag.DeleteMsg = "True";
                }
                else
                {
                    ViewBag.DeleteMsg = "False";
                }
            }

            return RedirectToAction("Index");
        }
    }
}