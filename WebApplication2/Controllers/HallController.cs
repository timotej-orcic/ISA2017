using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebApplication2.Models;
using System.Text;
using Microsoft.AspNet.Identity;

namespace WebApplication2.Controllers
{
    public class HallController : Controller
    {
        // GET: Hall
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangeSeats(String[] arr)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            Guid id = new Guid(arr[0]);
            var hala = dbCtx.Database.SqlQuery<Hall>("select * from Halls where Id = '" + id + "'").FirstOrDefault();
            var mama = dbCtx.Halls.Include(x => x.Seats).FirstOrDefault(x => x.Id == id);
            
            var indexRow = -1;
            var indexColumn = -1;
            List<Ticket> ticketsList = new List<Ticket>();
            int slobodna = 0;
            int balkon = 0;
            for (int i =1; i<arr.Length; i++)
            {
                if (arr[i].Equals("razmak"))
                {
                    slobodna = i;
                }
                if (arr[i].Equals("razmak2"))
                {
                   balkon = i;
                }
            }
            if (arr.Length > 1)
            {
                for (int i = 1; i < slobodna - 1; i++)
                {
                    if (!arr[i].Equals("")) { 
                    string[] redKolona = arr[i].Split('_');
                    int.TryParse(redKolona[0], out indexRow);
                    int.TryParse(redKolona[1], out indexColumn);

                    if (indexRow != -1 && indexColumn != -1)
                    {
                        Row red = mama.Seats[indexRow - 1];
                        var aStringBuilder = new StringBuilder(red.Seats);
                        aStringBuilder.Remove(indexColumn - 1, 1);
                        aStringBuilder.Insert(indexColumn - 1, "o");
                        red.Seats = aStringBuilder.ToString();
                        mama.Seats[indexRow - 1] = red;
                        

                        indexRow = -1;
                        indexColumn = -1;
                    }
                    }
                }
                for (int i = slobodna+1; i < balkon-1; i++)
                {
                    if (!arr[i].Equals(""))
                    {
                        string[] redKolona = arr[i].Split('_');
                        int.TryParse(redKolona[0], out indexRow);
                        int.TryParse(redKolona[1], out indexColumn);

                        if (indexRow != -1 && indexColumn != -1)
                        {
                            Row red = mama.Seats[indexRow - 1];
                            var aStringBuilder = new StringBuilder(red.Seats);
                            aStringBuilder.Remove(indexColumn - 1, 1);
                            aStringBuilder.Insert(indexColumn - 1, "f");
                            red.Seats = aStringBuilder.ToString();
                            mama.Seats[indexRow - 1] = red;


                            indexRow = -1;
                            indexColumn = -1;
                        }
                    }
                }
                for (int i = balkon+1; i < arr.Length; i++)
                {
                    if (!arr[i].Equals(""))
                    {
                        string[] redKolona = arr[i].Split('_');
                        int.TryParse(redKolona[0], out indexRow);
                        int.TryParse(redKolona[1], out indexColumn);

                        if (indexRow != -1 && indexColumn != -1)
                        {
                            Row red = mama.Seats[indexRow - 1];
                            var aStringBuilder = new StringBuilder(red.Seats);
                            aStringBuilder.Remove(indexColumn - 1, 1);
                            aStringBuilder.Insert(indexColumn - 1, "e");
                            red.Seats = aStringBuilder.ToString();
                            mama.Seats[indexRow - 1] = red;


                            indexRow = -1;
                            indexColumn = -1;
                        }
                    }
                }
            }

            // var projekcija = dbCtx.Database.SqlQuery<Projection>("select * from Projections where MyLocation = '" + hala.Id + "'").FirstOrDefault();
         

            dbCtx.SaveChanges();
            
            var obj = new
            {
                isok = true
            };
            return Json(obj);
        }

        [HttpPost]
        public JsonResult SaveFastTickets(String[] arr)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            Guid id = new Guid(arr[0]);

            double discount = -2;
            double.TryParse(arr[1], out discount);

            discount = (double)(100-discount) / (double)100;
            var mama = dbCtx.HallTimeProjection.Include(x => x.Seats).FirstOrDefault(x => x.Id == id);
            var saHalom = dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefault(x => x.Id == mama.Id);

            var saProjekcijom = dbCtx.HallTimeProjection.Include(x => x.Projection).FirstOrDefault(x => x.Id == mama.Id);
            var indexRow = -1;
            var indexColumn = -1;
            List<Ticket> ticketsList = new List<Ticket>();
            if (arr.Length > 1)
            {
                for (int i = 2; i < arr.Length; i++)
                {
                    string[] redKolona = arr[i].Split('_');
                    int.TryParse(redKolona[0], out indexRow);
                    int.TryParse(redKolona[1], out indexColumn);

                    if (indexRow != -1 && indexColumn != -1)
                    {
                        Row red = saProjekcijom.Seats[indexRow - 1];
                        var aStringBuilder = new StringBuilder(red.Seats);
                        aStringBuilder.Remove(indexColumn - 1, 1);
                        aStringBuilder.Insert(indexColumn - 1, "f");
                        red.Seats = aStringBuilder.ToString();
                        saProjekcijom.Seats[indexRow - 1] = red;
                        Ticket newReservation = new Ticket
                        {
                            Projection = saProjekcijom,
                            SeatColumn = indexColumn - 1,
                            SeatRow = indexRow - 1,
                            Price = saProjekcijom.TicketPrice,
                            DiscountMultiplier = discount

                        };
                        string userIdString = User.Identity.GetUserId();
                        var loggedUser = dbCtx.Users.Include(x => x.ReservationsList).FirstOrDefault(x => x.Id == userIdString);
                     
                        loggedUser.ReservationsList.Add(newReservation);
                        ticketsList.Add(newReservation);
                        dbCtx.Reservations.Add(newReservation);
                       

                        indexRow = -1;
                        indexColumn = -1;
                    }
                }
            }
            dbCtx.SaveChanges();
            var obj = new {
                isok = true
            };
            return Json(obj);
        }

        public async Task<ActionResult> AddFastTicket(Guid projekcija , Guid timehall)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            var projection = dbCtx.Database.SqlQuery<Projection>("select * from Projections where Id = '" + projekcija + "'").FirstOrDefault();
            var projWithHalls = await dbCtx.Projections.Include(x => x.ProjHallsTimeList).FirstOrDefaultAsync(x => x.Id == projekcija);
            
            var mama = await dbCtx.HallTimeProjection.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == timehall);
            var saHalom = await dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefaultAsync(x => x.Id == mama.Id);
            var saProjekcijom = await dbCtx.HallTimeProjection.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == mama.Id);
            FastReserveTicket fastResTic = new FastReserveTicket
            {
                hallTimeProj = saProjekcijom,
            };
            ViewBag.disc = 0;
            return View("AddSeatsAndDiscount", fastResTic);
        }

        public async Task<ActionResult> AddFastReserveTicket(Guid projekcija)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            var projection = dbCtx.Database.SqlQuery<Projection>("select * from Projections where Id = '" + projekcija + "'").FirstOrDefault();
            var projWithHalls = await dbCtx.Projections.Include(x => x.ProjHallsTimeList).FirstOrDefaultAsync(x => x.Id == projekcija);
            var projHalls = new List<HallTimeProjection>();
            foreach (HallTimeProjection htp in projWithHalls.ProjHallsTimeList)
            {
                HallTimeProjection projHall = new HallTimeProjection();
                projHall = dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefault(x => x.Id == htp.Id);
                projHalls.Add(projHall);
            }
            projWithHalls.ProjHallsTimeList = projHalls;
            return View("FastReserveTicket", projWithHalls);
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
            var mama = dbCtx.Halls.Include(x => x.Seats).FirstOrDefault(x => x.Id == hallToShow.Id);
            
            return View("Seats", mama);
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
            List<SelectListItem> items = new List<SelectListItem>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            var location = await dbCtx.Locations.Include(x => x.HallsList).FirstOrDefaultAsync(x => x.Id == MyLocation);
            if (model.TicketPrice <= 0)
            {
                List<string> halls = new List<string>();


                foreach (Hall h1 in location.HallsList)
                {
                    items.Add(new SelectListItem { Text = h1.Name });
                    halls.Add(h1.Name);
                }
                model.Hale = halls;
                ViewBag.projekcija = projekcija;
                ViewBag.location = MyLocation;
                ViewBag.Halls = items;
                ModelState.AddModelError("", "Ticket price can't be 0 or less than 0.");
                ViewBag.location = MyLocation;
                return View("AddHallTimeProjection", model);
            }

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
            var hallWithSeats = await dbCtx.Halls.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == h.Id);
            List<Row> seats = new List<Row>();

            string konfiguracija = "";
            for (int br = 0; br < hallWithSeats.RowsCount; br++)
            {
                Row row = new Models.Row();
                for(int br2 = 0; br2<hallWithSeats.Seats[br].Seats.Length; br2++)
                {
                    konfiguracija += hallWithSeats.Seats[br].Seats[br2];
                }
                row.Seats = konfiguracija;
                seats.Add(row);
                dbCtx.Rows.Add(row);
                konfiguracija = "";
            }
            string date = model.Date;
            string time = model.Time;
            string datetime = date + " " + time;
            /*List<Row> seats = new List<Row>();
            
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
            }*/
            DateTime datum = DateTime.Parse(datetime);
            
            var projection = await dbCtx.Projections.Include(x => x.ProjHallsTimeList).FirstOrDefaultAsync(x => x.Id == projekcija);
            HallTimeProjection ToAdd = new HallTimeProjection
            {
                Hall = h,
                Time = datum,
                Seats = seats,
                Projection = projection,
                TicketPrice = model.TicketPrice

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