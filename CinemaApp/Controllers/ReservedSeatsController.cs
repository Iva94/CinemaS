using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CinemaApp.Models;

namespace CinemaApp.Controllers
{
    public class ReservedSeatsController : Controller
    {
        private cinemaDatabaseEntities db = new cinemaDatabaseEntities();

        // GET: ReservedSeats
        public ActionResult Index()
        {
            var reservedSeats = db.ReservedSeats.Include(r => r.Projection).Include(r => r.Reservation).Include(r => r.Seat);
            return View(reservedSeats.ToList());
        }

        // GET: ReservedSeats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservedSeat reservedSeat = db.ReservedSeats.Find(id);
            if (reservedSeat == null)
            {
                return HttpNotFound();
            }
            return View(reservedSeat);
        }

        // GET: ReservedSeats/Create
        public ActionResult Create()
        {
            //int? movieId, int projectionId
            ViewBag.ProjectionId = new SelectList(db.Projections, "ProjectionId", "ProjectionId");
            ViewBag.ReservationId = new SelectList(db.Reservations, "ReservationId", "UserId");
            ViewBag.SeatId = new SelectList(db.Seats, "SeatId", "Row");
            return View();
        }

        // POST: ReservedSeats/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ReservedSeatId,ReservationId,SeatId,ProjectionId")] ReservedSeat reservedSeat)
        {
            if (ModelState.IsValid)
            {
                //Reservation newReservation = new Reservation();
                //newReservation.ProjectionId = reservedSeat.ProjectionId;
                //db.Reservations.Add(newReservation);
                //db.SaveChanges();
                //reservedSeat.ReservationId = newReservation.ReservationId;
                db.ReservedSeats.Add(reservedSeat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectionId = new SelectList(db.Projections, "ProjectionId", "ProjectionId", reservedSeat.ProjectionId);
            ViewBag.ReservationId = new SelectList(db.Reservations, "ReservationId", "UserId", reservedSeat.ReservationId);
            ViewBag.SeatId = new SelectList(db.Seats, "SeatId", "Row", reservedSeat.SeatId);
            return View(reservedSeat);
        }



        // GET: ReservedSeats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservedSeat reservedSeat = db.ReservedSeats.Find(id);
            if (reservedSeat == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectionId = new SelectList(db.Projections, "ProjectionId", "ProjectionId", reservedSeat.ProjectionId);
            ViewBag.ReservationId = new SelectList(db.Reservations, "ReservationId", "UserId", reservedSeat.ReservationId);
            ViewBag.SeatId = new SelectList(db.Seats, "SeatId", "Row", reservedSeat.SeatId);
            return View(reservedSeat);
        }

        // POST: ReservedSeats/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReservedSeatId,ReservationId,SeatId,ProjectionId")] ReservedSeat reservedSeat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservedSeat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectionId = new SelectList(db.Projections, "ProjectionId", "ProjectionId", reservedSeat.ProjectionId);
            ViewBag.ReservationId = new SelectList(db.Reservations, "ReservationId", "UserId", reservedSeat.ReservationId);
            ViewBag.SeatId = new SelectList(db.Seats, "SeatId", "Row", reservedSeat.SeatId);
            return View(reservedSeat);
        }

        // GET: ReservedSeats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReservedSeat reservedSeat = db.ReservedSeats.Find(id);
            if (reservedSeat == null)
            {
                return HttpNotFound();
            }
            return View(reservedSeat);
        }

        // POST: ReservedSeats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReservedSeat reservedSeat = db.ReservedSeats.Find(id);
            db.ReservedSeats.Remove(reservedSeat);
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
