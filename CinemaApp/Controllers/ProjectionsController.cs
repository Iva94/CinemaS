using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CinemaApp.Models;

namespace CinemaApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ProjectionsController : Controller
    {
        private cinemaDatabaseEntities db = new cinemaDatabaseEntities();
        CultureInfo culture = new CultureInfo("sr-SR", false);
        
        // GET: Projections
        public ActionResult Index(DateTime? Date)
        {
            ViewBag.Culture = culture;
            IList<Movie> movies = new List<Movie>(db.Movies.Where(p => p.IsAnnouncement == false));
            ViewData["movies"] = movies;
            var projections = db.Projections.Include(p => p.Auditorium).Include(p => p.Movie).OrderBy(p => p.MovieId).OrderBy(p => p.DateTime);
            IList<DateTime> movieProjectionDates = new List<DateTime>();
            foreach (var item in projections)
            {
                movieProjectionDates.Add(item.DateTime.Value.Date);
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
            ViewData["ProjectionDates"] = movieProjectionDates;

            if(Date == null)
            {
                Date = movieProjectionDates[0];
            }

            ViewBag.DateSet = Date;
          
            IList<Movie> moviesOnDate = new List<Movie>();
            foreach (var movie in movies)
            {
                var hasProjection = false;
                foreach(var projection in movie.Projections)
                {
                    if (projection.DateTime.Value.Date == Date)
                    {
                        hasProjection = true;
                    }
                }
                if (hasProjection)
                {
                    moviesOnDate.Add(movie);
                }
            }
            ViewData["moviesOnDate"] = moviesOnDate;

            return View(projections.ToList());
        }

        // GET: Projections/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projection projection = db.Projections.Find(id);
            if (projection == null)
            {
                return HttpNotFound();
            }
            return View(projection);
        }

        // GET: Projections/Create/5
        public ActionResult Create()
        {
            ViewBag.AuditoriumId = new SelectList(db.Auditoriums, "AuditoriumId", "AuditoriumName");
            ViewBag.MovieId = new SelectList(db.Movies.Where(m => m.IsAnnouncement == false).OrderBy(m => m.MovieTitle), "MovieId", "MovieTitle");
            return View();
        }

        // POST: Projections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectionId,MovieId,AuditoriumId,DateTime,TicketPrice")] Projection projection)
        {
            if (ModelState.IsValid)
            {
                var projections = db.Projections.Where(p => p.AuditoriumId == projection.AuditoriumId).Where(p => p.DateTime == projection.DateTime);
                if (projections.Count() != 0)
                {
                    ViewBag.Message = "Termin u odabranoj sali je rezervisan.";
                }
                else
                {
                    db.Projections.Add(projection);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.AuditoriumId = new SelectList(db.Auditoriums, "AuditoriumId", "AuditoriumName", projection.AuditoriumId);
            ViewBag.MovieId = new SelectList(db.Movies.Where(m => m.IsAnnouncement == false).OrderBy(m => m.MovieTitle), "MovieId", "MovieTitle", projection.MovieId);
            return View(projection);
        }

        // GET: Projections/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projection projection = db.Projections.Find(id);
            if (projection == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuditoriumId = new SelectList(db.Auditoriums, "AuditoriumId", "AuditoriumName", projection.AuditoriumId);
            ViewBag.MovieId = new SelectList(db.Movies.Where(m => m.IsAnnouncement == false).OrderBy(m => m.MovieTitle), "MovieId", "MovieTitle", projection.MovieId);
            return View(projection);
        }

        // POST: Projections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjectionId,MovieId,AuditoriumId,DateTime,TicketPrice")] Projection projection)
        {
            if (ModelState.IsValid)
            {
                var projections = db.Projections.Where(p => p.AuditoriumId == projection.AuditoriumId).Where(p => p.DateTime == projection.DateTime);
                if(projections.Count() != 0)
                {
                    ViewBag.Message = "Termin je rezervisan u odabranoj sali!";
                }
                else
                {
                    db.Entry(projection).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.AuditoriumId = new SelectList(db.Auditoriums, "AuditoriumId", "AuditoriumName", projection.AuditoriumId);
            ViewBag.MovieId = new SelectList(db.Movies.Where(m => m.IsAnnouncement == false).OrderBy(m => m.MovieTitle), "MovieId", "MovieTitle", projection.MovieId);
            return View(projection);
        }

        // GET: Projections/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projection projection = db.Projections.Find(id);
            if (projection == null)
            {
                return HttpNotFound();
            }
            return View(projection);
        }

        // POST: Projections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Projection projection = db.Projections.Find(id);
            db.Projections.Remove(projection);
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
    }
}
