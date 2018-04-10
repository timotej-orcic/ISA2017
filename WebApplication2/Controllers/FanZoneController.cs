using Isa2017Cinema.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class FanZoneController : Controller
    {
        // GET: FanZone/FanZone
        public ActionResult FanZonePage()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!(User.IsInRole("Fanzone_Admin") || User.IsInRole("Regular_User")))
                    return RedirectToAction("Index", "Home");
                else
                {
                    List<ThemeRequisit> themeRequisits = new List<ThemeRequisit>();
                    using (var ctx = new ApplicationDbContext())
                    {
                        var reqs = ctx.Database.SqlQuery<ThemeRequisit>("select * from ThemeRequisits where ApplicationUser_Id is NULL");
                        foreach (var req in reqs)
                        {
                            themeRequisits.Add(req);
                        }
                    }

                    ViewBag.requisitsToShow = themeRequisits;

                    if(User.IsInRole("Regular_User"))
                    {
                        List<Post> posts = new List<Post>();
                        using (var ctx = new ApplicationDbContext())
                        {
                            var postsData = ctx.Posts;
                            foreach (var p in postsData)
                                posts.Add(p);
                        }

                        ViewBag.postsToShow = posts;
                    }

                    return View();
                }
            }
        }

        // GET: FanZone/FirstLoginPasswordChange
        public ActionResult FirstLoginPasswordChange()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                /*if (!User.IsInRole("Fanzone_Admin"))
                    return RedirectToAction("Index", "Home");*/
                if (User.IsInRole("Fanzone_Admin") || User.IsInRole("Location_Admin"))
                    return View();
                else return RedirectToAction("Index", "Home");
            }
        }

        // GET: FanZone/AddNewThemeRequisit
        public ActionResult AddNewThemeRequisit()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Fanzone_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                    return View();
            }
        }

        // GET: FanZone/EditThemeRequisit
        public ActionResult EditThemeRequisit(string reqID)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Fanzone_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if (reqID != null)
                    {
                        EditThemeRequisitViewModel thReqVM;
                        if (TempData["editedRequisit"] == null)
                        {
                            using (ApplicationDbContext ctx = new ApplicationDbContext())
                            {
                                var resReq = ctx.ThemeRequisits.FirstOrDefault(x => x.Id.ToString() == reqID);
                                if (resReq != null)
                                {
                                    thReqVM = new EditThemeRequisitViewModel
                                    {
                                        Name = resReq.Name,
                                        Price = resReq.Price,
                                        AvailableCount = resReq.AvailableCount,
                                        ImageUpload = null,
                                        OldImageUrl = resReq.ImageUrl,
                                        Description = resReq.Description,
                                        RequisitID = reqID
                                    };

                                    return View(thReqVM);
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Error: Can't find the given Theme requisit.");
                                    return View("FanZonePage");
                                }
                            }
                        }
                        else
                        {
                            ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
                            thReqVM = TempData["editedRequisit"] as EditThemeRequisitViewModel;
                            return View(thReqVM);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        return View("FanZonePage");
                    }                    
                }                    
            }
        }

        // GET: FanZone/AddNewPost
        public ActionResult AddNewPost()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Regular_User"))
                    return RedirectToAction("Index", "Home");
                else
                    return View();
            }
        }

        public async Task<ActionResult> ChangeFirstPassword(FirstPasswordChangeViewModel fpcVM)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Fanzone_Admin") && !User.IsInRole("Location_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if (fpcVM.NewPassword != null && fpcVM.ConfirmPassword != null)
                    {
                        if(fpcVM.NewPassword == fpcVM.ConfirmPassword)
                        {
                            using (var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
                            {
                                var user = await um.FindByIdAsync(User.Identity.GetUserId());
                                if (user != null)
                                {
                                    user.PasswordHash = um.PasswordHasher.HashPassword(fpcVM.NewPassword);
                                    var result = await um.UpdateAsync(user);
                                    
                                    if (result.Succeeded)
                                    {
                                        using (var ctx = new ApplicationDbContext())
                                        {
                                            try
                                            {
                                                ctx.Database.ExecuteSqlCommand("update AspNetUsers set HasSetPassword = 1 where Id = '" + user.Id + "'");
                                            }
                                            catch (SqlException e)
                                            {
                                                ModelState.AddModelError("", "Error while trying to change password (SQL exception).");
                                                return View("FirstLoginPasswordChange");
                                            }                                           
                                        }

                                        TempData["success"] = "Succesfully updated the password.";
                                        return RedirectToAction("Index", "Home");
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "Error while trying to change password.");
                                        return View("FirstLoginPasswordChange");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Error while trying to change password (can't find given user).");
                                    return View("FirstLoginPasswordChange");
                                }                                
                            }                            
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error: The passwords are not matching.");
                            return View("FirstLoginPasswordChange");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        return View("FirstLoginPasswordChange");
                    }
                }
            }
        }

        public async Task<ActionResult> AddRequisit(AddNewThemeRequisitViewModel ntrVM)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Fanzone_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if (ntrVM.Description == null)
                        ntrVM.Description = "";
                    if(ntrVM.Name != null && ntrVM.ImageUpload != null && ntrVM.ImageUpload.ContentLength != 0)
                    {
                        ApplicationDbContext ctx = new ApplicationDbContext();

                        var searchReq = ctx.ThemeRequisits.FirstOrDefault(x => x.Name == ntrVM.Name);

                        if(searchReq == null)
                        {
                            var validImageTypes = new string[]
                            {
                                "image/gif",
                                "image/jpeg",
                                "image/pjpeg",
                                "image/png"
                            };

                            if (validImageTypes.Contains(ntrVM.ImageUpload.ContentType))
                            {
                                var uploadDir = "~/images/themeRequisits";
                                var imagePath = Path.Combine(Server.MapPath(uploadDir), ntrVM.ImageUpload.FileName);
                                var imageUrl = Path.Combine(uploadDir, ntrVM.ImageUpload.FileName);
                                ntrVM.ImageUpload.SaveAs(imagePath);

                                ThemeRequisit thReq = new ThemeRequisit
                                {
                                    Name = ntrVM.Name,
                                    Price = ntrVM.Price,
                                    AvailableCount = ntrVM.AvailableCount,
                                    ImageUrl = imageUrl,
                                    Description = ntrVM.Description
                                };

                                ctx.ThemeRequisits.Add(thReq);
                                ctx.SaveChanges();

                                TempData["success"] = "Succesfully added a new theme requisit.";
                                return RedirectToAction("FanZonePage","FanZone");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error: Unsuported image type.");
                                return View("AddNewThemeRequisit");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "The requisit with a given name already existst. Please try a different name.");
                            return View("AddNewThemeRequisit");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        return View("AddNewThemeRequisit");
                    }
                }
            }
        }

        public async Task<ActionResult> EditRequisit(EditThemeRequisitViewModel ntrVM)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Fanzone_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    TempData["editedRequisit"] = ntrVM;
                    if (ntrVM.Description == null)
                        ntrVM.Description = "";
                    if(ntrVM.Name != null)
                    {
                        ApplicationDbContext ctx = new ApplicationDbContext();
                        var resReq = ctx.ThemeRequisits.FirstOrDefault(x => x.Id.ToString() == ntrVM.RequisitID);

                        if(resReq != null)
                        {
                            var newNameCheckReq = ctx.ThemeRequisits.FirstOrDefault(x => x.Name == ntrVM.Name);
                            if (resReq.Name == ntrVM.Name || newNameCheckReq == null)
                            {
                                resReq.Name = ntrVM.Name;
                                resReq.Price = ntrVM.Price;
                                resReq.AvailableCount = ntrVM.AvailableCount;
                                resReq.Description = ntrVM.Description;

                                if (ntrVM.ImageUpload != null)
                                {
                                    var validImageTypes = new string[]
                                    {
                                        "image/gif",
                                        "image/jpeg",
                                        "image/pjpeg",
                                        "image/png"
                                    };

                                    if (validImageTypes.Contains(ntrVM.ImageUpload.ContentType))
                                    {
                                        var uploadDir = "~/images/themeRequisits";
                                        var imagePath = Path.Combine(Server.MapPath(uploadDir), ntrVM.ImageUpload.FileName);
                                        var imageUrl = Path.Combine(uploadDir, ntrVM.ImageUpload.FileName);
                                        ntrVM.ImageUpload.SaveAs(imagePath);

                                        resReq.ImageUrl = imageUrl;
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "Error: Unsuported image type.");
                                        TempData["ModelState"] = ModelState;
                                        return RedirectToAction("EditThemeRequisit", "FanZone", new { reqID = ntrVM.RequisitID });
                                    }
                                }

                                ctx.SaveChanges();
                                TempData["success"] = "Succesfully edited the given theme requisit.";
                                return RedirectToAction("FanZonePage", "FanZone");
                            }
                            else
                            {
                                ModelState.AddModelError("", "The requisit with a given name already existst. Please try a different name.");
                                TempData["ModelState"] = ModelState;
                                return RedirectToAction("EditThemeRequisit", "FanZone", new { reqID = ntrVM.RequisitID });
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Error: Can't find given Theme requisit.");
                            TempData["ModelState"] = ModelState;
                            return RedirectToAction("EditThemeRequisit", "FanZone", new { reqID = ntrVM.RequisitID });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("EditThemeRequisit", "FanZone", new { reqID = ntrVM.RequisitID });
                    }
                }
            }
        }

        public async Task<ActionResult> DeleteRequisit(string reqID)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Fanzone_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if(reqID != null)
                    {
                        using (ApplicationDbContext ctx = new ApplicationDbContext())
                        {
                            var resReq = ctx.ThemeRequisits.FirstOrDefault(x => x.Id.ToString() == reqID);
                            if(resReq != null)
                            {
                                ctx.ThemeRequisits.Remove(resReq);
                                ctx.SaveChanges();

                                TempData["success"] = "Succesfully deleted the given theme requisit.";
                                return RedirectToAction("FanZonePage", "FanZone");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error: Can't find the given Theme requisit.");
                                return View("FanZonePage");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        return View("FanZonePage");
                    }
                }
            }
        }

        public async Task<ActionResult> ReserveRequisit(string reqID)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Regular_User"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if (reqID != null)
                    {
                        using (ApplicationDbContext ctx = new ApplicationDbContext())
                        {
                            var resReq = ctx.ThemeRequisits.FirstOrDefault(x => x.Id.ToString() == reqID);
                            if (resReq != null)
                            {
                                if(resReq.AvailableCount > 0)
                                {
                                    string uID = User.Identity.GetUserId();
                                    var currentUser = await ctx.Users.Include(x => x.ReservedRequisitsList).FirstOrDefaultAsync(x => x.Id.ToString() == uID);

                                    if (currentUser != null)
                                    {
                                        var alreadyReservedCheck =  currentUser.ReservedRequisitsList.FirstOrDefault(x => x.Name == resReq.Name);
                                        int resCnt = 0;

                                        if (alreadyReservedCheck != null)
                                            resCnt = alreadyReservedCheck.AvailableCount;

                                        if (resCnt >= 3)
                                        {
                                            ModelState.AddModelError("", "You can't reserve more then 3 theme requisits of a kind.");
                                            return View("FanZonePage");
                                        }
                                        else
                                        {
                                            resCnt++;

                                            if (alreadyReservedCheck != null)
                                            {
                                                alreadyReservedCheck.AvailableCount = resCnt;
                                            }
                                            else
                                            {
                                                ThemeRequisit copyData = new ThemeRequisit()
                                                {
                                                    Id = resReq.Id,
                                                    Name = resReq.Name,
                                                    AvailableCount = resCnt,
                                                    Description = resReq.Description,
                                                    ImageUrl = resReq.ImageUrl,
                                                    Price = resReq.Price
                                                };

                                                currentUser.ReservedRequisitsList.Add(copyData);
                                            }

                                            resReq.AvailableCount--;
                                            ctx.SaveChanges();

                                            TempData["success"] = "Succesfully reserved the given theme requisit.";
                                            return RedirectToAction("FanZonePage", "FanZone");
                                        }
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "Error: Can't find current user data.");
                                        return View("FanZonePage");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "There are no more available theme requisits of this kind. Please try again later.");
                                    return View("FanZonePage");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error: Can't find the given Theme requisit.");
                                return View("FanZonePage");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        return View("FanZonePage");
                    }
                }
            }
        }

    }
}