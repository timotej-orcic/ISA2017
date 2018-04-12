using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}