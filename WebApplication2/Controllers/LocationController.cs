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
        public ActionResult ChangeNameLocation(Guid idLocation)
        {
            List<Location> allLocations = new List<Location>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allLocations = dbCtx.Locations.ToList();
            Location locationForEdit = new Location();

            foreach (Location p in allLocations)
            {
                if (p.Id.Equals(idLocation))
                {
                    locationForEdit = p;
                }
            }
            ChangeProjectionViewModel vm = new ChangeProjectionViewModel
            {
                Id = locationForEdit.Id,
                Field = locationForEdit.Name
            };
            return View("ChangeNameLocation", vm);
        }

        public ActionResult ChangeAddressLocation(Guid idLocation)
        {
            List<Location> allLocations = new List<Location>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allLocations = dbCtx.Locations.ToList();
            Location locationForEdit = new Location();

            foreach (Location p in allLocations)
            {
                if (p.Id.Equals(idLocation))
                {
                    locationForEdit = p;
                }
            }
            ChangeProjectionViewModel vm = new ChangeProjectionViewModel
            {
                Id = locationForEdit.Id,
                Field = locationForEdit.Address
            };
            return View("ChangeAddressLocation", vm);
        }

        public ActionResult ChangeDescriptionLocation(Guid idLocation)
        {
            List<Location> allLocations = new List<Location>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allLocations = dbCtx.Locations.ToList();
            Location locationForEdit = new Location();

            foreach (Location p in allLocations)
            {
                if (p.Id.Equals(idLocation))
                {
                    locationForEdit = p;
                }
            }
            ChangeProjectionViewModel vm = new ChangeProjectionViewModel
            {
                Id = locationForEdit.Id,
                Field = locationForEdit.Description
            };
            return View("ChangeDescriptionLocation", vm);
        }

        public ActionResult EditNameLocation(ChangeProjectionViewModel model)
        {
            List<Location> allLocations = new List<Location>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allLocations = dbCtx.Locations.ToList();
            Location locationForEdit = new Location();

            foreach (Location p in allLocations)
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
                        locationForEdit = p;
                    }
                }
            }

            /* */
            dbCtx.SaveChanges();
            return ChangeLocation();
        }

        public ActionResult EditAddressLocation(ChangeProjectionViewModel model)
        {
            List<Location> allLocations = new List<Location>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allLocations = dbCtx.Locations.ToList();
            Location locationForEdit = new Location();
            
            foreach (Location p in allLocations)
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
                        p.Address = model.Field;
                        locationForEdit = p;
                    }
                }
            }

            /* */
            dbCtx.SaveChanges();
            return ChangeLocation();
        }

        public ActionResult EditDescriptionLocation(ChangeProjectionViewModel model)
        {
            List<Location> allLocations = new List<Location>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allLocations = dbCtx.Locations.ToList();
            Location locationForEdit = new Location();

            foreach (Location p in allLocations)
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
                        p.Description = model.Field;
                        locationForEdit = p;
                    }
                }
            }

            dbCtx.SaveChanges();
            return ChangeLocation();
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

        public ActionResult ChangeGenreProjection(Guid projekcija)
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
                Field = projectionForEdit.Genre
            };
            return View("ChangeGenreProjection", vm);
        }

        public ActionResult EditGenreProjection(ChangeProjectionViewModel model)
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
                        p.Genre = model.Field;
                        projectionForEdit = p;
                    }
                }
            }

            /* */

            dbCtx.SaveChanges();
            return View("ChangeProjection", projectionForEdit);
        }
        public ActionResult ChangeDirectorProjection(Guid projekcija)
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
                Field = projectionForEdit.DirectorName
            };
            return View("ChangeDirectorProjection", vm);
        }

        public ActionResult EditDirectorProjection(ChangeProjectionViewModel model)
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
                        p.DirectorName = model.Field;
                        projectionForEdit = p;
                    }
                }
            }

            /* */

            dbCtx.SaveChanges();
            return View("ChangeProjection", projectionForEdit);
        }
        //GET
        public ActionResult ChangeDescriptionProjection(Guid projekcija)
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
                Field = projectionForEdit.Description
            };
            return View("ChangeDescriptionProjection", vm);
        }
        //POST
        public ActionResult EditDescriptionProjection(ChangeProjectionViewModel model)
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
                        p.Description = model.Field;
                        projectionForEdit = p;
                    }
                }
            }

            dbCtx.SaveChanges();
            return View("ChangeProjection", projectionForEdit);
        }
        public ActionResult ChangeDurationProjection(Guid projekcija)
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
                Field = projectionForEdit.DurationTime.ToString()
            };
            return View("ChangeDurationProjection", vm);
        }
        //POST
        public ActionResult EditDurationProjection(ChangeProjectionViewModel model)
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
                        int duration = -2;
                        int.TryParse(model.Field, out duration);
                        if(duration == 0)
                        {
                            ModelState.AddModelError("", "Duration can not be empty and must contains only numbers.");
                            return View("ChangeProjection", projectionForEdit);
                        }
                        p.DurationTime = duration;
                        projectionForEdit = p;
                    }
                }
            }

            dbCtx.SaveChanges();
            return View("ChangeProjection", projectionForEdit);
        }
        public ActionResult ChangePriceProjection(Guid projekcija)
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
                Field = projectionForEdit.TicketPrice.ToString()
            };
            return View("ChangePriceProjection", vm);
        }
        //POST
        public ActionResult EditPriceProjection(ChangeProjectionViewModel model)
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
                        double price = -2;
                        double.TryParse(model.Field, out price);
                        if (price == 0)
                        {
                            ModelState.AddModelError("", "Duration can not be empty and must contains only numbers.");
                            return View("ChangeProjection", projectionForEdit);
                        }
                        p.TicketPrice = price;
                        projectionForEdit = p;
                    }
                }
            }

            dbCtx.SaveChanges();
            return View("ChangeProjection", projectionForEdit);
        }
       
    }
}