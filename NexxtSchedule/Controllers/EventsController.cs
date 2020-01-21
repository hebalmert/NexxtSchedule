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
    [Authorize(Roles = "User, Profe")]

    public class EventsController : Controller
    {
        private NexxtCalContext db = new NexxtCalContext();

        // GET: Events
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var events = db.Events.Where(c => c.CompanyId == user.CompanyId)
                .Include(e => e.Client)
                .Include(e => e.Professional);
            return View(events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evento = db.Events.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return View(evento);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var evento = new Event
            {
                CompanyId = user.CompanyId,
                Start = DateTime.Today,
                End = DateTime.Today
            };

            ViewBag.ClientId = new SelectList(ComboHelper.GetClients(user.CompanyId), "ClientId", "Cliente");
            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(user.CompanyId), "FullName", "FullName");

            return View(evento);
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Event evento)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(evento);
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
                        ModelState.AddModelError(string.Empty, ("Existe un Registro con el mismo Valor"));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }

            ViewBag.ClientId = new SelectList(ComboHelper.GetClients(evento.CompanyId), "ClientId", "Cliente", evento.ClientId);
            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(evento.CompanyId), "ProfessionalId", "FullName", evento.ProfessionalId);

            return View(evento);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evento = db.Events.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }

            ViewBag.ClientId = new SelectList(ComboHelper.GetClients(evento.CompanyId), "ClientId", "Cliente", evento.ClientId);
            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(evento.CompanyId), "ProfessionalId", "FullName", evento.ProfessionalId);

            return View(evento);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event evento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evento).State = EntityState.Modified;
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
                        ModelState.AddModelError(string.Empty, ("Existe un Registro con el mismo Valor"));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }

            ViewBag.ClientId = new SelectList(ComboHelper.GetClients(evento.CompanyId), "ClientId", "Cliente", evento.ClientId);
            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(evento.CompanyId), "ProfessionalId", "FullName", evento.ProfessionalId);

            return View(evento);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var evento = db.Events.Find(id);
            if (evento == null)
            {
                return HttpNotFound();
            }
            return View(evento);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var evento = db.Events.Find(id);
            db.Events.Remove(evento);
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
                    ModelState.AddModelError(string.Empty, ("El Registo no puede ser Eliminado, porque tiene relacion con otros Datos"));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(evento);
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
