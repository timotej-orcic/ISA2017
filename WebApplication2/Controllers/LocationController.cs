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
    public class LocationController : Controller
    {
        // GET: Location
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectionsShow()
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            string id = User.Identity.GetUserId();
            var locationId = ctx.Database.SqlQuery<String>("select MyLocationId from AspNetUsers where id = '" + id + "'").FirstOrDefault();
            Guid idLoc = new Guid(locationId);
            return Projections(idLoc);
        }

        public ActionResult HallsShow()
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            string id = User.Identity.GetUserId();
            var locationId = ctx.Database.SqlQuery<String>("select MyLocationId from AspNetUsers where id = '" + id + "'").FirstOrDefault();
            Guid idLoc = new Guid(locationId);
            return Halls(idLoc);
        }

        public ActionResult Projections(Guid MyLocation)
        {
            ViewBag.loc = MyLocation;
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            string id = User.Identity.GetUserId();
            var locationToShow = new Location();
            var locationId = ctx.Database.SqlQuery<String>("select MyLocationId from AspNetUsers where id = '" + id + "'").FirstOrDefault();
            Guid idLoc = new Guid(locationId);

            var projections = ctx.Database.SqlQuery<Projection>("select * from Projections where Location_Id = '" + idLoc + "'").ToList();

            List<Location> allLocations = new List<Location>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allLocations = dbCtx.Locations.ToList();
            foreach (Location loc in allLocations)
            {
                if (loc.Id.Equals(idLoc))
                {
                    locationToShow = loc;
                }
            }
            locationToShow.ProjectionsList = projections;
            return View("Projections", locationToShow);
        }

        public ActionResult Halls(Guid MyLocation)
        {
            ViewBag.loc = MyLocation;
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            string id = User.Identity.GetUserId();
            var locationToShow = new Location();
            var locationId = ctx.Database.SqlQuery<String>("select MyLocationId from AspNetUsers where id = '" + id + "'").FirstOrDefault();
            Guid idLoc = new Guid(locationId);

            var halls = ctx.Database.SqlQuery<Hall>("select * from Halls where ParentLocation_Id = '" + idLoc + "'").ToList();

            List<Location> allLocations = new List<Location>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allLocations = dbCtx.Locations.ToList();
            foreach (Location loc in allLocations)
            {
                if (loc.Id.Equals(idLoc))
                {
                    locationToShow = loc;
                }
            }
            locationToShow.HallsList = halls;
            return View("Halls", locationToShow);
        }

        public ActionResult ChangeLocation()
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            string id = User.Identity.GetUserId();
            var locationToShow = new Location();
            var locationId = ctx.Database.SqlQuery<String>("select MyLocationId from AspNetUsers where id = '" + id + "'").FirstOrDefault();
            Guid idLoc = new Guid(locationId);
            var projections = ctx.Database.SqlQuery<Projection>("select * from Projections where Location_Id = '" + idLoc + "'").ToList();
           
            List<Location> allLocations = new List<Location>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allLocations = dbCtx.Locations.ToList();
            foreach(Location loc in allLocations)
            {
                if (loc.Id.Equals(idLoc))
                {
                    locationToShow = loc;
                }
            }
           locationToShow.ProjectionsList = projections;
            return View("ChangeLocation", locationToShow);

        }

        public async Task<ActionResult> ProjectionSubmit(Models.ProjectionViewModel model, Guid MyLocation)
        {


            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            var location = await dbCtx.Locations.Include(x => x.ProjectionsList).FirstOrDefaultAsync(x => x.Id == MyLocation);

            var validImageTypes = new string[]
                            {
                                "image/gif",
                                "image/jpeg",
                                "image/pjpeg",
                                "image/png"
                            };

            if (validImageTypes.Contains(model.ImageUpload.ContentType))
            {
                var uploadDir = "~/images/projectionPosters";
                var imagePath = Path.Combine(Server.MapPath(uploadDir), model.ImageUpload.FileName);
                var imageUrl = Path.Combine(uploadDir, model.ImageUpload.FileName);
                imageUrl = imageUrl.Replace("\\", "/");
                model.ImageUpload.SaveAs(imagePath);
                imageUrl = imageUrl.Substring(1);
                imageUrl = ".." + imageUrl;
                Projection projectionToAdd = new Projection
                {
                    Name = model.Name,
                    Description = model.Description,
                    DirectorName = model.DirectorName,
                    AvgRating = 0,
                    ActorsList = model.ActorsList,
                    DurationTime = model.DurationTime,
                    Genre = model.Genre,
                    PosterUrl = imageUrl,
                };
                location.ProjectionsList.Add(projectionToAdd);
                dbCtx.SaveChanges();
                ViewBag.location = MyLocation;
            }
            return Projections(MyLocation);
        }

        public ActionResult ChangePictureProjection(Guid projekcija)
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
            ChangeProjectionPictureViewModel vm = new ChangeProjectionPictureViewModel
            {
                Id = projectionForEdit.Id,
                ImageUpload = null
            };
            return View("ChangePictureProjection",vm);
        }

        public ActionResult EditPictureProjection(ChangeProjectionPictureViewModel model)
        {
            List<Projection> allProjections = new List<Projection>();
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            allProjections = dbCtx.Projections.ToList();
            Projection projectionForEdit = new Projection();


            foreach (Projection p in allProjections)
            {
                if (p.Id.Equals(model.Id))
                {
                    if (model.ImageUpload == null)
                    {
                        ModelState.AddModelError("", "Name can not be empty.");
                        return View(p);
                    }
                    else
                    {
                        var validImageTypes = new string[]
                           {
                                "image/gif",
                                "image/jpeg",
                                "image/pjpeg",
                                "image/png"
                           };
                        
                        if (validImageTypes.Contains(model.ImageUpload.ContentType))
                        {
                            var uploadDir = "~/images/projectionPosters";
                            var imagePath = Path.Combine(Server.MapPath(uploadDir), model.ImageUpload.FileName);
                            var imageUrl = Path.Combine(uploadDir, model.ImageUpload.FileName);
                            imageUrl = imageUrl.Replace("\\", "/");
                            model.ImageUpload.SaveAs(imagePath);
                            imageUrl = imageUrl.Substring(1);
                            imageUrl = ".." + imageUrl;

                            p.PosterUrl = imageUrl;

                        }

                        projectionForEdit = p;
                    }
                }
            }

            dbCtx.SaveChanges();
            return EditProjection(projectionForEdit.Id);
        }
        
        public ActionResult AddProjection(Guid MyLocation)
        {
            ViewBag.location = MyLocation;
            return View("AddProjection" , new Models.ProjectionViewModel());
        }
        public ActionResult AddHall(Guid MyLocation)
        {
            ViewBag.location = MyLocation;
            return View("AddHall", new Models.HallViewModel());
        }
        public async Task<ActionResult> HallSubmit(Models.HallViewModel model, Guid MyLocation)
        {
            ApplicationDbContext dbCtx = ApplicationDbContext.Create();
            List<Row> seats = new List<Row>();
            if(model.Rows <= 0)
            {
                ModelState.AddModelError("", "Number of rows can't be 0 or less.");
                ViewBag.location = MyLocation;
                return View("AddHall", model);
            }
            if (model.Columns <= 0)
            {
                ModelState.AddModelError("", "Number of columns can't be 0 or less.");
                ViewBag.location = MyLocation;
                return View("AddHall", model);
            }

            string praznic = "";
            for (int i = 0; i < model.Rows; i++)
            {
                Row row = new Row();
                for (int j = 0; j < model.Columns; j++)
                {
                    praznic += "e";

                }
                row.Seats = praznic;
                seats.Add(row);
                dbCtx.Rows.Add(row);
                praznic = "";
            }
            Hall hallToAdd = new Hall
            {
                
                Name = model.Name,
                RowsCount = model.Rows,
                ColsCount = model.Columns,
                Seats = seats
                
            };

            
            var location = await dbCtx.Locations.Include(x => x.HallsList).FirstOrDefaultAsync(x => x.Id == MyLocation);
            location.HallsList.Add(hallToAdd);
            dbCtx.SaveChanges();
           
            ViewBag.location = MyLocation;



            return View("../Hall/Seats",hallToAdd);

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

        public ActionResult DeleteTimeHallProjection(Guid projekcija, Guid timehall)
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
            var karte = dbCtx.Database.SqlQuery<Ticket>("select * from Tickets where Projection_Id = '" + timehall + "'").ToList();
            
            Projection proj = new Projection();
            proj = dbCtx.Projections.Include(x => x.ProjHallsTimeList).FirstOrDefault(x => x.Id == projectionForEdit.Id);
            foreach (HallTimeProjection htp in proj.ProjHallsTimeList)
            {
                if (htp.Id.Equals(timehall))
                {
                    proj.ProjHallsTimeList.Remove(htp);
                    break;
                }
            }
           
            dbCtx.SaveChanges();
            return EditProjection(projectionForEdit.Id);
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
            var projHalls = new List<HallTimeProjection>();
            Projection proj = new Projection();
            proj = dbCtx.Projections.Include(x => x.ProjHallsTimeList).FirstOrDefault(x => x.Id == projectionForEdit.Id);
            foreach (HallTimeProjection htp in proj.ProjHallsTimeList)
            {
                HallTimeProjection projHall = new HallTimeProjection();
                projHall = dbCtx.HallTimeProjection.Include(x => x.Hall).FirstOrDefault(x => x.Id == htp.Id);
                projHall = dbCtx.HallTimeProjection.Include(x => x.Seats).FirstOrDefault(x => x.Id == htp.Id);
                projHalls.Add(projHall);
            }
            proj.ProjHallsTimeList = projHalls;
            
            return View("ChangeProjection", proj);
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
        public ActionResult ChangeCityLocation(Guid idLocation)
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
                Field = locationForEdit.City
            };
            return View("ChangeCityLocation", vm);
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
        public ActionResult EditCityLocation(ChangeProjectionViewModel model)
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
                        ModelState.AddModelError("", "City can not be empty.");
                        return View(p);
                    }
                    else
                    {
                        p.City = model.Field;
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
            return EditProjection(projectionForEdit.Id);
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
            return EditProjection(projectionForEdit.Id);
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
            return EditProjection(projectionForEdit.Id);
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
            return EditProjection(projectionForEdit.Id);
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

        public ActionResult ChangeActors(Guid projekcija)
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
                Field = projectionForEdit.ActorsList
            };
            return View("ChangeActorsProjection", vm);
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
            return EditProjection(projectionForEdit.Id);
        }

        public ActionResult EditActorsProjection(ChangeProjectionViewModel model)
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
                        string actors = model.Field;
                        
                        p.ActorsList = actors;
                        projectionForEdit = p;
                    }
                }
            }

            dbCtx.SaveChanges();
            return EditProjection(projectionForEdit.Id);
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
                        
                        projectionForEdit = p;
                    }
                }
            }

            dbCtx.SaveChanges();
            return EditProjection(projectionForEdit.Id);
        }
       
    }
}