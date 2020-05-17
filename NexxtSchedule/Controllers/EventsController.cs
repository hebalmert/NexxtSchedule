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


        // GET: Events/Delete/5
        public ActionResult Asistio(int? id)
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
            evento.Asistencia = true;
            db.Entry(evento).State = EntityState.Modified;
            db.SaveChanges();

            var fe = Convert.ToString(evento.Start);

            return RedirectToAction("Index", new { fecha = fe, profesionalid = evento.ProfessionalId });
        }

        [HttpPost]
        public JsonResult Search2(string Prefix2)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var profesionales = (from profe in db.Professionals
                            where profe.FullName.StartsWith(Prefix2) && profe.CompanyId == user.CompanyId
                            select new
                            {
                                label = profe.FullName,
                                val = profe.ProfessionalId
                            }).ToList();

            return Json(profesionales);

        }

        [HttpPost]
        public JsonResult Search(string Prefix)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var clientes = (from client in db.Clients
                               where client.Cliente.StartsWith(Prefix) && client.CompanyId == user.CompanyId
                               select new
                               {
                                   label = client.Cliente,
                                   val = client.ClientId
                               }).ToList();

            return Json(clientes);

        }

        // GET: Events
        public ActionResult Index(string fecha, int? profesionalid)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!string.IsNullOrEmpty(fecha) && profesionalid != 0 && profesionalid != null)
            {
                DateTime dia =Convert.ToDateTime(fecha);

                var eventos = db.Events.Where(c => c.CompanyId == user.CompanyId && c.Start == dia && c.ProfessionalId == profesionalid)
                    .Include(e => e.Client)
                    .Include(e=> e.Hour)
                    .Include(e=> e.Color)
                    .Include(e => e.Professional);
                     return View(eventos.OrderBy(o=> o.Hour.Hora).ToList());
            }
            else
            {
                var eventos = db.Events.Where(c => c.CompanyId == user.CompanyId && c.ProfessionalId == 0)
                    .Include(e => e.Client)
                    .Include(e => e.Hour)
                    .Include(e => e.Color)
                    .Include(e => e.Professional);
                return View(eventos.OrderBy(o => o.Hour.Hora).ToList());
            }
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
        public ActionResult Create(int clienteId)
        {
            if (clienteId == 0)
            {
                return RedirectToAction("Index");
            }

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var nombrecliente = db.Clients.Find(clienteId);

            var evento = new Event
            {
                CompanyId = user.CompanyId,
                ClientId = clienteId,
                Cliente = nombrecliente.Cliente,
                Start = DateTime.Today,
            };

            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(user.CompanyId), "ProfessionalId", "FullName");
            ViewBag.HourId = new SelectList(ComboHelper.GetHora(), "HourId", "Hora");
            ViewBag.ColorId = new SelectList(ComboHelper.GetColor(), "ColorId", "ColorDate");

            return View(evento);
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Event evento)
        {
            var profesionales = db.Professionals.Find(evento.ProfessionalId);
            evento.Profesional = profesionales.FullName;

            if (ModelState.IsValid)
            {
               
                db.Events.Add(evento);

                try
                {
                    db.SaveChanges();
                    var fe = Convert.ToString(evento.Start);

                    return RedirectToAction("Index", new { fecha = fe, profesionalid  = evento.ProfessionalId});
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

            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(evento.CompanyId), "ProfessionalId", "FullName", evento.ProfessionalId);
            ViewBag.HourId = new SelectList(ComboHelper.GetHora(), "HourId", "Hora");
            ViewBag.ColorId = new SelectList(ComboHelper.GetColor(), "ColorId", "ColorDate");
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

            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(evento.CompanyId), "ProfessionalId", "FullName", evento.ProfessionalId);
            ViewBag.HourId = new SelectList(ComboHelper.GetHora(), "HourId", "Hora", evento.HourId);
            ViewBag.ColorId = new SelectList(ComboHelper.GetColor(), "ColorId", "ColorDate", evento.ColorId);

            return View(evento);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event evento)
        {
            var profesionales = db.Professionals.Find(evento.ProfessionalId);
            evento.Profesional = profesionales.FullName;

            if (ModelState.IsValid)
            {
                db.Entry(evento).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();

                    var fe = Convert.ToString(evento.Start);

                    return RedirectToAction("Index", new { fecha = fe, profesionalid = evento.ProfessionalId });
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

            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(evento.CompanyId), "ProfessionalId", "FullName", evento.ProfessionalId);
            ViewBag.HourId = new SelectList(ComboHelper.GetHora(), "HourId", "Hora", evento.HourId);
            ViewBag.ColorId = new SelectList(ComboHelper.GetColor(), "ColorId", "ColorDate", evento.ColorId);

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
