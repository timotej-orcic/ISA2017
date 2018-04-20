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
            string id = User.Identity.GetUserId();
            
            Guid idproj = projekcija.Projection.Id;

            List<Recension> recenzije = dbCtx.Recensions.ToList();
            foreach(Recension r in recenzije)
            {
                Recension re = dbCtx.Recensions.Include(x => x.RecensionUser).FirstOrDefault(x => x.Id == r.Id);
                if (re.projection != null)
                {
                    if (re.projection.Id.Equals(idproj) && re.RecensionUser.Id.Equals(id))
                    {
                        return View("AlreadyGaveRecensionProjection", r);
                    }
                }
            }

            return View("ShowRecension",projekcija.Projection);
        }
        public async Task<ActionResult> ShowRecensionForLocation(Guid lokacija)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            Location lokacijaCela = dbCtx.Locations.Include(x => x.ProjectionsList).FirstOrDefault(x => x.Id == lokacija);
            // HallTimeProjection projekcija = dbCtx.HallTimeProjection.Include(x => x.Projection).FirstOrDefault(x => x.Id == rezKarta.Projection.Id);
            string id = User.Identity.GetUserId();

           

            List<Recension> recenzije = dbCtx.Recensions.ToList();
            foreach (Recension r in recenzije)
            {
                Recension re = dbCtx.Recensions.Include(x => x.RecensionUser).FirstOrDefault(x => x.Id == r.Id);
                Recension rec = dbCtx.Recensions.Include(x => x.location).FirstOrDefault(x => x.Id == re.Id);
                if (rec.location != null)
                {
                    if (re.location.Id.Equals(rec.location.Id) && re.RecensionUser.Id.Equals(id))
                    {
                        return View("AlreadyGaveRecensionLocation", rec);
                    }
                }
            }

            return View("ShowRecensionForLocation", lokacijaCela);
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
        [HttpPost]
        public JsonResult RateLocation(String[] arr)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            Guid idLokacije = new Guid(arr[0]);
            int ocena = -1;
            int.TryParse(arr[1], out ocena);
            Location lokacija = dbCtx.Locations.Include(x => x.RecensionsList).FirstOrDefault(x => x.Id == idLokacije);
            string userId = User.Identity.GetUserId();
            var reserver = dbCtx.Users.Include(x => x.RecensionList).FirstOrDefault(x => x.Id == userId);

            Recension newRecension = new Recension
            {
                location = lokacija,
                projection = null,
                RatingLocation = ocena,
                RatingProjection = -1,
                RecensionUser = reserver
            };
            reserver.RecensionList.Add(newRecension);
            dbCtx.Recensions.Add(newRecension);
            dbCtx.SaveChanges();
            var recenzije = dbCtx.Database.SqlQuery<Recension>("select * from Recensions where location_Id = '" + lokacija.Id + "'").ToList();
            double suma = 0;
            foreach (Recension rec in recenzije)
            {
                suma += rec.RatingLocation;
            }
            double prosecna = suma / recenzije.Count;
            lokacija.AvgRating = prosecna;
            dbCtx.SaveChanges();
            var obj = new
            {
                tr = true
            };
            return Json(obj);
        }
    }
}