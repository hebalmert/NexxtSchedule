using NexxtSchedule.Classes;
using NexxtSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NexxtSchedule.Controllers
{
    [Authorize(Roles = "User, Profe")]

    public class CalendarController : Controller
    {
        private NexxtCalContext db = new NexxtCalContext();

        // GET: Calendar
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ProfessionalUp = new SelectList(ComboHelper.GetProfessional(user.CompanyId), "ProfessionalId", "FullName", (0));
            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(user.CompanyId), "ProfessionalId", "FullName", (0));
            ViewBag.ClientId = new SelectList(ComboHelper.GetClients(user.CompanyId), "ClientId", "Cliente", (0));

            return View();
        }

        [HttpPost]
        public JsonResult SaveEvent(Event e)
        {
            var status = false;
            db.Configuration.ProxyCreationEnabled = false;

            var db3 = new NexxtCalContext();
            db3.Configuration.ProxyCreationEnabled = false;
            var profesionales = db3.Professionals.Find(e.ProfessionalId);

            var db4 = new NexxtCalContext();
            db4.Configuration.ProxyCreationEnabled = false;
            var clientes = db4.Clients.Find(e.ClientId);

            {
                if (e.EventId > 0)
                {
                    //Update the event

                    var db2 = new NexxtCalContext();
                    db2.Configuration.ProxyCreationEnabled = false;
                    var user = db2.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                    var v = db.Events.Where(a => a.EventId == e.EventId).FirstOrDefault();
                    if (v != null)
                    {
                        if (e.End == null)
                        {
                            e.End = e.Start;
                        }
                        v.CompanyId = user.CompanyId;
                        v.ProfessionalId = e.ProfessionalId;
                        v.Profesional = profesionales.FullName;
                        v.ClientId = e.ClientId;
                        v.Cliente = clientes.Cliente;
                        v.Subject = e.Subject;
                        v.Start = TimeZoneInfo.ConvertTimeFromUtc(e.Start, ComboHelper.GetTimeZone());
                        v.End = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(e.End),ComboHelper.GetTimeZone());
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                    }
                    db2.Dispose();
                }
                else
                {
                    var db2 = new NexxtCalContext();
                    db2.Configuration.ProxyCreationEnabled = false;
                    var user = db2.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                    //DateTime date1 = e.Start;
                    //TimeZoneInfo tz = TimeZoneInfo.CreateCustomTimeZone("COLOMBIA", new TimeSpan(-3, 0, 0), "Colombia", "Colombia");
                    //DateTime custDateTime1 = TimeZoneInfo.ConvertTimeFromUtc(date1, tz);
                    var nuevoEvento = new Event
                    {
                        CompanyId = user.CompanyId,
                        ProfessionalId = e.ProfessionalId,
                        Profesional = profesionales.FullName,
                        ClientId = e.ClientId,
                        Cliente = clientes.Cliente,
                        Subject = e.Subject,
                        Start =e.Start,
                        End = e.End,
                        Description = e.Description,
                        IsFullDay = e.IsFullDay,
                        ThemeColor = e.ThemeColor,
                    };
                    db2.Dispose();
                    db.Events.Add(nuevoEvento);
                }
                db.SaveChanges();
                db3.Dispose();
                db4.Dispose();

                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int EventId)
        {
            var status = false;
            db.Configuration.ProxyCreationEnabled = false;
            {
                var v = db.Events.Where(a => a.EventId == EventId).FirstOrDefault();
                if (v != null)
                {
                    db.Events.Remove(v);
                    db.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        public JsonResult GetEvents(int? professionalId)
        {
            {
                var db2 = new NexxtCalContext();
                db2.Configuration.ProxyCreationEnabled = false;
                var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var compania = user.CompanyId;
                db2.Dispose();

                db.Configuration.ProxyCreationEnabled = false;
                if (professionalId > 0)
                {
                    var events = db.Events.Where(p => p.ProfessionalId == professionalId && p.CompanyId == compania).ToList();
                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    var events = db.Events.Where(p => p.CompanyId == compania && p.ProfessionalId == 0).ToList();
                    return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

            }
        }

        public JsonResult CallProfesional()
        {
            var db2 = new NexxtCalContext();
            db2.Configuration.ProxyCreationEnabled = false;
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var compania = user.CompanyId;
            db2.Dispose();

            db.Configuration.ProxyCreationEnabled = false;
            var profe = db.Professionals.Where(c => c.CompanyId == compania);
            return Json(profe);
        }
    }
}