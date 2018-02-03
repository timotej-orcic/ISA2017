using Isa2017Cinema.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class System_AdminController : Controller
    {
        // GET: System_Admin
        public ActionResult AdminPage()
        {
            return View();
        }

        // GET: System_Admin/AddNewAdmin
        public ActionResult AddNewAdmin()
        {
            return View();
        }

        public async Task<ActionResult> AddAdmin(AddNewAdminViewModel adminVM)
        {
            if(adminVM.Name != null && adminVM.LastName != null && adminVM.Email != null && adminVM.UserName != null)
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
                        PendingPostsList = new List<Post>()
                    };
                }
                else if (adminVM.Admin_Type == AdminType.LOCATION_ADMIN)
                {
                    newAdmin = new LocationAdmin
                    {
                        Admin_Type = adminVM.Admin_Type,
                        Name = adminVM.Name,
                        LastName = adminVM.LastName,
                        Email = adminVM.Email,
                        UserName = adminVM.UserName,
                        AdminLocationsList = new List<Location>()
                    };
                }

                if (newAdmin != null)
                {
                    using (var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
                    {
                        IdentityResult result;
                        if (um.Users.FirstOrDefault(usr => usr.Email == newAdmin.Email) == null)
                        {
                            result = await um.CreateAsync(newAdmin, RandomString());
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
            return View("AdminPage");
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