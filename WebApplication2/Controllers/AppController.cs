using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class AppController : Controller
    {
        // GET: App
        public ActionResult Index()
        {
            return View();
        }

        /*        // POST: App/Users
                public JsonResult getUsers()
                {
                    List<ApplicationUser> retVal = new List<ApplicationUser>();

                    ApplicationDbContext dbCtx = ApplicationDbContext.Create();

                    retVal = dbCtx.Users.ToList();

                    return Json(retVal, JsonRequestBehavior.AllowGet);
                }
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Search(CinemaViewModel model)
        {
            
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var locations = ctx.Locations.ToList();
            List<Location> locToShow = new List<Location>();
            if (model.Type.Equals(LocationType.CINEMA))
            {
                foreach (Location l in locations)
                {
                    if (model.Name != null)
                    {
                        if (l.LocType.Equals(LocationType.CINEMA) && l.Name.ToLower().Contains(model.Name.ToLower()))
                        {
                            locToShow.Add(l);
                        }
                    }else
                    {
                        locToShow.Add(l);
                    }
                }
                ViewBag.type = LocationType.CINEMA;
                ViewBag.Message = "Cinemas";
            }else
            {
                foreach (Location l in locations)
                {
                    if (model.Name != null)
                    {
                        if (l.LocType.Equals(LocationType.THEATRE) && l.Name.ToLower().Contains(model.Name.ToLower()))
                        {
                            locToShow.Add(l);
                        }
                    }else
                    {
                        locToShow.Add(l);
                    }
                }
                ViewBag.type = LocationType.THEATRE;
                ViewBag.Message = "Theatres";
            }
            ViewBag.locations = locToShow;

            return View("ShowCinemas", model);
        }

        public ActionResult ShowCinemas()
        {
            ViewBag.Message = "Cinemas ";
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
            ViewBag.type = LocationType.CINEMA;
            ViewBag.locations = cinemas;
            CinemaViewModel cvm = new CinemaViewModel
            {
                Type = LocationType.CINEMA
            };
            return View(cvm);
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
            ViewBag.type = LocationType.THEATRE;
            ViewBag.locations = theatres;
            CinemaViewModel cvm = new CinemaViewModel
            {
                Type = LocationType.THEATRE
            };
            return View("ShowCinemas",cvm);
        }

        public ActionResult getCinemas()
        {
            List<Location> retVal = new List<Location>();

            ApplicationDbContext dbCtx = ApplicationDbContext.Create();

            retVal = dbCtx.Locations.ToList();

            return View(retVal);
        }

        public ActionResult SortLocationsByName(String name, LocationType type)
        {
            if (type.Equals(LocationType.CINEMA))
            {
                ViewBag.Message = "Cinemas ";
                List<Location> retVal = new List<Location>();

                ApplicationDbContext dbCtx = ApplicationDbContext.Create();

                retVal = dbCtx.Locations.ToList();
                List<Location> cinemas = new List<Location>();
                foreach (Location loc in retVal)
                {
                    if (loc.LocType == LocationType.CINEMA)
                    {
                        cinemas.Add(loc);
                    }
                }
                List<Location> SortedList = cinemas.OrderBy(o => o.Name).ToList();
                ViewBag.type = LocationType.CINEMA;
                ViewBag.locations = SortedList;
                CinemaViewModel cvm = new CinemaViewModel
                {
                    Type = LocationType.CINEMA
                };
                return View("ShowCinemas", cvm);
            }
            else
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
                List<Location> SortedList = theatres.OrderBy(o => o.Name).ToList();
                ViewBag.type = LocationType.THEATRE;
                ViewBag.locations = SortedList;
                CinemaViewModel cvm = new CinemaViewModel
                {
                    Type = LocationType.THEATRE
                };
                return View("ShowCinemas", cvm);
            }
        }
        public ActionResult SortLocationsByCity(String name, LocationType type)
        {
            if (type.Equals(LocationType.CINEMA))
            {
                ViewBag.Message = "Cinemas ";
                List<Location> retVal = new List<Location>();

                ApplicationDbContext dbCtx = ApplicationDbContext.Create();

                retVal = dbCtx.Locations.ToList();
                List<Location> cinemas = new List<Location>();
                foreach (Location loc in retVal)
                {
                    if (loc.LocType == LocationType.CINEMA)
                    {
                        cinemas.Add(loc);
                    }
                }
                List<Location> SortedList = cinemas.OrderBy(o => o.City).ToList();
                ViewBag.type = LocationType.CINEMA;
                ViewBag.locations = SortedList;
                CinemaViewModel cvm = new CinemaViewModel
                {
                    Type = LocationType.CINEMA
                };
                return View("ShowCinemas", cvm);
            }
            else
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
                List<Location> SortedList = theatres.OrderBy(o => o.City).ToList();
                ViewBag.type = LocationType.THEATRE;
                ViewBag.locations = SortedList;
                CinemaViewModel cvm = new CinemaViewModel
                {
                    Type = LocationType.THEATRE
                };
                return View("ShowCinemas", cvm);
            }
        }
    }
}