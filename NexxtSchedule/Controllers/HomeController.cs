﻿using NexxtSchedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NexxtSchedule.Controllers
{
    public class HomeController : Controller
    {
        private NexxtCalContext db = new NexxtCalContext();

        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (user != null)
            {
                var db2 = new NexxtCalContext();
                var companyUp = db2.Companies.Find(user.CompanyId);
                bool comActivo = companyUp.Activo;
                db2.Dispose();

                if (comActivo == false)
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            return View(user);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}