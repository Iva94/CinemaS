using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CinemaApp.Models;

namespace CinemaApp.Controllers
{
    public class SpecialOffersController : Controller
    {
        private cinemaDatabaseEntities db = new cinemaDatabaseEntities();

        // GET: SpecialOffers
        public ActionResult Index()
        {
            return View(db.SpecialOffers.ToList());
        }

        // GET: SpecialOffers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialOffer specialOffer = db.SpecialOffers.Find(id);
            if (specialOffer == null)
            {
                return HttpNotFound();
            }
            return View(specialOffer);
        }

        // GET: SpecialOffers/Create
        [Authorize(Roles = "Administrator")]

        public ActionResult Create()
        {
            return View();
        }

        // POST: SpecialOffers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OfferId,OfferName,Description,OfferImage")] SpecialOffer specialOffer, HttpPostedFileBase OfferImageFile)
        {
            if (ModelState.IsValid)
            {
                if (OfferImageFile != null)
                {
                    string fileName = Path.GetFileName(OfferImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    OfferImageFile.SaveAs(path);
                    specialOffer.OfferImage = Path.GetFileName(OfferImageFile.FileName);
                }
                db.SpecialOffers.Add(specialOffer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(specialOffer);
        }

        // GET: SpecialOffers/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialOffer specialOffer = db.SpecialOffers.Find(id);
            if (specialOffer == null)
            {
                return HttpNotFound();
            }
            return View(specialOffer);
        }

        // POST: SpecialOffers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OfferId,OfferName,Description,OfferImage")] SpecialOffer specialOffer, HttpPostedFileBase OfferImageFile)
        {
            if (ModelState.IsValid)
            {
                if (OfferImageFile != null)
                {
                    string fileName = Path.GetFileName(OfferImageFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    OfferImageFile.SaveAs(path);
                    specialOffer.OfferImage = Path.GetFileName(OfferImageFile.FileName);
                }
                db.Entry(specialOffer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(specialOffer);
        }

        // GET: SpecialOffers/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialOffer specialOffer = db.SpecialOffers.Find(id);
            if (specialOffer == null)
            {
                return HttpNotFound();
            }
            return View(specialOffer);
        }

        // POST: SpecialOffers/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SpecialOffer specialOffer = db.SpecialOffers.Find(id);
            db.SpecialOffers.Remove(specialOffer);
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
