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

            return Json(retVal, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ShowCinemas()
        {
            ViewBag.Message = "Cinemas :";
            List<Location> retVal = new List<Location>();

            ApplicationDbContext dbCtx = ApplicationDbContext.Create();

            retVal = dbCtx.Locations.ToList();
            List<Location> cinemas = new List<Location>();
            foreach(Location loc in retVal)
            {
                if(loc.LocType == LocationType.CINEMA)
                {
                    cinemas.Add(loc);
                }
            }
            return View(cinemas);
        }

        public ActionResult ShowTheatres()
        {
            ViewBag.Message = "Theatres :";
            List<Location> retVal = new List<Location>();

            ApplicationDbContext dbCtx = ApplicationDbContext.Create();

            retVal = dbCtx.Locations.ToList();
            List<Location> theatres = new List<Location>();
            foreach (Location loc in retVal)
            {
                if (loc.LocType == LocationType.THEATRE)
                {
                    theatres.Add(loc);
                }
            }
            return View("ShowCinemas",theatres);
        }
        public ActionResult getCinemas()
        {
            List<Location> retVal = new List<Location>();

            ApplicationDbContext dbCtx = ApplicationDbContext.Create();

            retVal = dbCtx.Locations.ToList();

            return View(retVal);
        }
    }
}