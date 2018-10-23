using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CinemaApp.Models;

namespace CinemaApp.Controllers
{
    public class MoviesController : Controller
    {
        private cinemaDatabaseEntities db = new cinemaDatabaseEntities();
        CultureInfo culture = new CultureInfo("bs-Latn", false);

        // GET: Movies
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View(db.Movies.ToList());
        }
        
        // GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.MovieProjectionDate = new SelectList(db.Projections.Where(p => p.MovieId == id), "Date", "Date");
            ViewBag.MovieProjectionTime = new SelectList(db.Projections.Where(p => p.MovieId == id), "Time", "Time");

            IList<Projection> projections = new List<Projection>(db.Projections.Where(p => p.MovieId == id).OrderBy(p => p.DateTime));
            IList<String> movieProjectionDates = new List<String>();
            foreach(var item in projections)
            {
                movieProjectionDates.Add(item.DateTime.Value.ToString("dd. MMMM", culture));
            }
            for (int i = 0; i < movieProjectionDates.Count(); i++)
            {
                for (int j = i + 1; j < movieProjectionDates.Count(); j++)
                {
                    if (movieProjectionDates[i] == movieProjectionDates[j])
                    {
                        movieProjectionDates.Remove(movieProjectionDates[j]);
                        j--;
                    }
                }
            }

            ViewData["MovieProjectionDates"] = movieProjectionDates;
            Movie movie = db.Movies.Find(id);
            ViewBag.Culture = culture;
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            ViewBag.Genre = new SelectList(db.Genres, "GenreId", "GenreName");
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MovieId,MovieTitle,OriginalTitle,PremiereDate,Director,Actors,Synopsis,Duration,Image,Video,IsAnnouncement,CoverImage,Genre")] Movie movie, HttpPostedFileBase ImageFile, HttpPostedFileBase CoverImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    ImageFile.SaveAs(path);
                    movie.Image = Path.GetFileName(ImageFile.FileName);
                }

                if (CoverImageFile != null)
                {
                    string fileName = Path.GetFileName(CoverImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    CoverImageFile.SaveAs(path);
                    movie.CoverImage = Path.GetFileName(CoverImageFile.FileName);
                }

                db.Movies.Add(movie);
                db.SaveChanges();
                if (movie.IsAnnouncement == true)
                {
                    return RedirectToAction("Soon");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            ViewBag.Genre = new SelectList(db.Genres, "GenreId", "GenreName");
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MovieId,MovieTitle,OriginalTitle,PremiereDate,Director,Actors,Synopsis,Duration,Image,Video,IsAnnouncement,CoverImage,Genre")] Movie movie, HttpPostedFileBase ImageFile, HttpPostedFileBase CoverImageFile)
        {
            if (ModelState.IsValid)
            {
                
                if (ImageFile != null)
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    ImageFile.SaveAs(path);
                    movie.Image = Path.GetFileName(ImageFile.FileName);
                }
                else
                {
                    movie.Image = movie.Image;
                }

                if (CoverImageFile != null)
                {
                    string fileName = Path.GetFileName(CoverImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    CoverImageFile.SaveAs(path);
                    movie.CoverImage = Path.GetFileName(CoverImageFile.FileName);
                }
                else
                {
                    movie.CoverImage = movie.CoverImage;
                }

                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                if (movie.IsAnnouncement == true)
                {
                    return RedirectToAction("Soon");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult All()
        {
            return View(db.Movies.ToList());
        }

        public ActionResult Repertoire(DateTime? Date)
        {
            DateTime today = DateTime.Now;
            IList<DateTime> dates = new List<DateTime>();
            for(int i=0; i < 7; i++)
            {
                dates.Add(DateTime.Now.AddDays(i));
            }
            ViewData["Dates"] = dates;
            ViewBag.Date = (Date != null) ? Date.Value.ToShortDateString() : DateTime.Now.ToShortDateString();
            ViewBag.Culture = culture;
            ViewBag.DateSet = "";
            if(Date != null)
            {
                ViewBag.DateSet = culture.DateTimeFormat.GetDayName(Date.Value.DayOfWeek);
            }
            
            return View(db.Movies.ToList());
        }

        public ActionResult Soon()
        {
            return View(db.Movies.ToList());
        }

        public ActionResult ReservateTickets(int? movieId, int projectionId)
        {
            return RedirectToAction("Create", "Reservations", new { movieId = movieId, projectionId = projectionId });
        }
    }
}
