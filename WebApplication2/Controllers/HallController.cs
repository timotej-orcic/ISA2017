using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class HallController : Controller
    {
        // GET: Hall
        public ActionResult Index()
        {
            return View();
        }

        

        public ActionResult ViewHall(Guid IdHall)
        {
            List<Hall> allHalls = new List<Hall>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allHalls = dbCtx.Halls.ToList();
            Hall hallToShow = new Hall();
            foreach(Hall hall in allHalls)
            {
                if (hall.Id.Equals(IdHall))
                {
                    hallToShow = hall;
                    break;
                }
            }

            return View("Seats", hallToShow);
        }

        public async Task<ActionResult> AddHallTimeProjection(Guid projekcija)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            List<Location> allLocations = new List<Location>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            var locationId = dbCtx.Database.SqlQuery<Guid>("select Location_Id from Projections where Id = '" + projekcija + "'").FirstOrDefault();
            var location = await dbCtx.Locations.Include(x => x.HallsList).FirstOrDefaultAsync(x => x.Id == locationId);


            List<string> halls = new List<string>();
            
           
            foreach (Hall h in location.HallsList)
            {
                items.Add(new SelectListItem { Text = h.Name });
                halls.Add(h.Name);
            }
            
            HallTimeViewModel halle = new HallTimeViewModel
            {
                Date = "",
                Hall = "",
                Hale = halls, 
                Time = ""
                
            };
            ViewBag.location = locationId;
            ViewBag.Halls = items;
            ViewBag.projekcija = projekcija;
            return View("AddHallTimeProjection", halle);
        }
        public async Task<ActionResult> HallTimeSubmit(Models.HallTimeViewModel model, Guid MyLocation, Guid projekcija)
        {
            
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            var location = await dbCtx.Locations.Include(x => x.HallsList).FirstOrDefaultAsync(x => x.Id == MyLocation);
            Hall h = new Hall();
            h = location.HallsList[0];
            var rows = h.RowsCount;
            var columns = h.ColsCount;
            string imehale = model.Hall ;
            foreach(Hall hall in location.HallsList)
            {
                if (hall.Name.Equals(imehale))
                {
                    h = hall;
                }
            }

            string date = model.Date;
            string time = model.Time;
            string datetime = date + " " + time;
            List<Row> seats = new List<Row>();
            
            string praznic = "";
            for(int i = 0; i< rows; i++)
            {
                Row row = new Row();
                for (int j = 0; j< columns; j++)
                {
                    praznic += "e";
                    
                }
                row.Seats = praznic;
                seats.Add(row);
                dbCtx.Rows.Add(row);
                praznic = "";
            }
            DateTime datum = DateTime.Parse(datetime);
            
            var projection = await dbCtx.Projections.Include(x => x.ProjHallsTimeList).FirstOrDefaultAsync(x => x.Id == projekcija);
            HallTimeProjection ToAdd = new HallTimeProjection
            {
                Hall = h,
                Time = datum,
                Seats = seats,
                Projection = projection

            };
            projection.ProjHallsTimeList.Add(ToAdd);
            dbCtx.HallTimeProjection.Add(ToAdd);
            dbCtx.SaveChanges();
            var mama = await dbCtx.HallTimeProjection.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == x.Id);

            ViewBag.location = MyLocation;
            List<Projection> allProjections = new List<Projection>();
            
            allProjections = dbCtx.Projections.ToList();
            Projection projectionForEdit = new Projection();
            foreach (Projection p in allProjections)
            {
                if (p.Id.Equals(projekcija))
                {
                    projectionForEdit = p;
                }
            }
            var projHalls = new List<HallTimeProjection>();
            Projection proj = new Projection();
            proj = dbCtx.Projections.Include(x => x.ProjHallsTimeList).FirstOrDefault(x => x.Id == projectionForEdit.Id);
            foreach (HallTimeProjection htp in proj.ProjHallsTimeList)
            {
                HallTimeProjection projHall = new HallTimeProjection();
                projHall = dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefault(x => x.Id == htp.Id);
                projHalls.Add(projHall);
            }
            proj.ProjHallsTimeList = projHalls;
            return View("../Location/ChangeProjection",proj);

        }
    }
}