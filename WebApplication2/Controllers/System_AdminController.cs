using Isa2017Cinema.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class System_AdminController : Controller
    {
        // GET: System_Admin/AddNewAdmin
        public ActionResult AddNewAdmin()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("System_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    List<SelectListItem> items = new List<SelectListItem>();
                    items.Add(new SelectListItem { Text = "Fanzone admin", Value = "0" });
                    items.Add(new SelectListItem { Text = "Location admin", Value = "1" });

                    List<Location> locations = new List<Location>();

                    using (var ctx = new ApplicationDbContext())
                    { 
                        var resUsr = ctx.Database.SqlQuery<SystemAdmin>("select * from AspNetUsers where id = '" + User.Identity.GetUserId() + "'").FirstOrDefault();
                        if (resUsr != null)
                        {
                            if (resUsr.IsMainAdmin)
                            {
                                items.Add(new SelectListItem { Text = "System admin", Value = "2" });
                            }
                        }

                        var resLocs = ctx.Locations.Where(x => x.MyAdminId == null);
                        if(resLocs != null)
                        {
                            foreach(var resLoc in resLocs)
                            {
                                locations.Add(resLoc);
                            }
                        }
                    }

                    ViewBag.items = items;
                    ViewBag.locations = locations;

                    return View();
                }                    
            }
        }

        // GET: System_Admin/RegisterNewLocation
        public ActionResult RegisterNewLocation()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("System_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                    return View();
            }
        }

        // GET: System_Admin/ChangePointsScale
        public ActionResult ChangePointsScale()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("System_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                    return View();
            }
        }

        public async Task<ActionResult> AddAdmin(AddNewAdminViewModel adminVM)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("System_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if (adminVM.Name != null && adminVM.LastName != null && adminVM.Email != null && adminVM.UserName != null)
                    {
                        Admin newAdmin = null;
                        if (adminVM.Admin_Type == AdminType.SYSTEM_ADMIN)
                        {
                            newAdmin = new SystemAdmin
                            {
                                Admin_Type = adminVM.Admin_Type,
                                Name = adminVM.Name,
                                LastName = adminVM.LastName,
                                Email = adminVM.Email,
                                UserName = adminVM.UserName,
                                IsMainAdmin = false                                
                            };
                        }
                        else if (adminVM.Admin_Type == AdminType.FANZONE_ADMIN)
                        {
                            newAdmin = new FanZoneAdmin
                            {
                                Admin_Type = adminVM.Admin_Type,
                                Name = adminVM.Name,
                                LastName = adminVM.LastName,
                                Email = adminVM.Email,
                                UserName = adminVM.UserName,
                                HasSetPassword = false                                
                            };
                        }
                        else if (adminVM.Admin_Type == AdminType.LOCATION_ADMIN)
                        {
                            if(adminVM.MyLocationId != null)
                            {
                                Location loc = new Location();
                                ApplicationDbContext ctx = ApplicationDbContext.Create();
                                Guid idLokacije = new Guid(adminVM.MyLocationId);
                                foreach(Location lokacija in ctx.Locations)
                                {
                                    if (lokacija.Id.Equals(idLokacije)) loc = lokacija;
                                }
                                newAdmin = new LocationAdmin
                                {
                                    Admin_Type = adminVM.Admin_Type,
                                    Name = adminVM.Name,
                                    LastName = adminVM.LastName,
                                    Email = adminVM.Email,
                                    UserName = adminVM.UserName,
                                    MyLocation = loc
                                };
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error: Admin location is null.");
                                return RedirectToAction("AddNewAdmin", "System_Admin");
                            }
                        }

                        if (newAdmin != null)
                        {
                            using (var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
                            {
                                IdentityResult result;
                                if (um.Users.FirstOrDefault(usr => usr.Email == newAdmin.Email) == null)
                                {
                                    //RandomString()
                                    string newPassword = "defaultPassword1!";
                                    result = await um.CreateAsync(newAdmin, newPassword);
                                    if (result.Succeeded)
                                    {
                                        if (adminVM.Admin_Type == AdminType.SYSTEM_ADMIN)
                                        {
                                            if (!um.IsInRole(newAdmin.Id, "System_Admin"))
                                            {
                                                var userResult = um.AddToRole(newAdmin.Id, "System_Admin");
                                                if (!userResult.Succeeded)
                                                {
                                                    ModelState.AddModelError("", "Adding user '" + newAdmin.Id + "' to '" + "System_Admin" + "' role failed with error(s): " + userResult.Errors);
                                                    return View("AddNewAdmin");
                                                }
                                            }
                                        }
                                        else if (adminVM.Admin_Type == AdminType.FANZONE_ADMIN)
                                        {
                                            if (!um.IsInRole(newAdmin.Id, "Fanzone_Admin"))
                                            {
                                                var userResult = um.AddToRole(newAdmin.Id, "Fanzone_Admin");
                                                if (!userResult.Succeeded)
                                                {
                                                    ModelState.AddModelError("", "Adding user '" + newAdmin.Id + "' to '" + "Fanzone_Admin" + "' role failed with error(s): " + userResult.Errors);
                                                    return View("AddNewAdmin");
                                                }
                                                else
                                                {
                                                    //email to isaNS2017@gmail.com from the same address
                                                    var fromAddress = new MailAddress("isaNS2017@gmail.com", "ISA NS");
                                                    var toAddress = new MailAddress("isaNS2017@gmail.com", "ISA NS");
                                                    string fromPassword = "isa2017_123";
                                                    string subject = "Welcome to ISA2017 Cinemas";
                                                    string body = "Hello new Fanzone admin!" + System.Environment.NewLine + "Your sign-in credentials are:" + System.Environment.NewLine + "Email: " + adminVM.Email + System.Environment.NewLine + "Password: " + newPassword;

                                                    var smtp = new SmtpClient
                                                    {
                                                        Host = "smtp.gmail.com",
                                                        Port = 587,
                                                        EnableSsl = true,
                                                        DeliveryMethod = SmtpDeliveryMethod.Network,
                                                        UseDefaultCredentials = false,
                                                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                                                    };
                                                    using (var message = new MailMessage(fromAddress, toAddress)
                                                    {
                                                        Subject = subject,
                                                        Body = body
                                                    })
                                                    {
                                                        smtp.Send(message);
                                                    }
                                                }                                                
                                            }
                                        }
                                        else if (adminVM.Admin_Type == AdminType.LOCATION_ADMIN)
                                        {
                                            if (!um.IsInRole(newAdmin.Id, "Location_Admin"))
                                            {
                                                var userResult = um.AddToRole(newAdmin.Id, "Location_Admin");
                                                if (!userResult.Succeeded)
                                                {
                                                    ModelState.AddModelError("", "Adding user '" + newAdmin.Id + "' to '" + "Location_Admin" + "' role failed with error(s): " + userResult.Errors);
                                                    return View("AddNewAdmin");
                                                }
                                                else
                                                {
                                                    ApplicationDbContext ctx = new ApplicationDbContext();
                                                    var resLoc = ctx.Locations.FirstOrDefault(x => x.Id.ToString() == adminVM.MyLocationId);

                                                    if(resLoc != null)
                                                    {
                                                        resLoc.MyAdminId = newAdmin.Id;
                                                        ctx.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        um.RemoveFromRole(newAdmin.Id, "Location_Admin");
                                                        ModelState.AddModelError("", "Error: Given admin location is not found! Please try again.");
                                                        return View("AddNewAdmin");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "Error while trying to create new admin");
                                        return View("AddNewAdmin");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "User with this email adress already exists");
                                    return View("AddNewAdmin");
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error while trying to add new admin (newAdmin is null)");
                            return View("AddNewAdmin");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error while trying to add new admin (some fields are null)");
                        return View("AddNewAdmin");
                    }

                    TempData["success"] = "Succesfully added a new: " + adminVM.Admin_Type.ToString();
                    return RedirectToAction("Index", "Home");
                }
            }            
        }

        public async Task<ActionResult> AddLocation(RegisterNewLocationViewModel locationVM)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("System_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if (locationVM.Name != null && locationVM.Address != null)
                    {
                        if (locationVM.Location_Type == LocationType.CINEMA || locationVM.Location_Type == LocationType.THEATRE)
                        {
                            ApplicationDbContext ctx = ApplicationDbContext.Create();
                            bool exists = false;

                            if (locationVM.Location_Type == LocationType.CINEMA)
                            {
                                var queryData = ctx.Locations.FirstOrDefault(x => x.LocType == LocationType.CINEMA && x.Name == locationVM.Name);
                                if (queryData != null)
                                    exists = true;
                            }
                            else if (locationVM.Location_Type == LocationType.THEATRE)
                            {
                                var queryData = ctx.Locations.FirstOrDefault(x => x.LocType == LocationType.THEATRE && x.Name == locationVM.Name);
                                if (queryData != null)
                                    exists = true;
                            }

                            if (!exists)
                            {
                                string description = "";
                                if (locationVM.Description != null)
                                    description = locationVM.Description;

                                Location newLocation = new Location
                                {
                                    LocType = locationVM.Location_Type,
                                    Name = locationVM.Name,
                                    Address = locationVM.Address,
                                    Description = description,
                                    DiscountedTicketsList = new List<Ticket>(),
                                    ProjectionsList = new List<Projection>(),
                                    HallsList = new List<Hall>(),
                                    RecensionsList = new List<Recension>(),
                                    MyAdminId = null,
                                    AvgRating = 0
                                };

                                ctx.Locations.Add(newLocation);
                                ctx.SaveChanges();
                            }
                            else
                            {
                                ModelState.AddModelError("", "A " + locationVM.Location_Type.ToString() + " with this name already exists.");
                                return View("RegisterNewLocation");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error: wrong location type.");
                            return View("RegisterNewLocation");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: some inputs are null.");
                        return View("RegisterNewLocation");
                    }

                    TempData["success"] = "Succesfully added a new: " + locationVM.Location_Type.ToString();
                    return RedirectToAction("Index", "Home");
                }
            }           
        }

        public async Task<ActionResult> SetPointsScale(ChangePointsScaleViewModels pointsVM)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("System_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if (pointsVM.BronzeCount == 0 || pointsVM.SilverCount == 0 || pointsVM.GoldCount == 0)
                    {
                        ModelState.AddModelError("", "Points can not be 0.");
                        return View("ChangePointsScale");
                    }
                    else if (pointsVM.BronzeCount >= pointsVM.SilverCount)
                    {
                        ModelState.AddModelError("", "Bronze points can not be greather then or equal to silver points.");
                        return View("ChangePointsScale");
                    }
                    else if (pointsVM.BronzeCount >= pointsVM.GoldCount)
                    {
                        ModelState.AddModelError("", "Bronze points can not be greather then or equal to gold points.");
                        return View("ChangePointsScale");
                    }
                    else if (pointsVM.SilverCount >= pointsVM.GoldCount)
                    {
                        ModelState.AddModelError("", "Silver points can not be greather then or equal to gold points.");
                        return View("ChangePointsScale");
                    }

                    ApplicationDbContext ctx = ApplicationDbContext.Create();
                    var bronzeData = ctx.DiscountPoints.FirstOrDefault(x => x.Points_Type == PointsType.BRONZE);
                    bronzeData.PointsCount = pointsVM.BronzeCount;

                    var silverData = ctx.DiscountPoints.FirstOrDefault(x => x.Points_Type == PointsType.SILVER);
                    silverData.PointsCount = pointsVM.SilverCount;

                    var goldData = ctx.DiscountPoints.FirstOrDefault(x => x.Points_Type == PointsType.GOLD);
                    goldData.PointsCount = pointsVM.GoldCount;

                    ctx.SaveChanges();

                    TempData["success"] = "Succesfully updated the points scale";
                    return RedirectToAction("Index", "Home");
                }
            }            
        }

        private string RandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var numbers = "0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < 2; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            for (int i = 2; i < 6; i++)
            {
                stringChars[i] = numbers[random.Next(numbers.Length)];
            }
            for (int i = 6; i < 8; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
    }
}