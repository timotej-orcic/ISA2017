using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace WebApplication2.Controllers
{
    public class AppController : Controller
    {
        // GET: App
        public ActionResult Index()
        {
            return View();
        }

        // POST: App/Users
        public JsonResult getUsers()
        {
            List<ApplicationUser> retVal = new List<ApplicationUser>();

            ApplicationDbContext dbCtx = ApplicationDbContext.Create();

            retVal = dbCtx.Users.ToList();

            return Json(retVal);
        }
    }
}