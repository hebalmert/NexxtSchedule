﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NexxtSchedule.Classes;
using NexxtSchedule.Models;
using PagedList;

namespace NexxtSchedule.Controllers
{
    [Authorize(Roles = "User, Profe")]

    public class ClientsController : Controller
    {
        private readonly NexxtCalContext db = new NexxtCalContext();

        [HttpPost]
        public JsonResult Search(string Prefix)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            var Iclient = (from cliente in db.Clients
                           where cliente.Cliente.Contains(Prefix) && cliente.CompanyId == user.CompanyId
                           select new
                           {
                               label = cliente.Cliente,
                               val = cliente.ClientId
                           }).ToList();

            return Json(Iclient);

        }

        // GET: Clients
        public ActionResult Index(int? clientid, int? page = null)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            page = (page ?? 1);

            if (clientid != null)
            {
                var clients = db.Clients.Where(c => c.CompanyId == user.CompanyId && c.ClientId == clientid)
                      .Include(c => c.Identification);

                return View(clients.OrderBy(o => o.Cliente).ToList().ToPagedList((int)page, 25));
            }
            else
            {
                var clients = db.Clients.Where(c => c.CompanyId == user.CompanyId)
                      .Include(c => c.Identification);

                return View(clients.OrderBy(o => o.Cliente).ToList().ToPagedList((int)page, 25));
            }
        }

        // GET: Clients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var client = new Client
            {
                CompanyId = user.CompanyId,
                Nacimiento = DateTime.Today
            };

            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(user.CompanyId), "IdentificationId", "TipoDocumento", (0));

            return View(client);
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Client client)
        {
            if (client.FirstName == null || client.LastName == null)
            {
                {
                    ModelState.AddModelError(string.Empty, ("Es Obligatorio el Nombre y el Apellido"));
                    ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(client.CompanyId), "IdentificationId", "TipoDocumento", client.IdentificationId);

                    return View(client);
                }
            }

            client.Cliente = client.FirstName + " " + client.LastName;
            if (ModelState.IsValid)
            {      
                db.Clients.Add(client);
                try
                {
                    if (client.FirstName == null || client.LastName == null)
                    {
                        ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_ErrorNameLastName));
                        ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(client.CompanyId), "IdentificationId", "TipoDocumento", client.IdentificationId);
                        return View(client);
                    }

                    await db.SaveChangesAsync();
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

            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(client.CompanyId), "IdentificationId", "TipoDocumento", client.IdentificationId);

            return View(client);
        }

        // GET: Clients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }

            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(client.CompanyId), "IdentificationId", "TipoDocumento", client.IdentificationId);
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Client client)
        {
            if (client.FirstName == null || client.LastName == null)
            {
                {
                    ModelState.AddModelError(string.Empty, ("Es Obligatorio el Nombre y el Apellido"));
                    ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(client.CompanyId), "IdentificationId", "TipoDocumento", client.IdentificationId);
                    
                    return View(client);
                }
            }

            client.Cliente = client.FirstName + " " + client.LastName;
            if (ModelState.IsValid)
            {
                if (client.FirstName == null || client.LastName == null)
                {
                    ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_ErrorNameLastName));
                    ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(client.CompanyId), "IdentificationId", "TipoDocumento", client.IdentificationId);
                    return View(client);
                }

                db.Entry(client).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("_Index"))
                    {
                        ModelState.AddModelError(string.Empty, (@Resources.Resource.Msg_DoubleData));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
            }

            ViewBag.IdentificationId = new SelectList(ComboHelper.GetIdentifications(client.CompanyId), "IdentificationId", "TipoDocumento", client.IdentificationId);
            return View(client);
        }

        // GET: Clients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var client = db.Clients.Find(id);
            db.Clients.Remove(client);
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
            return View(client);
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
