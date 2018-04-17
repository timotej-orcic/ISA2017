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
using Microsoft.AspNet.Identity;
using WebApplication2.Services;
using Microsoft.Owin.Security;

namespace WebApplication2.Controllers
{
    public class RecensionController : Controller
    {
        // GET: Recension
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ShowRecension(Guid rezervacija)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            Ticket rezKarta = dbCtx.Reservations.Include(x => x.Projection).FirstOrDefault(x => x.Id == rezervacija);
            HallTimeProjection projekcija = dbCtx.HallTimeProjection.Include(x => x.Projection).FirstOrDefault(x => x.Id == rezKarta.Projection.Id);


            return View("ShowRecension",projekcija.Projection);
        }

        [HttpPost]
        public JsonResult RateProjection(String[] arr)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            Guid idProjekcije = new Guid(arr[0]);
            int ocena = -1;
            int.TryParse(arr[1], out ocena);
            Projection projekcija = dbCtx.Projections.Include(x => x.ProjHallsTimeList).FirstOrDefault(x => x.Id == idProjekcije);
            string userId = User.Identity.GetUserId();
            var reserver = dbCtx.Users.Include(x => x.RecensionList).FirstOrDefault(x => x.Id == userId);

            Recension newRecension = new Recension
            {
                location = null,
                projection = projekcija,
                RatingLocation = -1,
                RatingProjection = ocena,
                RecensionUser = reserver
            };
            reserver.RecensionList.Add(newRecension);
            dbCtx.Recensions.Add(newRecension);
            dbCtx.SaveChanges();
            var recenzije = dbCtx.Database.SqlQuery<Recension>("select * from Recensions where projection_Id = '" + projekcija.Id + "'").ToList();
            double suma = 0;
            foreach(Recension rec in recenzije)
            {
                suma += rec.RatingProjection;
            }
            double prosecna = suma / recenzije.Count;
            projekcija.AvgRating = prosecna;
            dbCtx.SaveChanges();
            var obj = new
            {
                tr = true
            };
            return Json(obj);
        }
  }
}