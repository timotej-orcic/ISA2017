using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class ReservationController : Controller
    {
        // GET: Reservation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewRepertoar(Guid locationId)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            var locationToShow = new Location();
           
            var projections = dbCtx.Database.SqlQuery<Projection>("select * from Projections where Location_Id = '" + locationId + "'").ToList();

            List<Location> allLocations = new List<Location>();
            
            allLocations = dbCtx.Locations.ToList();
            foreach (Location loc in allLocations)
            {
                if (loc.Id.Equals(locationId))
                {
                    locationToShow = loc;
                    break;
                }
            }
            locationToShow.ProjectionsList = projections;
            return View("ShowRepertoar", locationToShow);
        }
    }
}