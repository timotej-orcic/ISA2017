﻿using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using WebApplication2.Models;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNet.Identity;
using WebApplication2.Services;
using Microsoft.Owin.Security;

namespace WebApplication2.Controllers
{
    public class ReservationController : Controller
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        // GET: Reservation
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost] 
        public JsonResult Test(String[] arr)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            Guid id = new Guid(arr[0]);

            var mama = dbCtx.HallTimeProjection.Include(x => x.Seats).FirstOrDefault(x => x.Id == id);
            var saHalom = dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefault(x => x.Id == mama.Id);

            var saProjekcijom =  dbCtx.HallTimeProjection.Include(x => x.Projection).FirstOrDefault(x => x.Id == mama.Id);
            var indexRow = -1;
            var indexColumn = -1;
            List<Ticket> ticketsList = new List<Ticket>();
            if (arr.Length > 1)
            {
                for(int i = 1; i < arr.Length; i++)
                {
                    string[] redKolona = arr[i].Split('_');
                    int.TryParse(redKolona[0],out indexRow);
                    int.TryParse(redKolona[1], out indexColumn);
                  
                    if (indexRow != -1 && indexColumn != -1)
                    {
                        Row red = saProjekcijom.Seats[indexRow-1];
                        var aStringBuilder = new StringBuilder(red.Seats);
                        aStringBuilder.Remove(indexColumn-1, 1);
                        aStringBuilder.Insert(indexColumn-1, "f");
                        red.Seats = aStringBuilder.ToString();
                        saProjekcijom.Seats[indexRow-1] = red;
                        Ticket newReservation = new Ticket
                        {
                            Projection = saProjekcijom,
                            SeatColumn = indexColumn - 1,
                            SeatRow = indexRow - 1,
                            Price = saProjekcijom.Projection.TicketPrice,
                            DiscountMultiplier = 1.0

                        };
                        ticketsList.Add(newReservation);
                        dbCtx.Reservations.Add(newReservation);
                        string userIdString = User.Identity.GetUserId();
                        var loggedUser =  dbCtx.Users.Include(x => x.ReservationsList).FirstOrDefault(x => x.Id == userIdString);
                        loggedUser.Points += 5;
                        loggedUser.ReservationsList.Add(newReservation);

                        indexRow = -1;
                        indexColumn = -1;
                    }
                }
            }

            // var projekcija = dbCtx.Database.SqlQuery<Projection>("select * from Projections where MyLocation = '" + hala.Id + "'").FirstOrDefault();
            string userId = User.Identity.GetUserId();

            dbCtx.SaveChanges();
            var obj = new
            {
                logUser = userId,
                brRezKarata = arr.Length-1,
                idProjekcije = saProjekcijom.Id
            };
            return Json(obj);
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
                projHalls = new List<HallTimeProjection>();
            }
            string id = User.Identity.GetUserId();
            ViewBag.user = id;
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
            // rezervacija sa halom i sedistima
            var mama = await dbCtx.HallTimeProjection.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == projHall);
            var saHalom = await dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefaultAsync(x => x.Id == mama.Id);
            var saProjekcijom = await dbCtx.HallTimeProjection.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == mama.Id);
           
            return View("Seats", saProjekcijom);
        }
        public async Task<ActionResult> CallFriends(string logUser, int brRezKarata, Guid idProjekcije)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            var userWithRes = dbCtx.Users.Include(x => x.ReservationsList).FirstOrDefault(x => x.Id == logUser);
            var userWithFriends = dbCtx.Users.Include(x => x.FriendList).FirstOrDefault(x => x.Id == userWithRes.Id);
            HallTimeProjection projectionWithHall = new HallTimeProjection();

            var mama = await dbCtx.HallTimeProjection.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == idProjekcije);
            var saHalom = await dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefaultAsync(x => x.Id == mama.Id);
            var saProjekcijom = await dbCtx.HallTimeProjection.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == mama.Id);
            CallFriendsViewModel model = new CallFriendsViewModel
            {
                user = userWithFriends,
                brKarata = brRezKarata,
                projectionHallTime = saProjekcijom
            };
          
            return View("CallFriends",model);
        }
        public async Task<ActionResult> InviteFriend(Guid idPrijatelja, Guid idPozivaoca, Guid terminNaKojiPoziva, int brRezKarata)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();

            var mama = await dbCtx.HallTimeProjection.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == terminNaKojiPoziva);
            var saHalom = await dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefaultAsync(x => x.Id == mama.Id);
            var saProjekcijom = await dbCtx.HallTimeProjection.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == mama.Id);
            var inviter = dbCtx.Users.Include(x => x.ReservationsList).FirstOrDefault(x => x.Id == idPozivaoca.ToString());
            var invited = dbCtx.Users.Include(x => x.ReservationsList).FirstOrDefault(x => x.Id == idPrijatelja.ToString());

            var projekcija = dbCtx.Database.SqlQuery<Projection>("select * from Projections where Id = '" + saProjekcijom.Projection.Id + "'").FirstOrDefault();
            var hala = dbCtx.Database.SqlQuery<Hall>("select * from Halls where Id = '" + saProjekcijom.Hall.Id + "'").FirstOrDefault();
            var halaSaLokacijom = await dbCtx.Halls.Include(x => x.ParentLocation).FirstOrDefaultAsync(x => x.Id == saProjekcijom.Hall.Id);
            var karte = dbCtx.Database.SqlQuery<Ticket>("select * from Tickets where Projection_Id = '" + saProjekcijom.Id + "' and ApplicationUser_Id = '" + inviter.Id + "'").ToList();
            Ticket zaPoziv = karte[karte.Count - brRezKarata + 1];
            var karta = await dbCtx.Reservations.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == zaPoziv.Id);

            var callbackUrlAccept = Url.Action("ConfirmInvitation", "Reservation",
                   new { inviterId = inviter.Id, invitedId = invited.Id, ticketId = karta.Id }, protocol: Request.Url.Scheme);
            var callbackUrlDecline = Url.Action("DeclineInvitation", "Reservation",
                   new { inviterId = inviter.Id, invitedId = invited.Id, ticketId = karta.Id }, protocol: Request.Url.Scheme);

            EmailService emailService = new EmailService();
            emailService.SendInvitationEmail(invited, inviter, karta,callbackUrlAccept, callbackUrlDecline);
           
            //sad ovde poslati mejl i u mejlu link na koji ce se ici za potvrdu / odbijanje
            //imam lokaciju halu datum projekciju sve

            return View();
        }

        public async Task<ActionResult> ShowReservations()
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            String id = User.Identity.GetUserId();
            Guid idUsera = new Guid(id);
            var userWithReservations = await ctx.Users.Include(x => x.ReservationsList).FirstOrDefaultAsync(x => x.Id == id);
            List<ProjectionWithFlagViewModel> reservations = new List<ProjectionWithFlagViewModel>();
            foreach(Ticket res in userWithReservations.ReservationsList)
            {
               Ticket resWithProjection = await ctx.Reservations.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == res.Id);
                var mama = await ctx.HallTimeProjection.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == res.Projection.Id);
                var saHalom = await ctx.HallTimeProjection.Include(x => x.Hall).FirstOrDefaultAsync(x => x.Id == mama.Id);
                var saProjekcijom = await ctx.HallTimeProjection.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == mama.Id);

                var hala = ctx.Database.SqlQuery<Hall>("select * from Halls where Id = '" + resWithProjection.Projection.Hall.Id + "'").FirstOrDefault();
                var halaSaLokacijom = await ctx.Halls.Include(x => x.ParentLocation).FirstOrDefaultAsync(x => x.Id == resWithProjection.Projection.Hall.Id);
                resWithProjection.Projection.Hall = halaSaLokacijom;
                var saSedistima = await ctx.HallTimeProjection.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == resWithProjection.Projection.Id);
                resWithProjection.Projection = saSedistima;

              //  reservations.Add(resWithProjection);
                DateTime now = DateTime.Now;
                bool isCancelable = true;
                if(now.Date.Equals(resWithProjection.Projection.Time.Date) && now.TimeOfDay.Hours.Equals(resWithProjection.Projection.Time.TimeOfDay.Hours)){
                    if(resWithProjection.Projection.Time.TimeOfDay.Minutes - now.TimeOfDay.Minutes < 30)
                    {
                        isCancelable = false;
                    }else
                    {
                        isCancelable = true;
                    }
                }else
                {
                    ProjectionWithFlagViewModel forCheck = new ProjectionWithFlagViewModel
                    {
                        Karta = resWithProjection,
                        isCancelable = true
                    };
                    if (checkIfReservationIsInPast(forCheck))
                    {
                        isCancelable = false;
                    }
                    else
                    {
                        isCancelable = true;
                    }
                }
                ProjectionWithFlagViewModel vm = new ProjectionWithFlagViewModel
                {
                    Karta = resWithProjection,
                    isCancelable = isCancelable
                };
                reservations.Add(vm);
            }
            List<ProjectionWithFlagViewModel> sve = new List<ProjectionWithFlagViewModel>();
            foreach(ProjectionWithFlagViewModel t in reservations)
            {
                sve.Add(t);
            }
            List<ProjectionWithFlagViewModel> preciscene = removeSameProjectionTickets(reservations);
            List<Location> visitedPlaces = findVisitHistory(preciscene);
            var visited = new List<SelectListItem>();
            foreach(Location posecena in visitedPlaces)
            {
                visited.Add(new SelectListItem { Text = posecena.Name, Value = posecena.Name });

            }
            ViewBag.visitedPlaces = visited;
            return View("ShowReservations",sve);
        }
        public List<ProjectionWithFlagViewModel> removeSameProjectionTickets(List<ProjectionWithFlagViewModel> reservations)
        {
            for (int i = reservations.Count - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    if (reservations[i].Karta.Projection.Time.Equals(reservations[i - 1].Karta.Projection.Time) && reservations[i].Karta.Projection.Projection.Id.Equals(reservations[i - 1].Karta.Projection.Projection.Id) && reservations[i].Karta.Projection.Hall.Id.Equals(reservations[i - 1].Karta.Projection.Hall.Id))
                    {
                        reservations.RemoveAt(i);
                    }
                }
            }
            for (int i = reservations.Count - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    if (reservations[i].Karta.Projection.Time.Equals(reservations[0].Karta.Projection.Time) && reservations[i].Karta.Projection.Projection.Id.Equals(reservations[0].Karta.Projection.Projection.Id) && reservations[i].Karta.Projection.Hall.Id.Equals(reservations[0].Karta.Projection.Hall.Id))
                    {
                        reservations.RemoveAt(i);
                    }
                }
            }
            return reservations;
        }
        public List<Location> findVisitHistory(List<ProjectionWithFlagViewModel> reservations)
        {
            List<Location> visitedPlaces = new List<Location>();
           
            for(int i = reservations.Count-1; i >=0; i--)
            {
                bool past = checkIfReservationIsInPast(reservations[i]);
                if(past == true)
                {
                    visitedPlaces.Add(reservations[i].Karta.Projection.Hall.ParentLocation);
                }
            }
            return visitedPlaces;
        }
        public bool checkIfReservationIsInPast(ProjectionWithFlagViewModel reservation)
        {
            DateTime now = DateTime.Now;
            bool RetVal = false;
            if (reservation.Karta.Projection.Time.Date.Year < now.Date.Year)
            {
                RetVal = true;
            }else if(reservation.Karta.Projection.Time.Date.Year == now.Date.Year)
            {
                if(reservation.Karta.Projection.Time.Date.Month < now.Date.Month)
                {
                    RetVal = true;
                }else if(reservation.Karta.Projection.Time.Date.Month == now.Date.Month)
                {
                    if(reservation.Karta.Projection.Time.Date.Day < now.Date.Day)
                    {
                        RetVal = true;
                    }
                }
            }
            return RetVal;
        }
        public async Task<ActionResult> ConfirmInvitation(Guid inviterId, Guid invitedId, Guid ticketId)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            Ticket resWithProjection = await dbCtx.Reservations.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == ticketId);

            var mama = await dbCtx.HallTimeProjection.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == resWithProjection.Projection.Id);
            var saHalom = await dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefaultAsync(x => x.Id == mama.Id);
            var saProjekcijom = await dbCtx.HallTimeProjection.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == mama.Id);

            var inviter = dbCtx.Users.Include(x => x.ReservationsList).FirstOrDefault(x => x.Id == inviterId.ToString());
            var invited = dbCtx.Users.Include(x => x.ReservationsList).FirstOrDefault(x => x.Id == invitedId.ToString());

            var projekcija = dbCtx.Database.SqlQuery<Projection>("select * from Projections where Id = '" + saProjekcijom.Projection.Id + "'").FirstOrDefault();
            var hala = dbCtx.Database.SqlQuery<Hall>("select * from Halls where Id = '" + saProjekcijom.Hall.Id + "'").FirstOrDefault();
            var halaSaLokacijom = await dbCtx.Halls.Include(x => x.ParentLocation).FirstOrDefaultAsync(x => x.Id == saProjekcijom.Hall.Id);
            var karta = await dbCtx.Reservations.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == ticketId);
            inviter.Points += 5;
            inviter.ReservationsList.Remove(karta);
            invited.ReservationsList.Add(karta);
            dbCtx.SaveChanges();
           /* if(!User.Identity.GetUserId().Equals(invitedId.ToString()))
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);*/
            EmailService emailService = new EmailService();
            emailService.SendNotificationEmail(invited, inviter, karta, true);

            return View("ConfirmInvitation");
        }
        public async Task<ActionResult> DeclineInvitation(Guid inviterId, Guid invitedId, Guid ticketId)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            Ticket resWithProjection = await dbCtx.Reservations.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == ticketId);

            var mama = await dbCtx.HallTimeProjection.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == resWithProjection.Projection.Id);
            var saHalom = await dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefaultAsync(x => x.Id == mama.Id);
            var saProjekcijom = await dbCtx.HallTimeProjection.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == mama.Id);

            var inviter = dbCtx.Users.Include(x => x.ReservationsList).FirstOrDefault(x => x.Id == inviterId.ToString());
            var invited = dbCtx.Users.Include(x => x.ReservationsList).FirstOrDefault(x => x.Id == invitedId.ToString());

            var projekcija = dbCtx.Database.SqlQuery<Projection>("select * from Projections where Id = '" + saProjekcijom.Projection.Id + "'").FirstOrDefault();
            var hala = dbCtx.Database.SqlQuery<Hall>("select * from Halls where Id = '" + saProjekcijom.Hall.Id + "'").FirstOrDefault();
            var halaSaLokacijom = await dbCtx.Halls.Include(x => x.ParentLocation).FirstOrDefaultAsync(x => x.Id == saProjekcijom.Hall.Id);
            var karta = await dbCtx.Reservations.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == ticketId);

            Row red = saProjekcijom.Seats[karta.SeatRow];
            var aStringBuilder = new StringBuilder(red.Seats);
            aStringBuilder.Remove(karta.SeatColumn, 1);
            aStringBuilder.Insert(karta.SeatColumn, "e");
            red.Seats = aStringBuilder.ToString();
            saProjekcijom.Seats[karta.SeatRow] = red;
            inviter.ReservationsList.Remove(karta);
            dbCtx.SaveChanges();
           /* if (!User.Identity.GetUserId().Equals(invitedId.ToString()))
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);*/
           
            EmailService emailService = new EmailService();
            emailService.SendNotificationEmail(invited, inviter, karta, false);

            return View("DeclineInvitation");
        }
        public async Task<ActionResult> CancelReservation(Guid rezervacija)
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            
            Ticket resWithProjection = await ctx.Reservations.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == rezervacija);
            var mama = await ctx.HallTimeProjection.Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == resWithProjection.Projection.Id);
            var saHalom = await ctx.HallTimeProjection.Include(x => x.Hall).FirstOrDefaultAsync(x => x.Id == mama.Id);
            var saProjekcijom = await ctx.HallTimeProjection.Include(x => x.Projection).FirstOrDefaultAsync(x => x.Id == mama.Id);

            Row red = saProjekcijom.Seats[resWithProjection.SeatRow];
            var aStringBuilder = new StringBuilder(red.Seats);
            aStringBuilder.Remove(resWithProjection.SeatColumn, 1);
            aStringBuilder.Insert(resWithProjection.SeatColumn, "e");
            red.Seats = aStringBuilder.ToString();
            saProjekcijom.Seats[resWithProjection.SeatRow] = red;
            
            String id = User.Identity.GetUserId();
            Guid idUsera = new Guid(id);
            var userWithReservations = await ctx.Users.Include(x => x.ReservationsList).FirstOrDefaultAsync(x => x.Id == id);
            List<ProjectionWithFlagViewModel> reservations = new List<ProjectionWithFlagViewModel>();
            userWithReservations.ReservationsList.Remove(resWithProjection);
            ctx.SaveChanges();

            return await ShowReservations();
        }
    }
}