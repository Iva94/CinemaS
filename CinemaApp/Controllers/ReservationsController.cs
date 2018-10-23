using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CinemaApp.Models;
using Microsoft.AspNet.Identity;

namespace CinemaApp.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private cinemaDatabaseEntities db = new cinemaDatabaseEntities();
        CultureInfo culture = new CultureInfo("bs-Latn", false);

        [Authorize(Roles = "Administrator")]
        // GET: Reservations
        public ActionResult Index(int? movie, DateTime? dateTime, int? projection)
        {
            ViewBag.Culture = culture;
            ViewBag.Projection = projection;
            ViewBag.Message = "";
            IList<Reservation> reservations = new List<Reservation>(db.Reservations.Include(r => r.Projection).Include(r => r.User));
            IList<Reservation> newReservations = CheckIsActive(reservations);
            if (projection != null)
            {
                var selectedProjection = db.Projections.Include(p => p.Auditorium).Include(p => p.Movie).Where(p => p.ProjectionId == projection).FirstOrDefault();

                if (selectedProjection != null)
                {
                    IList<Reservation> selectedReservations = new List<Reservation>(db.Reservations.Include(r => r.User).Where(r => r.ProjectionId == projection));
                    ViewData["reservations"] = selectedReservations;
                    ViewBag.NumOfReservedSeats = selectedReservations.Count;
                    IList<Seat> seats = new List<Seat>(db.Seats.Where(s => s.AuditoriumId == selectedProjection.AuditoriumId));
                    ViewData["seats"] = seats;
                    Auditorium auditorium = db.Auditoriums.Where(a => a.AuditoriumId == selectedProjection.AuditoriumId).FirstOrDefault();
                    ViewBag.Auditorium = auditorium.AuditoriumName;
                    ViewBag.MovieTitle = selectedProjection.Movie.MovieTitle;
                    ViewBag.MovieImage = selectedProjection.Movie.Image;
                    ViewBag.Date = selectedProjection.DateTime.Value.ToString("dd. MMMM", culture);
                    ViewBag.Time = selectedProjection.DateTime.Value.ToShortTimeString();
                    ViewBag.TicketPrice = selectedProjection.TicketPrice;

                    IList<ReservedSeat> reservedSeats = new List<ReservedSeat>(db.ReservedSeats.Where(r => r.ProjectionId == selectedProjection.ProjectionId));
                    ViewBag.NumOfReservedSeats = reservedSeats.Count();
                    ViewData["reservedSeats"] = reservedSeats;
                    newReservations = CheckIsActive(selectedReservations);
                    ViewData["expiredReservations"] = new List<Reservation>(db.Reservations.Include(r => r.User).Where(r => r.ProjectionId == selectedProjection.ProjectionId).Where(r => r.IsActive == false)); ;
                    ViewBag.HasResults = true;    
                }
                else
                {
                    ViewBag.HasResult = false;
                    ViewBag.Message = "Film nema projekciju za odabrani termin.";
                }
            }

            if (movie != null && dateTime != null)
            {
                var projection2 = db.Projections.Include(p => p.Auditorium).Include(p => p.Movie).Where(p => p.MovieId == movie).Where(p => p.DateTime == dateTime).FirstOrDefault();

                if (projection2 != null)
                {
                    IList<Reservation> selectedReservations = new List<Reservation>(db.Reservations.Include(r => r.User).Include(r => r.Projection).Where(r => r.ProjectionId == projection2.ProjectionId));
                    ViewData["reservations"] = selectedReservations;
                    ViewBag.NumOfReservedSeats = selectedReservations.Count;
                    IList<Seat> seats = new List<Seat>(db.Seats.Where(s => s.AuditoriumId == projection2.AuditoriumId));
                    ViewData["seats"] = seats;
                    Auditorium auditorium = db.Auditoriums.Where(a => a.AuditoriumId == projection2.AuditoriumId).FirstOrDefault();
                    ViewBag.Auditorium = auditorium.AuditoriumName;
                    Movie movie2 = db.Movies.Where(m => m.MovieId == projection2.MovieId).FirstOrDefault();
                    ViewBag.MovieImage = movie2.Image;
                    ViewBag.Date = projection2.DateTime.Value.ToString("dd. MMMM", culture);
                    ViewBag.Time = projection2.DateTime.Value.ToShortTimeString();
                    IList<ReservedSeat> reservedSeats = new List<ReservedSeat>(db.ReservedSeats.Where(r => r.ProjectionId == projection2.ProjectionId));
                    ViewBag.NumOfReservedSeats = reservedSeats.Count();
                    ViewData["reservedSeats"] = reservedSeats;
                    newReservations = CheckIsActive(selectedReservations);
                    ViewData["expiredReservations"] = new List<Reservation>(db.Reservations.Include(r => r.User).Where(r => r.ProjectionId == projection2.ProjectionId).Where(r => r.IsActive == false));
                    ViewBag.HasResults = true;
                }
                else
                {
                    ViewBag.HasResult = false;
                    ViewBag.Message = "Film nema projekciju za odabrani termin!";
                }
            }
            
            List<Movie> movies = new List<Movie>(db.Movies.Where(m => m.IsAnnouncement == false));
            
            ViewData["movies"] = (IEnumerable<Movie>)movies;

            using (cinemaDatabaseEntities dbCinema = new cinemaDatabaseEntities())
            {
                ViewBag.Movies = dbCinema.Movies.Where(m => m.IsAnnouncement == false).OrderBy(m => m.MovieTitle).ToList();
            }
            
            return View(newReservations.ToList());
        }


        public ActionResult GetDateList(int movieId)
        {
            List<Projection> movieProjections = new List<Projection>();
            int stateiD = Convert.ToInt32(movieId);
            using (cinemaDatabaseEntities dbCinema = new cinemaDatabaseEntities())
            {
                movieProjections = (dbCinema.Projections.Where(p => p.MovieId == movieId)).ToList<Projection>();
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(movieProjections);
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult MyReservations()
        {
            var currentUser = User.Identity.GetUserId();
            var reservations = db.Reservations.Include(r => r.Projection).Include(r => r.User).Where(r => r.UserId == currentUser);

            IList<ReservedSeat> allReservedSeats = new List<ReservedSeat>();
            foreach (var item in reservations)
            {
                IList<ReservedSeat> reservedSeats = new List<ReservedSeat>(db.ReservedSeats.Include(r => r.Seat).Where(r => r.ReservationId == item.ReservationId));
                foreach (var seat in reservedSeats)
                {
                    allReservedSeats.Add(seat);
                }
            }

            ViewData["allReservedSeats"] = allReservedSeats;
            
            return View(reservations.ToList());
        }

        // GET: Reservations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }

            IList<ReservedSeat> reservedSeats = new List<ReservedSeat>(db.ReservedSeats.Where(r => r.ReservationId == id).Include(r => r.Seat));
            ViewData["ReservedSeats"] = reservedSeats;
            ViewBag.Culture = culture;
            return View(reservation);
        }

        // GET: Reservations/Create/4
        public ActionResult Create(int movieId, int projectionId)
        {
            ViewBag.ProjectionId = new SelectList(db.Projections.Where(p => p.MovieId == movieId), "ProjectionId", "ProjectionId");
            var projection = db.Projections.Include(p => p.Auditorium).Include(p => p.Movie).Where(p => p.ProjectionId == projectionId).FirstOrDefault();
            ViewBag.Movie = new SelectList(db.Movies.Where(p => p.MovieId == movieId), "MovieId", "MovieId");
            
            ViewBag.ProjectionId = new SelectList(db.Projections.Where(p => p.MovieId == movieId), "ProjectionId", "ProjectionId");
            ViewBag.Auditorium = new SelectList(db.Auditoriums.Where(a => a.AuditoriumId == projection.AuditoriumId), "Auditorium", "Auditorium");
            ViewBag.ProjectionDate = new SelectList(db.Projections.Where(p => p.ProjectionId == projectionId), "DateTime", "DateTime");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Email");

            CultureInfo culture = new CultureInfo("sr-SR", false);
            ViewBag.TicketPrice = projection.TicketPrice;
            ViewBag.Date = projection.DateTime.Value.ToString("dd. MMMM, yyyy", culture);
            ViewBag.Time = projection.DateTime.Value.ToShortTimeString();

            IList<Seat> seats = new List<Seat>(db.Seats.Where(s => s.AuditoriumId == projection.AuditoriumId));
            ViewData["seats"] = seats;

            Movie movie = db.Movies.Where(m => m.MovieId == movieId).FirstOrDefault();
            ViewBag.MovieTitle = movie.MovieTitle;
            ViewBag.MovieImage = movie.Image;
            
            Auditorium auditorium = db.Auditoriums.Where(a => a.AuditoriumId == projection.AuditoriumId).FirstOrDefault();

            ViewBag.Auditorium = auditorium.AuditoriumName;
            ViewBag.NumOfRows = auditorium.NumberOfRows;
            ViewBag.NumOfColumns = auditorium.NumberOfColumns;

            IList<ReservedSeat> reservedSeats = new List<ReservedSeat>(db.ReservedSeats.Where(r => r.ProjectionId == projectionId));
            ViewBag.NumOfReservedSeats = reservedSeats.Count();
            ViewData["reservedSeats"] = reservedSeats;

            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReservationId,ProjectionId,NumberOfTickets,TotalPrice,Reservationtime,IsConfirmed,IsActive,UserId")] Reservation reservation, int? movieId, int projectionId, int[] seats)
        {
            if (ModelState.IsValid)
            {
                var projekcija = db.Projections.Where(p => p.ProjectionId == projectionId).FirstOrDefault();
                reservation.ProjectionId = projectionId;
                reservation.UserId = User.Identity.GetUserId();
                reservation.ReservationTime = DateTime.Now;
                reservation.ExpireTime = projekcija.DateTime - new TimeSpan(0, 30, 0);
                reservation.IsConfirmed = false;
                reservation.IsActive = true;
                db.Reservations.Add(reservation);
                db.SaveChanges();
                foreach (var item in seats)
                {
                    ReservedSeat reservedSeat = new ReservedSeat();
                    reservedSeat.ReservationId = reservation.ReservationId;
                    reservedSeat.ProjectionId = reservation.ProjectionId;
                    reservedSeat.SeatId = item;
                    db.ReservedSeats.Add(reservedSeat);
                    db.SaveChanges();
                }
                return RedirectToAction("MyReservations");
            }

            ViewBag.ProjectionId = new SelectList(db.Projections, "ProjectionId", "ProjectionId", reservation.ProjectionId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Email", reservation.UserId);
            return View(reservation);
        }


        // GET: Reservations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectionId = new SelectList(db.Projections, "ProjectionId", "ProjectionId", reservation.ProjectionId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Email", reservation.UserId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReservationId,ProjectionId,NumberOfTickets,TotalPrice,Reservationtime,IsConfirmed,IsActive,UserId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectionId = new SelectList(db.Projections, "ProjectionId", "ProjectionId", reservation.ProjectionId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Email", reservation.UserId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            db.Reservations.Remove(reservation);
            db.SaveChanges();
            return RedirectToAction("MyReservations");
        }

        // GET: Reservations/Delete/5
        public ActionResult CancelReservation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }


        public List<Reservation> CheckIsActive(IList<Reservation> reservations)
        {
            foreach(var item in reservations)
            {
                if(item.ExpireTime < DateTime.Now)
                {
                    if(item.IsConfirmed != true)
                    {
                        item.IsActive = false;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            var newReservations = new List<Reservation>(db.Reservations.Include(r => r.Projection).Include(r => r.User));

            return newReservations;
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
