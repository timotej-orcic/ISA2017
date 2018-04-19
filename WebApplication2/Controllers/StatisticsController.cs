using Isa2017Cinema.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebApplication2.Models;


namespace WebApplication2.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ShowIncome(GraphicViewModel model)
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            string id = User.Identity.GetUserId();
            var locationToShow = new Location();
            var locationId = ctx.Database.SqlQuery<String>("select MyLocationId from AspNetUsers where id = '" + id + "'").FirstOrDefault();
            Guid idLoc = new Guid(locationId);
            List<Location> allLocations = new List<Location>();
            var projections = ctx.Database.SqlQuery<Projection>("select * from Projections where Location_Id = '" + idLoc + "'").ToList();
            allLocations = ctx.Locations.ToList();
            foreach (Location loc in allLocations)
            {
                if (loc.Id.Equals(locationId))
                {
                    locationToShow = loc;
                }
            }
            locationToShow.ProjectionsList = projections;
            List<Ticket> tickets = new List<Ticket>();
            List<Ticket> ticketsToCalculate = new List<Ticket>();
            tickets = ctx.Reservations.ToList();
            Ticket ticket = new Ticket();
            string time = "00:00 PM";
            string vreme1 = model.DatumOd + " " + time;
            DateTime datum1 = DateTime.Parse(vreme1);
            string vreme2 = model.DatumDo + " " + time;
            DateTime datum2 = DateTime.Parse(vreme2);
            foreach (Ticket t in tickets)
            {
                ticket = await ctx.Reservations.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == t.Id);
                HallTimeProjection pr = new HallTimeProjection();
                pr = await ctx.HallTimeProjection.Include(x => x.Hall).FirstOrDefaultAsync(x => x.Id == ticket.Projection.Id);

                if (pr.Hall.ParentLocation.Id.Equals(new Guid(locationId)) && DateTime.Compare(pr.Time,datum1)>0 && DateTime.Compare(pr.Time, datum2) < 0)
                {
                    ticketsToCalculate.Add(ticket);
                }
            }
            double prihod = 0;
            foreach(Ticket t in ticketsToCalculate)
            {
                prihod += t.Price * t.DiscountMultiplier;
            }
            ViewBag.prihod = prihod;
            return await ShowStatistics();
        }

        public async Task<ActionResult> ShowStatistics()
        {
            
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            string id = User.Identity.GetUserId();
            var locationToShow = new Location();
            var locationId = ctx.Database.SqlQuery<String>("select MyLocationId from AspNetUsers where id = '" + id + "'").FirstOrDefault();
            Guid idLoc = new Guid(locationId);
            List<Location> allLocations = new List<Location>();
            var projections = ctx.Database.SqlQuery<Projection>("select * from Projections where Location_Id = '" + idLoc + "'").ToList();
            allLocations = ctx.Locations.ToList();
            foreach (Location loc in allLocations)
            {
                if (loc.Id.Equals(locationId))
                {
                    locationToShow = loc;
                }
            }
            locationToShow.ProjectionsList = projections;
            List<Ticket> tickets = new List<Ticket>();
            List<Ticket> ticketsToCalculate = new List<Ticket>();
            tickets = ctx.Reservations.ToList();
            Ticket ticket = new Ticket();
            foreach (Ticket t in tickets) {
                ticket = await ctx.Reservations.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == t.Id);
                HallTimeProjection pr = new HallTimeProjection();
                pr = await ctx.HallTimeProjection.Include(x => x.Hall).FirstOrDefaultAsync(x => x.Id == ticket.Projection.Id);

                if (pr.Hall.ParentLocation.Id.Equals(new Guid(locationId)))
                {
                    ticketsToCalculate.Add(ticket);
                }
            }
            List<Pair> parovi = new List<Pair>();
            foreach(Ticket t in ticketsToCalculate)
            {
                Pair par = new Pair();
                par.date = t.Projection.Time.ToString();
                par.date = par.date.Split(' ')[0];
                DateTime dt = DateTime.Now;
                int trenutnidan = dt.DayOfYear;
                int trenutnagod = dt.Year;
                par.numberofday = t.Projection.Time.DayOfYear;
                par.year = t.Projection.Time.Year;
                par.number = 0;
                if(t.Projection.Time.DayOfYear < 105)
                {
                    int k = 8;
                }
                int ima = 1;
                for(int i =0; i<parovi.Count; i++)
                {
                    if(parovi[i].numberofday.Equals(par.numberofday) && parovi[i].year.Equals(par.year) && trenutnidan-par.numberofday < 7 && trenutnagod - par.year == 0 )
                    {
                        ima = 0;
                    }
                }
                if(ima == 1)
                {
                    if(trenutnidan - par.numberofday < 7)
                        parovi.Insert(0,par);
                }
                ima = 1;
                
            }
            foreach (Ticket t in ticketsToCalculate)
            {
                for(int i=0; i< parovi.Count; i++)
                {
                    if (t.Projection.Time.DayOfYear.Equals(parovi[i].numberofday) && t.Projection.Time.Year.Equals(parovi[i].year))
                    {
                        parovi[i].number++;
                        break;
                    }
                }
            }
            List<Pair> nedeljno = new List<Pair>();
            int b = 0;
            for (int i=0; i < 12; i++)
            {
                Pair par = new Pair();
                int p = b + 2;
                par.date = b + "-" + p;
                par.number = 0;
                DateTime now = DateTime.Now;
                par.year = now.Year;
                par.numberofday = now.DayOfYear;
                nedeljno.Add(par);
                b += 2;
            }
            
           
            foreach (Ticket t in ticketsToCalculate)
            {
                for (int i = 0; i < nedeljno.Count; i++)
                {
                    if (t.Projection.Time.DayOfYear.Equals(nedeljno[i].numberofday) && t.Projection.Time.Year.Equals(nedeljno[i].year))
                    {
                        int donja = 44;
                        int.TryParse(nedeljno[i].date.Split('-')[0], out donja);
                        int gornja = 44;
                        int.TryParse(nedeljno[i].date.Split('-')[1], out gornja);
                        if (t.Projection.Time.Hour >= donja && t.Projection.Time.Hour < gornja ){
                           nedeljno[i].number++;
                            break;
                        }
                      
                    }
                }
            }
            List<Pair2> mesecno = new List<Pair2>();
            foreach (Ticket t in ticketsToCalculate)
            {
                Pair2 par = new Pair2();
                par.date = t.Projection.Time.ToString();
                par.date = par.date.Split(' ')[0];
                DateTime dt = DateTime.Now;
                int trenutnidan = dt.DayOfYear;
                int trenutnagod = dt.Year;
                par.numberofday = t.Projection.Time.DayOfYear;
                par.numberofmonth = t.Projection.Time.Month;
                int trenutnimesec = dt.Month;
                par.year = t.Projection.Time.Year;
                par.number = 0;
                int ima = 1;
                for (int i = 0; i < mesecno.Count; i++)
                {
                    if (mesecno[i].numberofday.Equals(par.numberofday) && mesecno[i].year.Equals(par.year) && trenutnimesec - par.numberofmonth ==0  && trenutnagod - par.year == 0)
                    {
                        ima = 0;
                    }
                }
                if (ima == 1)
                {
                    if(trenutnimesec - par.numberofmonth ==0)
                        mesecno.Insert(0,par);
                }
                ima = 1;

            }
            foreach (Ticket t in ticketsToCalculate)
            {
                for (int i = 0; i < mesecno.Count; i++)
                {
                    if (t.Projection.Time.DayOfYear.Equals(mesecno[i].numberofday) && t.Projection.Time.Year.Equals(mesecno[i].year))
                    {
                        mesecno[i].number++;
                        break;
                    }
                }
            }



            GraphicViewModel graph = new GraphicViewModel
            {
                loc = locationToShow,
                pairs = parovi,
                nedeljni = nedeljno,
                mesecni = mesecno
            };
            if(ViewBag.prihod == null)
                ViewBag.prihod = 0;
            return View("ShowStatistics" , graph);
        }
    }
}