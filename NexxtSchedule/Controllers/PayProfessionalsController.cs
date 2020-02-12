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

    public class PayProfessionalsController : Controller
    {
        private NexxtCalContext db = new NexxtCalContext();

        // GET: PayProfessionals/Edit/5
        public ActionResult Adds(int idPay, int idProfe, int idCompany, string NPago) //el Id es PayProfessionalId
        {
            var directgenerallist = db.DirectGenerals.Where(c => c.CompanyId == idCompany && c.ProfessionalId == idProfe && c.Facturado == false).ToList();
            if (directgenerallist.Count != 0)
            {
                var db2 = new NexxtCalContext();
                var db3 = new NexxtCalContext();
                var db4 = new NexxtCalContext();
                decimal sum = 0;         //To save all the payprofessional and can save it in PayProfessional
                string sumTickets = null;
                foreach (var item in directgenerallist)
                {

                    var NewDetail = new PayProfessionalDetails
                    {
                        PayProfessionalId = idPay,
                        CompanyId = idCompany,
                        DirectGeneralId = item.DirectGeneralId,
                        Date = item.Date,
                        NotaCobro = item.NotaCobro,
                        PagoProfesional = item.PagoProfesional
                    };

                    sum += item.PagoProfesional;
                    if (sumTickets == null)
                    {
                        sumTickets = item.NotaCobro;
                    }
                    else
                    {
                        sumTickets = sumTickets + " - " + item.NotaCobro;
                    }   
                    db2.PayProfessionalsDetails.Add(NewDetail);

                    var DirectGeneralUpdate = db3.DirectGenerals.Find(item.DirectGeneralId);
                    DirectGeneralUpdate.Facturado = true;
                    DirectGeneralUpdate.FacturadoDate = DateTime.Today;
                    DirectGeneralUpdate.ComprobantePago = NPago;
                }
                var payprofessionalUpdate = db4.PayProfessionals.Find(idPay);
                payprofessionalUpdate.PagoProfesional = sum;
                payprofessionalUpdate.Detalle = sumTickets;
                db4.Entry(payprofessionalUpdate).State = EntityState.Modified;
                db4.SaveChanges();
                
                db2.SaveChanges();
                db3.SaveChanges();
                db2.Dispose();
                db3.Dispose();
                db4.Dispose();
            }

            return RedirectToAction("Details", new { id = idPay});
        }


        // GET: PayProfessionals
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var payProfessionals = db.PayProfessionals.Where(c => c.CompanyId == user.CompanyId)
                .Include(p => p.Professional);

            return View(payProfessionals.ToList());
        }

        // GET: PayProfessionals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var payProfessional = db.PayProfessionals.Find(id);
            if (payProfessional == null)
            {
                return HttpNotFound();
            }
            return View(payProfessional);
        }

        // GET: PayProfessionals/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var payprofessoinal = new PayProfessional
            {
                CompanyId = user.CompanyId,
                Date = DateTime.Today
            };

            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(user.CompanyId), "ProfessionalId", "FullName");

            return View(payprofessoinal);
        }

        // POST: PayProfessionals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PayProfessional payProfessional)
        {
            if (ModelState.IsValid)
            {
                //Chech for register in the table DirectGeneral
                var db2 = new NexxtCalContext();
                var proregister = db2.DirectGenerals.Where(p => p.ProfessionalId == payProfessional.ProfessionalId && p.Facturado == false).ToList();
                if (proregister.Count == 0)
                {
                    db2.Dispose();
                    ModelState.AddModelError(string.Empty, @Resources.Resource.Msg_NoRegisterToContinue);
                    ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(payProfessional.CompanyId), "ProfessionalId", "FirstName", payProfessional.ProfessionalId);
                    return View(payProfessional);
                }
                db2.Dispose();
                // End the Chekout register in Table DirectGeneral

                //Create a New Register consecutive to Payment Note
                var db3 = new NexxtCalContext();
                int Recep = 0;
                int sum = 0;
                var register = db3.Registers.Where(c => c.CompanyId == payProfessional.CompanyId).FirstOrDefault();
                Recep = register.Compobantepago;
                sum = Recep + 1;
                register.Compobantepago = sum;
                db3.Entry(register).State = EntityState.Modified;
                db3.SaveChanges();
                db3.Dispose();
                //Close the new register

                payProfessional.NotaPago = Convert.ToString(sum);
                db.PayProfessionals.Add(payProfessional);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = payProfessional.PayProfessionalId});
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

            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(payProfessional.CompanyId), "ProfessionalId", "FirstName", payProfessional.ProfessionalId);
            return View(payProfessional);
        }

        // GET: PayProfessionals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var payProfessional = db.PayProfessionals.Find(id);
            if (payProfessional == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(payProfessional.CompanyId), "ProfessionalId", "FirstName", payProfessional.ProfessionalId);
            return View(payProfessional);
        }

        // POST: PayProfessionals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PayProfessional payProfessional)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payProfessional).State = EntityState.Modified;
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

            ViewBag.ProfessionalId = new SelectList(ComboHelper.GetProfessional(payProfessional.CompanyId), "ProfessionalId", "FirstName", payProfessional.ProfessionalId);
            return View(payProfessional);
        }

        // GET: PayProfessionals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var payProfessional = db.PayProfessionals.Find(id);
            if (payProfessional == null)
            {
                return HttpNotFound();
            }
            return View(payProfessional);
        }

        // POST: PayProfessionals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PayProfessional payProfessional = db.PayProfessionals.Find(id);
            db.PayProfessionals.Remove(payProfessional);
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
            return View(payProfessional);
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
