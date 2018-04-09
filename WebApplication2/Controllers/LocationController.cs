using Isa2017Cinema.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class LocationController : Controller
    {
        // GET: Location
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangeLocation()
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            string id = User.Identity.GetUserId();
            var locationToShow = new Location();
            var locationId = ctx.Database.SqlQuery<Guid>("select MyLocation from AspNetUsers where id = '" + id + "'").FirstOrDefault();

            var projections = ctx.Database.SqlQuery<Projection>("select * from Projections where Location_Id = '" + locationId + "'").ToList();
           
            List<Location> allLocations = new List<Location>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allLocations = dbCtx.Locations.ToList();
            foreach(Location loc in allLocations)
            {
                if (loc.Id.Equals(locationId))
                {
                    locationToShow = loc;
                }
            }
            ViewBag.locc = "dasdas";
            locationToShow.ProjectionsList = projections;
            return View("ChangeLocation", locationToShow);

        }
        public ActionResult DeleteProjection(Guid idProj)
        {
            List<Projection> allProjections = new List<Projection>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allProjections = dbCtx.Projections.ToList();
            Projection projectionForDelete = new Projection();
            foreach(Projection p in allProjections)
            {
                if (p.Id.Equals(idProj))
                {
                    projectionForDelete = p;
                }
            }
            dbCtx.Projections.Remove(projectionForDelete);
            dbCtx.SaveChanges();
            
            return ChangeLocation();


        }

        public ActionResult EditProjection(Guid idProj)
        {
            List<Projection> allProjections = new List<Projection>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allProjections = dbCtx.Projections.ToList();
            Projection projectionForEdit = new Projection();
            foreach (Projection p in allProjections)
            {
                if (p.Id.Equals(idProj))
                {
                    projectionForEdit = p;
                }
            }
            return View("ChangeProjection", projectionForEdit);
        }
        
        public ActionResult ChangeNameProjection(Guid projekcija)
        {
            List<Projection> allProjections = new List<Projection>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allProjections = dbCtx.Projections.ToList();
            Projection projectionForEdit = new Projection();

            foreach (Projection p in allProjections)
            {
                if (p.Id.Equals(projekcija))
                {
                    projectionForEdit = p;
                }
            }
            ChangeProjectionViewModel vm = new ChangeProjectionViewModel
            {
                Id = projectionForEdit.Id,
                Field = projectionForEdit.Name
            };
            return View("ChangeNameProjection", vm);
        }

        public ActionResult EditNameProjection(ChangeProjectionViewModel model)
        {
            List<Projection> allProjections = new List<Projection>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allProjections = dbCtx.Projections.ToList();
            Projection projectionForEdit = new Projection();

            
            foreach (Projection p in allProjections)
            {
                if (p.Id.Equals(model.Id))
                {
                    if (model.Field == null)
                    {
                        ModelState.AddModelError("", "Name can not be empty.");
                        return View(p);
                    }
                    else
                    {
                        p.Name = model.Field;
                        projectionForEdit = p;
                    }
                }
            }

           /* */
          
            dbCtx.SaveChanges();
            return View("ChangeProjection", projectionForEdit);
        }
    }
}