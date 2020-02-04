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
    [Authorize(Roles = "User")]

    public class DirectPaymentsController : Controller
    {
        private NexxtCalContext db = new NexxtCalContext();

        // GET: DirectPayments
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var directPayments = db.DirectPayments.Where(c => c.CompanyId == user.CompanyId)
                .Include(d => d.Client)
                .Include(d => d.LevelPrice)
                .Include(d => d.Professional)
                .Include(d => d.Service)
                .Include(d => d.ServiceCategory);

            return View(directPayments.OrderByDescending(o=> o.Date).ToList());
        }

        // GET: DirectPayments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var directPayment = db.DirectPayments.Find(id);
            if (directPayment == null)
            {
                return HttpNotFound();
            }
            return View(directPayment);
        }

        // GET: DirectPayments/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var paydirect = new DirectPayment 
            { 
                CompanyId = user.CompanyId,
                Date = DateTime.Today
            };

            ViewBag.ClientId = new SelectList(ComboHelper.GetClients(user.CompanyId), "ClientId", "Cliente");
            ViewBag.LevelPriceId = new SelectList(ComboHelper.GetLevelPrice(), "LevelPriceId", "NivelPrecio");
            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(user.CompanyId), "ProfessionalId", "FullName");
            ViewBag.ServiceId = new SelectList(ComboHelper.GetServices(user.CompanyId), "ServiceId", "Servicio");
            ViewBag.ServiceCategoryId = new SelectList(ComboHelper.GetServicecategories(user.CompanyId), "ServiceCategoryId", "Categoria");

            return View(paydirect);
        }

        // POST: DirectPayments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DirectPayment directPayment)
        {
            if (ModelState.IsValid)
            {
                db.DirectPayments.Add(directPayment);
                try
                {
                    int Recep = 0;
                    int sum = 0;
                    var db2 = new NexxtCalContext();
                    var db3 = new NexxtCalContext();

                    var register = db2.Registers.Where(c => c.CompanyId == directPayment.CompanyId).FirstOrDefault();
                    Recep = register.NotaCobro;
                    sum = Recep + 1;
                    register.NotaCobro = sum;
                    db2.Entry(register).State = EntityState.Modified;
                    db2.SaveChanges();

                    var servicios = db3.Services.Find(directPayment.ServiceId);

                    directPayment.NotaCobro = Convert.ToString(sum);
                    directPayment.ServicioNombre = servicios.Servicio;

                    db3.Dispose();
                    db2.Dispose();

                    db.SaveChanges();

                    // Incrustar nuevo registro en la tabla de DirectGeneral
                    ////////////////////////////////////////////////////////
                    var db4 = new NexxtCalContext();
                    decimal rate = directPayment.TasaProfesional;
                    decimal iva = 0;
                    decimal tasapro = 0;
                    decimal TotalOriginal = directPayment.Total;
                    iva = rate + 1;
                    tasapro = (iva * TotalOriginal) - TotalOriginal;


                    var directogeneral = new DirectGeneral
                    {
                        DirectPaymentId = directPayment.DirectPaymentId,
                        CompanyId = directPayment.CompanyId,
                        Date = directPayment.Date,
                        NotaCobro = directPayment.NotaCobro,
                        ProfessionalId = directPayment.ProfessionalId,
                        TasaProfesional = directPayment.TasaProfesional,
                        ClientId = directPayment.ClientId,
                        ServicioNombre = directPayment.ServicioNombre,
                        Tasa = directPayment.Tasa,
                        Precio = directPayment.Precio,
                        Cantidad = directPayment.Cantidad,
                        Total = directPayment.Total,
                        PagoProfesional = tasapro
                    };
                    db4.DirectGenerals.Add(directogeneral);
                    db4.SaveChanges();
                    db4.Dispose();
                    /////////////////////////////////////////////////////////
                    ///
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

            ViewBag.ClientId = new SelectList(ComboHelper.GetClients(directPayment.CompanyId), "ClientId", "Cliente", directPayment.ClientId);
            ViewBag.LevelPriceId = new SelectList(ComboHelper.GetLevelPrice(), "LevelPriceId", "NivelPrecio", directPayment.LevelPriceId);
            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(directPayment.CompanyId), "ProfessionalId", "FullName", directPayment.ProfessionalId);
            ViewBag.ServiceId = new SelectList(ComboHelper.GetServices(directPayment.CompanyId), "ServiceId", "Servicio", directPayment.ServiceId);
            ViewBag.ServiceCategoryId = new SelectList(ComboHelper.GetServicecategories(directPayment.CompanyId), "ServiceCategoryId", "Categoria", directPayment.ServiceCategoryId);
            
            return View(directPayment);
        }

        // GET: DirectPayments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var directPayment = db.DirectPayments.Find(id);
            if (directPayment == null)
            {
                return HttpNotFound();
            }

            ViewBag.ClientId = new SelectList(ComboHelper.GetClients(directPayment.CompanyId), "ClientId", "Cliente", directPayment.ClientId);
            ViewBag.LevelPriceId = new SelectList(ComboHelper.GetLevelPrice(), "LevelPriceId", "NivelPrecio", directPayment.LevelPriceId);
            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(directPayment.CompanyId), "ProfessionalId", "FullName", directPayment.ProfessionalId);
            ViewBag.ServiceId = new SelectList(ComboHelper.GetServices(directPayment.CompanyId), "ServiceId", "Servicio", directPayment.ServiceId);
            ViewBag.ServiceCategoryId = new SelectList(ComboHelper.GetServicecategories(directPayment.CompanyId), "ServiceCategoryId", "Categoria", directPayment.ServiceCategoryId);

            return View(directPayment);
        }

        // POST: DirectPayments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DirectPayment directPayment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(directPayment).State = EntityState.Modified;
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

            ViewBag.ClientId = new SelectList(ComboHelper.GetClients(directPayment.CompanyId), "ClientId", "Cliente", directPayment.ClientId);
            ViewBag.LevelPriceId = new SelectList(ComboHelper.GetLevelPrice(), "LevelPriceId", "NivelPrecio", directPayment.LevelPriceId);
            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(directPayment.CompanyId), "ProfessionalId", "FullName", directPayment.ProfessionalId);
            ViewBag.ServiceId = new SelectList(ComboHelper.GetServices(directPayment.CompanyId), "ServiceId", "Servicio", directPayment.ServiceId);
            ViewBag.ServiceCategoryId = new SelectList(ComboHelper.GetServicecategories(directPayment.CompanyId), "ServiceCategoryId", "Categoria", directPayment.ServiceCategoryId);

            return View(directPayment);
        }

        // GET: DirectPayments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectPayment directPayment = db.DirectPayments.Find(id);
            if (directPayment == null)
            {
                return HttpNotFound();
            }
            return View(directPayment);
        }

        // POST: DirectPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DirectPayment directPayment = db.DirectPayments.Find(id);
            db.DirectPayments.Remove(directPayment);
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
            return View(directPayment);
        }

        public JsonResult GetServices(int CategoryId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var servicios = db.Services.Where(c => c.ServiceCategoryId == CategoryId);
            return Json(servicios);
        }

        public JsonResult GetProfesional(int ProfessionalId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var profesional = db.Professionals.Find(ProfessionalId);
            decimal tasa = profesional.Rate;

            return Json(tasa);
        }

        public JsonResult GetPrecio(int ServiceId, int LevelPriceId)
        {
            decimal precio = 0;

            db.Configuration.ProxyCreationEnabled = false;
            if (ServiceId == 0)
            {
                return Json(precio);
            }
            var servicios = db.Services.Find(ServiceId);
            int Lprice = LevelPriceId;
            
            if (Lprice == 1)
            {
                precio = servicios.Precio1;
            }
            if (Lprice == 2)
            {
                precio = servicios.Precio2;
            }
            if (Lprice == 3)
            {
                precio = servicios.Precio3;
            }

            return Json(precio);
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
