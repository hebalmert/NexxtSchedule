using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NexxtSchedule.Classes;
using NexxtSchedule.Models;

namespace NexxtSchedule.Controllers
{
    public class DirectGeneralsController : Controller
    {
        private NexxtCalContext db = new NexxtCalContext();

        // GET: DirectGenerals
        public ActionResult Index(int? ProfessionalId)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (ProfessionalId != 0 || ProfessionalId != null)
            {
                var directGenerals = db.DirectGenerals.Where(c => c.CompanyId == user.CompanyId && c.Facturado == false && c.ProfessionalId == ProfessionalId)
                    .Include(d => d.Client)
                    .Include(d => d.DirectPayment)
                    .Include(d => d.Professional);
                ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(user.CompanyId), "ProfessionalId", "FullName", (0));
                return View(directGenerals.OrderByDescending(t => t.Date).ToList());
            }
            else
            {
                var directGenerals = db.DirectGenerals.Where(c => c.CompanyId == user.CompanyId && c.Facturado == false && c.Client.Cliente == "rt445fff45345")
                    .Include(d => d.Client)
                    .Include(d => d.DirectPayment)
                    .Include(d => d.Professional);
                ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(user.CompanyId), "ProfessionalId", "FullName", (0));
                return View(directGenerals.OrderBy(t => t.Date).ToList());
            }
        }

        // GET: DirectGenerals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var directGeneral = db.DirectGenerals.Find(id);
            if (directGeneral == null)
            {
                return HttpNotFound();
            }
            return View(directGeneral);
        }

        // GET: DirectGenerals/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //var paydirect = new DirectPayment
            //{
            //    CompanyId = user.CompanyId,
            //    Date = DateTime.Today
            //};

            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Cliente");
            ViewBag.DirectPaymentId = new SelectList(db.DirectPayments, "DirectPaymentId", "NotaCobro");
            ViewBag.ProfessionalId = new SelectList(db.Professionals, "ProfessionalId", "FirstName");

            return View();
        }

        // POST: DirectGenerals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DirectGeneral directGeneral)
        {
            if (ModelState.IsValid)
            {
                db.DirectGenerals.Add(directGeneral);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("_Index"))
                    {
                        ModelState.AddModelError(string.Empty, @Resources.Resource.Msg_DoubleData);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }

            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Cliente", directGeneral.ClientId);
            ViewBag.DirectPaymentId = new SelectList(db.DirectPayments, "DirectPaymentId", "NotaCobro", directGeneral.DirectPaymentId);
            ViewBag.ProfessionalId = new SelectList(db.Professionals, "ProfessionalId", "FirstName", directGeneral.ProfessionalId);
            return View(directGeneral);
        }

        // GET: DirectGenerals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var directGeneral = db.DirectGenerals.Find(id);
            if (directGeneral == null)
            {
                return HttpNotFound();
            }

            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Cliente", directGeneral.ClientId);
            ViewBag.DirectPaymentId = new SelectList(db.DirectPayments, "DirectPaymentId", "NotaCobro", directGeneral.DirectPaymentId);
            ViewBag.ProfessionalId = new SelectList(db.Professionals, "ProfessionalId", "FirstName", directGeneral.ProfessionalId);
            
            return View(directGeneral);
        }

        // POST: DirectGenerals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DirectGeneral directGeneral)
        {
            if (ModelState.IsValid)
            {
                db.Entry(directGeneral).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("_Index"))
                    {
                        ModelState.AddModelError(string.Empty, @Resources.Resource.Msg_DoubleData);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }

            ViewBag.ClientId = new SelectList(db.Clients, "ClientId", "Cliente", directGeneral.ClientId);
            ViewBag.DirectPaymentId = new SelectList(db.DirectPayments, "DirectPaymentId", "NotaCobro", directGeneral.DirectPaymentId);
            ViewBag.ProfessionalId = new SelectList(db.Professionals, "ProfessionalId", "FirstName", directGeneral.ProfessionalId);
            
            return View(directGeneral);
        }

        // GET: DirectGenerals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectGeneral directGeneral = db.DirectGenerals.Find(id);
            if (directGeneral == null)
            {
                return HttpNotFound();
            }
            return View(directGeneral);
        }

        // POST: DirectGenerals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DirectGeneral directGeneral = db.DirectGenerals.Find(id);
            db.DirectGenerals.Remove(directGeneral);
            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    ModelState.AddModelError(string.Empty, @Resources.Resource.Msg_Relationship);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(directGeneral);
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
