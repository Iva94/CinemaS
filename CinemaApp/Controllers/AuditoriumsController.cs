using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CinemaApp.Models;

namespace CinemaApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AuditoriumsController : Controller
    {
        private cinemaDatabaseEntities db = new cinemaDatabaseEntities();

        // GET: Auditoriums
        public ActionResult Index()
        {
            return View(db.Auditoriums.OrderBy(a => a.AuditoriumName).ToList());
        }

        // GET: Auditoriums/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auditorium auditorium = db.Auditoriums.Find(id);

            IList<Seat> seats = new List<Seat>(db.Seats.Where(r => r.AuditoriumId == id));
            ViewData["seats"] = seats;
            
            if (auditorium == null)
            {
                return HttpNotFound();
            }
            return View(auditorium);
        }

        // GET: Auditoriums/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Auditoriums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AuditoriumId,AuditoriumName,Capacity,NumberOfRows,NumberOfColumns")] Auditorium auditorium, int[] numOfSeats)
        {
            if (ModelState.IsValid)
            {
                var auditoriums = db.Auditoriums.Where(a => a.AuditoriumName == auditorium.AuditoriumName);
                if (auditoriums.Count() != 0)
                {
                    ViewBag.Message = "Sala sa ovom oznakom već postoji u bazi.";
                }
                else
                {
                    db.Auditoriums.Add(auditorium);
                    db.SaveChanges();

                    var insertedAuditorium = db.Auditoriums.Where(a => a.AuditoriumName == auditorium.AuditoriumName).FirstOrDefault();

                    for (int i = 0; i < insertedAuditorium.NumberOfRows; i++)
                    {
                        for (int j = 0; j < numOfSeats[i]; j++)
                        {
                            Seat seat = new Seat();
                            seat.AuditoriumId = insertedAuditorium.AuditoriumId;
                            seat.Row = ((char)('A' + (i))).ToString();
                            seat.Number = j + 1;
                            db.Seats.Add(seat);
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("Index");
                }
            }
            return View(auditorium);
        }

        // GET: Auditoriums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auditorium auditorium = db.Auditoriums.Find(id);
            if (auditorium == null)
            {
                return HttpNotFound();
            }
            return View(auditorium);
        }

        // POST: Auditoriums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AuditoriumId,AuditoriumName,Capacity, NumberOfRows, NumberOfColumns")] Auditorium auditorium, int[] numOfSeats)
        {
            if (ModelState.IsValid)
            {
                var auditoriums = db.Auditoriums.Where(a => a.AuditoriumName == auditorium.AuditoriumName).Where(a => a.AuditoriumId != auditorium.AuditoriumId);
                if (auditoriums.Count() != 0)
                {
                    ViewBag.Message = "Sala sa istom oznakom već postoji u bazi.";
                }
                else
                {
                    for (int i = 0; i < auditorium.NumberOfRows; i++)
                    {
                        string row = ((char)('A' + (i))).ToString();
                        for (int j = 0; j < numOfSeats[i]; j++)
                        {
                            var seatExist = db.Seats.Where(s => s.AuditoriumId == auditorium.AuditoriumId).Where(s => s.Row == row).Where(s => s.Number == (j+1)).FirstOrDefault();
                            if(seatExist == null) { 
                                Seat seat = new Seat();
                                seat.AuditoriumId = auditorium.AuditoriumId;
                                seat.Row = ((char)('A' + (i))).ToString();
                                seat.Number = j + 1;
                                db.Seats.Add(seat);
                                db.SaveChanges();
                            }
                        }
                    }
                    db.Entry(auditorium).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(auditorium);
        }

        // GET: Auditoriums/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auditorium auditorium = db.Auditoriums.Find(id);
            if (auditorium == null)
            {
                return HttpNotFound();
            }
            return View(auditorium);
        }

        // POST: Auditoriums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Auditorium auditorium = db.Auditoriums.Find(id);
            db.Auditoriums.Remove(auditorium);
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

        public ActionResult AddSeat(int auditoriumId)
        {
            return RedirectToAction("Create", "Seats", new { auditoriumId = auditoriumId });
        }
    }
}
