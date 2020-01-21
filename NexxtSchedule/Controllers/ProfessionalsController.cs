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

    public class ProfessionalsController : Controller
    {
        private NexxtCalContext db = new NexxtCalContext();

        // GET: Professionals
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var professionals = db.Professionals.Where(c => c.CompanyId == user.CompanyId)
                .Include(p => p.Identification);

            return View(professionals.ToList());
        }

        // GET: Professionals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var professional = db.Professionals.Find(id);

            if (professional == null)
            {
                return HttpNotFound();
            }
            return View(professional);
        }

        // GET: Professionals/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var profesional = new Professional
            {
                CompanyId = user.CompanyId,
                Activo = true
            };

            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(user.CompanyId), "IdentificationId", "TipoDocumento", (0));

            return View(profesional);
        }

        // POST: Professionals/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Professional professional)
        {
            if (ModelState.IsValid)
            {
                db.Professionals.Add(professional);
                try
                {
                    db.SaveChanges();
                    UsersHelper.CreateUserASP(professional.UserName, "Profe");

                    if (professional.PhotoFile != null)
                    {
                        var folder = "~/Content/Professionals";
                        var file = string.Format("{0}.jpg", professional.ProfessionalId);
                        var response = FilesHelper.UploadPhoto(professional.PhotoFile, folder, file);

                        if (response)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            professional.Photo = pic;
                            db.Entry(professional).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    //Unir el primer y ultimo nombre para nombre completo
                    var db33 = new NexxtCalContext();
                    var profesional = db33.Professionals.Find(professional.ProfessionalId);
                    profesional.FullName = professional.FirstName + " " + professional.LastName;
                    db33.Entry(profesional).State = EntityState.Modified;
                    db33.SaveChanges();
                    db33.Dispose();
                    //--------------------------------------------------------------
                    //..............................................................

                    var db3 = new NexxtCalContext();
                    var usuario = new User
                    {
                        UserName = professional.UserName,
                        FirstName = professional.FirstName,
                        LastName = professional.LastName,
                        Phone = professional.Phone,
                        Address = professional.Address,
                        Puesto = "Profe",
                        CompanyId = professional.CompanyId
                    };
                    db3.Users.Add(usuario);
                    db3.SaveChanges();
                    db3.Dispose();

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

            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(professional.CompanyId), "IdentificationId", "TipoDocumento", professional.IdentificationId);
            
            return View(professional);
        }

        // GET: Professionals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var professional = db.Professionals.Find(id);
            if (professional == null)
            {
                return HttpNotFound();
            }

            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(professional.CompanyId), "IdentificationId", "TipoDocumento", professional.IdentificationId);
            
            return View(professional);
        }

        // POST: Professionals/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Professional professional)
        {
            if (ModelState.IsValid)
            {
                db.Entry(professional).State = EntityState.Modified;
                try
                {
                    if (professional.PhotoFile != null)
                    {
                        var pic = string.Empty;
                        var folder = "~/Content/Professionals";
                        var file = string.Format("{0}.jpg", professional.ProfessionalId);
                        var response = FilesHelper.UploadPhoto(professional.PhotoFile, folder, file);

                        if (response)
                        {
                            pic = string.Format("{0}/{1}", folder, file);
                            professional.Photo = pic;
                        }
                    }

                    if (professional.Activo == true)
                    {
                        var db2 = new NexxtCalContext();
                        var currentTech = db2.Professionals.Find(professional.ProfessionalId);
                        if (currentTech.UserName != professional.UserName)
                        {
                            var db3 = new NexxtCalContext();
                            var usuarios = db3.Users.Where(c => c.UserName == currentTech.UserName && c.FirstName == currentTech.FirstName && c.LastName == currentTech.LastName).FirstOrDefault();
                            if (usuarios != null)
                            {
                                usuarios.UserName = professional.UserName;
                                db3.Entry(usuarios).State = EntityState.Modified;
                                db3.SaveChanges();
                                db3.Dispose();
                            }
                            else
                            {
                                var db4 = new NexxtCalContext();
                                var usuario = new User
                                {
                                    UserName = professional.UserName,
                                    FirstName = professional.FirstName,
                                    LastName = professional.LastName,
                                    Phone = professional.Phone,
                                    Address = professional.Address,
                                    Puesto = "Profe",
                                    CompanyId = professional.CompanyId
                                };
                                db4.Users.Add(usuario);
                                db4.SaveChanges();
                                db4.Dispose();
                                UsersHelper.CreateUserASP(professional.UserName, "Profe");
                            }
                            UsersHelper.UpdateUserName(currentTech.UserName, professional.UserName);
                        }
                        else
                        {
                            var db3 = new NexxtCalContext();
                            var usuarios = db3.Users.Where(c => c.UserName == currentTech.UserName && c.FirstName == currentTech.FirstName && c.LastName == currentTech.LastName).FirstOrDefault();
                            if (usuarios != null)
                            {
                                usuarios.UserName = professional.UserName;
                                db3.Entry(usuarios).State = EntityState.Modified;
                                db3.SaveChanges();
                                db3.Dispose();
                            }
                            else
                            {
                                var db4 = new NexxtCalContext();
                                var usuario = new User
                                {
                                    UserName = professional.UserName,
                                    FirstName = professional.FirstName,
                                    LastName = professional.LastName,
                                    Phone = professional.Phone,
                                    Address = professional.Address,
                                    Puesto = "Profe",
                                    CompanyId = professional.CompanyId
                                };
                                db4.Users.Add(usuario);
                                db4.SaveChanges();
                                db4.Dispose();
                                UsersHelper.CreateUserASP(professional.UserName, "Profe");
                            }
                            UsersHelper.UpdateUserName(currentTech.UserName, professional.UserName);
                        }
                    }

                    if (professional.Activo == false)
                    {
                        var db5 = new NexxtCalContext();
                        var currentTech2 = db5.Professionals.Find(professional.ProfessionalId);

                        var db6 = new NexxtCalContext();
                        var usuarios = db6.Users.Where(c => c.UserName == currentTech2.UserName && c.FirstName == currentTech2.FirstName && c.LastName == currentTech2.LastName).FirstOrDefault();
                        if (usuarios != null)
                        {
                            db6.Users.Remove(usuarios);
                            db6.SaveChanges();
                            db6.Dispose();
                        }
                        UsersHelper.DeleteUser(professional.UserName, "Profe");
                        //if (currentTech2.UserName != technical.UserName)
                        //{
                        //    UsersHelper.UpdateUserName(currentTech2.UserName, technical.UserName);
                        //}

                        db5.Dispose();
                    }

                    db.SaveChanges();

                    //Unir el primer y ultimo nombre para nombre completo
                    var db33 = new NexxtCalContext();
                    var profesional = db33.Professionals.Find(professional.ProfessionalId);
                    profesional.FullName = professional.FirstName + " " + professional.LastName;
                    db33.Entry(profesional).State = EntityState.Modified;
                    db33.SaveChanges();
                    db33.Dispose();
                    //--------------------------------------------------------------
                    //..............................................................
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

            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(professional.CompanyId), "IdentificationId", "TipoDocumento", professional.IdentificationId);
            return View(professional);
        }

        // GET: Professionals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var professional = db.Professionals.Find(id);

            if (professional == null)
            {
                return HttpNotFound();
            }
            return View(professional);
        }

        // POST: Professionals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var professional = db.Professionals.Find(id);
            db.Professionals.Remove(professional);
            try
            {
                db.SaveChanges();
                UsersHelper.DeleteUser(professional.UserName, "Profe");

                var db6 = new NexxtCalContext();
                var profesional = db6.Users.Where(c => c.UserName == professional.UserName && c.FirstName == professional.FirstName && c.LastName == professional.LastName).FirstOrDefault();
                if (profesional != null)
                {
                    db6.Users.Remove(profesional);
                    db6.SaveChanges();
                    db6.Dispose();
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    ModelState.AddModelError(string.Empty, @Resources.Resource.Msg_Relationship);                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(professional);
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
