using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebApplication2.Models;
using System.Threading.Tasks;
using System.Text;

namespace WebApplication2.Controllers
{
    public class ReservationController : Controller
    {
        // GET: Reservation
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost] 
        public async Task<ActionResult> Test(String[] arr)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            Guid id = new Guid(arr[0]);

            var mama = await dbCtx.HallTimeProjection.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == id);
            var saHalom = await dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefaultAsync(x => x.Id == mama.Id);
            var indexRow = -1;
            var indexColumn = -1;
            if (arr.Length > 1)
            {
                for(int i = 1; i < arr.Length; i++)
                {
                    string[] redKolona = arr[i].Split('_');
                    int.TryParse(redKolona[0],out indexRow);
                    int.TryParse(redKolona[1], out indexColumn);
                  //  indexColumn = int.Parse(redKolona[1]) - 1;

                    if (indexRow != -1 && indexColumn != -1)
                    {
                        Row red = saHalom.Seats[indexRow-1];
                        
                        var aStringBuilder = new StringBuilder(red.Seats);
                        aStringBuilder.Remove(indexColumn-1, 1);
                        aStringBuilder.Insert(indexColumn-1, "f");
                        red.Seats = aStringBuilder.ToString();
                        saHalom.Seats[indexRow-1] = red;
                        indexRow = -1;
                        indexColumn = -1;
                    }
                }
            }
            dbCtx.SaveChanges();
            bool isValid = true; //.. check
            var obj = new
            {
                valid = isValid
            };
            return View("Seats", saHalom);
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
            var projs = new List<Projection>();
            var projHalls = new List<HallTimeProjection>();
            foreach (Projection p in projections)
            {
                Projection proj = new Projection();
                proj = dbCtx.Projections.Include(x => x.ProjHallsTimeList).FirstOrDefault(x => x.Id == p.Id);
                foreach(HallTimeProjection htp in proj.ProjHallsTimeList)
                {
                    HallTimeProjection projHall = new HallTimeProjection();
                    projHall = dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefault(x => x.Id == htp.Id);
                    projHalls.Add(projHall);
                }
                proj.ProjHallsTimeList = projHalls;
                projs.Add(proj);
            }
            locationToShow.ProjectionsList = projs;
            return View("ShowRepertoar", locationToShow);
        }

        public async Task<ActionResult> ViewReservation(Guid projectionId , Guid projHall)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            List<HallTimeProjection> timeProjections = new List<HallTimeProjection>();
            HallTimeProjection hallTimeProj = new HallTimeProjection();
            foreach(HallTimeProjection htp in timeProjections)
            {
                if (htp.Id.Equals(projHall))
                {
                    hallTimeProj = htp;
                    break;
                }
            }
            var mama = await dbCtx.HallTimeProjection.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == projHall);
            var saHalom = await dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefaultAsync(x => x.Id == mama.Id);
            return View("Seats", saHalom);
        }
    }
}