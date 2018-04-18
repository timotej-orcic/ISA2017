using Isa2017Cinema.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                        string fzID = ctx.Fanzone.FirstOrDefault().Id.ToString();
                        var fzData = ctx.Fanzone.Include(x => x.RequisitsList).FirstOrDefault();
                        foreach (var req in fzData.RequisitsList)
                        {
                            if(req.ParentUserId == null)
                                themeRequisits.Add(req);
                        }
                    }

                    ViewBag.requisitsToShow = themeRequisits;

                    string uID = User.Identity.GetUserId();

                    if (User.IsInRole("Regular_User"))
                    {
                        List<ThemeRequisit> myRequisits = new List<ThemeRequisit>();
                        List<Post> allPosts = new List<Post>();
                        List<Post> myPosts = new List<Post>();
                        using (var ctx = new ApplicationDbContext())
                        {
                            string fzID = ctx.Fanzone.FirstOrDefault().Id.ToString();
                            var fzData = ctx.Fanzone.Include(x => x.PostsList).Include(y => y.RequisitsList).FirstOrDefault();

                            foreach (var p in fzData.PostsList)
                            {
                                if (p.ParentUserId != uID)
                                    allPosts.Add(p);
                                else
                                    myPosts.Add(p);
                            }

                            foreach (var req in fzData.RequisitsList)
                            {
                                if(req.ParentUserId == uID)
                                    myRequisits.Add(req);
                            }
                        }

                        ViewBag.myRequisits = myRequisits;
                        ViewBag.allPosts = allPosts;
                        ViewBag.myPosts = myPosts;
                    }
                    else if (User.IsInRole("Fanzone_Admin"))
                    {
                        List<Post> unapprovedPosts = new List<Post>();
                        List<Post> postsToManage = new List<Post>();
                        using (var ctx = new ApplicationDbContext())
                        {
                            var fzData = ctx.Fanzone.Include(y => y.PostsList).FirstOrDefault();

                            foreach (var p in fzData.PostsList)
                            {
                                if (!p.IsTakenByAdmin)
                                    unapprovedPosts.Add(p);
                                else if (uID == p.ParentAdminId)
                                    postsToManage.Add(p);
                            }                               

                            ViewBag.unapprovedPosts = unapprovedPosts;
                            ViewBag.postsToManage = postsToManage;
                        }
                    }

                    ViewBag.systemTime = DateTime.Now;

                    ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
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
                                        RequisitID = reqID,                                        
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

        // GET: FanZone/PostOffers
        public ActionResult PostOffers(string postID)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Regular_User"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    bool isExpired = false;
                    bool isGraded = false;
                    bool iHaveOffer = false;
                    bool iAmParent = false;
                    List<Licitation> postLicitations = new List<Licitation>();
                    Dictionary<string, string> userNames = new Dictionary<string, string>();

                    using (ApplicationDbContext ctx = new ApplicationDbContext())
                    {
                        string fzID = ctx.Fanzone.FirstOrDefault().Id.ToString();
                        var fzData = ctx.Fanzone.Include(x => x.PostsList.Select(y => y.LicitationsList)).FirstOrDefault();
                        var resPost = fzData.PostsList.FirstOrDefault(p => p.Id.ToString() == postID);

                        if (resPost.OfferExpireDate < DateTime.Now)
                            isExpired = true;

                        if (resPost.IsGraded)
                            isGraded = true;

                        string uID = User.Identity.GetUserId().ToString();

                        if (resPost.ParentUserId == uID)
                            iAmParent = true;

                        if(resPost != null)
                        {
                            foreach (var l in resPost.LicitationsList)
                            {
                                postLicitations.Add(l);

                                if(!userNames.ContainsKey(l.ParentUserId))
                                {
                                    using (var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
                                    {
                                        userNames.Add(l.ParentUserId, um.FindById(l.ParentUserId).UserName);
                                    }
                                }                                    

                                if (l.ParentUserId == uID)
                                    iHaveOffer = true;
                            }                                
                        }
                    }

                    ViewBag.postLicitations = postLicitations;
                    ViewBag.userNames = userNames;
                    ViewBag.isExpired = isExpired;
                    ViewBag.isGraded = isGraded;
                    ViewBag.iHaveOffer = iHaveOffer;
                    ViewBag.iAmParent = iAmParent;
                    ViewBag.postID = postID;
                    
                    ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);

                    return View();
                }                    
            }
        }

        // GET: FanZone/AddMyPostOffer
        public ActionResult AddMyPostOffer(string postID)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Regular_User"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    AddMyPostOfferViewModel ampoVM = new AddMyPostOfferViewModel
                    {
                        PostId = postID
                    };
                    return View(ampoVM);
                }                    
            }
        }

        // GET: FanZone/EditMyPostOffer
        public ActionResult EditMyPostOffer(string licitationID)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Regular_User"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    EditMyPostOfferViewModel empoVM = new EditMyPostOfferViewModel();
                    using (ApplicationDbContext ctx = new ApplicationDbContext())
                    {
                        var licData = ctx.Licitations.FirstOrDefault(x => x.Id.ToString() == licitationID);

                        if(licData != null)
                        {
                            empoVM.OfferValue = licData.OfferedPrice;
                            empoVM.LicitationId = licitationID;
                            empoVM.PostId = licData.ParentPostId;
                        }
                    }
                                            
                    return View(empoVM);
                }
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

                        string fzID = ctx.Fanzone.FirstOrDefault().Id.ToString();
                        var fanzone = await ctx.Fanzone.Include(x => x.RequisitsList).FirstOrDefaultAsync(x => x.Id.ToString() == fzID);

                        if(fanzone != null)
                        {
                            var searchReq = fanzone.RequisitsList.FirstOrDefault(x => x.Name == ntrVM.Name);

                            if (searchReq == null)
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
                                        Description = ntrVM.Description,
                                        ParentUserId = null
                                    };

                                    fanzone.RequisitsList.Add(thReq);
                                    ctx.SaveChanges();

                                    TempData["success"] = "Succesfully added a new theme requisit.";
                                    return RedirectToAction("FanZonePage", "FanZone");
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
                            ModelState.AddModelError("", "Error: Can't load fanzone data.");
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

                        string fzID = ctx.Fanzone.FirstOrDefault().Id.ToString();
                        var fanzone = await ctx.Fanzone.Include(x => x.RequisitsList).FirstOrDefaultAsync(x => x.Id.ToString() == fzID);

                        if(fanzone != null)
                        {
                            var resReq = fanzone.RequisitsList.FirstOrDefault(x => x.Id.ToString() == ntrVM.RequisitID);

                            if (resReq != null)
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
                            ModelState.AddModelError("", "Error: Can't read the fanzone data.");
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
                            string fzID = ctx.Fanzone.FirstOrDefault().Id.ToString();
                            var fanzone = await ctx.Fanzone.Include(x => x.RequisitsList).FirstOrDefaultAsync(x => x.Id.ToString() == fzID);

                            if (fanzone != null)
                            {
                                var resReq = fanzone.RequisitsList.FirstOrDefault(x => x.Id.ToString() == reqID);
                                if (resReq != null)
                                {
                                    fanzone.RequisitsList.Remove(resReq);
                                    ctx.ThemeRequisits.Remove(resReq);
                                    ctx.SaveChanges();

                                    TempData["success"] = "Succesfully deleted the given theme requisit.";
                                    return RedirectToAction("FanZonePage", "FanZone");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Error: Can't find the given Theme requisit.");
                                    TempData["ModelState"] = ModelState;
                                    return RedirectToAction("FanZonePage", "FanZone");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error: Can't read the fanzone data.");
                                TempData["ModelState"] = ModelState;
                                return RedirectToAction("FanZonePage", "FanZone");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("FanZonePage", "FanZone");
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
                            string fzID = ctx.Fanzone.FirstOrDefault().Id.ToString();
                            var fanzone = await ctx.Fanzone.Include(x => x.RequisitsList).FirstOrDefaultAsync(x => x.Id.ToString() == fzID);

                            if(fanzone != null)
                            {
                                var resReq = fanzone.RequisitsList.FirstOrDefault(x => x.Id.ToString() == reqID);
                                if (resReq != null)
                                {
                                    if (resReq.AvailableCount > 0)
                                    {
                                        string uID = User.Identity.GetUserId();
                                        var alreadyReservedCheck = fanzone.RequisitsList.FirstOrDefault(x => x.Name == resReq.Name && x.ParentUserId == uID);
                                        int resCnt = 0;

                                        if (alreadyReservedCheck != null)
                                            resCnt = alreadyReservedCheck.AvailableCount;

                                        if (resCnt >= 3)
                                        {
                                            ModelState.AddModelError("", "You can't reserve more then 3 theme requisits of a kind.");
                                            TempData["ModelState"] = ModelState;
                                            return RedirectToAction("FanZonePage", "FanZone");
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
                                                    Price = resReq.Price,
                                                    ParentUserId = uID
                                                };

                                                fanzone.RequisitsList.Add(copyData);
                                            }

                                            resReq.AvailableCount--;
                                            ctx.SaveChanges();

                                            TempData["success"] = "Succesfully reserved the given theme requisit.";
                                            return RedirectToAction("FanZonePage", "FanZone");
                                        }
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "There are no more available theme requisits of this kind. Please try again later.");
                                        TempData["ModelState"] = ModelState;
                                        return RedirectToAction("FanZonePage", "FanZone");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Error: Can't find the given Theme requisit.");
                                    TempData["ModelState"] = ModelState;
                                    return RedirectToAction("FanZonePage", "FanZone");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error: Can't read the fanzone data.");
                                TempData["ModelState"] = ModelState;
                                return RedirectToAction("FanZonePage", "FanZone");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("FanZonePage", "FanZone");
                    }
                }
            }
        }

        public async Task<ActionResult> AddPost(AddNewPostViewModel anpVM)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Regular_User"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if(anpVM.Name != null && anpVM.Description != null && anpVM.BiddingDate != null)
                    {
                        if (anpVM.BiddingDate > DateTime.Now)
                        {
                            using (ApplicationDbContext ctx = new ApplicationDbContext())
                            {
                                string uID = User.Identity.GetUserId();

                                string imgUrl = null;

                                if (anpVM.ImageUpload != null)
                                {
                                    var validImageTypes = new string[]
                                    {
                                            "image/gif",
                                            "image/jpeg",
                                            "image/pjpeg",
                                            "image/png"
                                    };

                                    if (validImageTypes.Contains(anpVM.ImageUpload.ContentType))
                                    {
                                        var uploadDir = "~/images/themeRequisits";
                                        var imagePath = Path.Combine(Server.MapPath(uploadDir), anpVM.ImageUpload.FileName);
                                        var imageUrl = Path.Combine(uploadDir, anpVM.ImageUpload.FileName);
                                        anpVM.ImageUpload.SaveAs(imagePath);

                                        imgUrl = imageUrl;
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "Error: Unsuported image type.");
                                        return View("AddNewPost");
                                    }
                                }

                                Post newPost = new Post
                                {
                                    Name = anpVM.Name,
                                    Description = anpVM.Description,
                                    OfferExpireDate = anpVM.BiddingDate,
                                    ImageUrl = imgUrl,
                                    IsChecked = false,
                                    IsApproved = false,
                                    IsTakenByAdmin = false,
                                    ParentAdminId = null,
                                    ParentUserId = uID,
                                    IsGraded = false,
                                    LicitationsList = new List<Licitation>()
                                };

                                string fzID = ctx.Fanzone.FirstOrDefault().Id.ToString();
                                var fanzone = await ctx.Fanzone.Include(x => x.PostsList).FirstOrDefaultAsync(x => x.Id.ToString() == fzID);
                                if (fanzone != null)
                                {
                                    fanzone.PostsList.Add(newPost);
                                    ctx.SaveChanges();

                                    TempData["success"] = "Succesfully submitted a new post.";
                                    return RedirectToAction("FanZonePage", "FanZone");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Error: Can't load fanzone data.");
                                    return View("AddNewPost");
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Warning: The bidding date must be a future DateTime element.");
                            return View("AddNewPost");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        return View("AddNewPost");
                    }
                }
            }
        }

        public async Task<ActionResult> ManagePost(string postID)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Fanzone_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if (postID != null)
                    {
                        using (ApplicationDbContext ctx = new ApplicationDbContext())
                        {
                            var fzData = ctx.Fanzone.Include(x => x.PostsList).FirstOrDefault();
                            var post = fzData.PostsList.FirstOrDefault(y => y.Id.ToString() == postID);

                            if (post != null)
                            {
                                if (!post.IsTakenByAdmin)
                                {
                                    string uID = User.Identity.GetUserId();

                                    post.IsTakenByAdmin = true;
                                    post.ParentAdminId = uID;

                                    try
                                    {
                                        ctx.SaveChanges();
                                    }catch(DbUpdateConcurrencyException ex)
                                    {
                                        ModelState.AddModelError("", "This post has already been taken by another admin (db concurency).");
                                        TempData["ModelState"] = ModelState;
                                        return RedirectToAction("FanZonePage", "FanZone");
                                    }

                                    TempData["success"] = "Succesfully took a post to manage.";
                                    return RedirectToAction("FanZonePage", "FanZone");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "This post has already been taken by another admin.");
                                    TempData["ModelState"] = ModelState;
                                    return RedirectToAction("FanZonePage", "FanZone");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error: Can't find the given post.");
                                TempData["ModelState"] = ModelState;
                                return RedirectToAction("FanZonePage", "FanZone");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("FanZonePage", "FanZone");
                    }
                }
            }
        }

        public async Task<ActionResult> ApprovePost(string postID)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Fanzone_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if(postID != null)
                    {
                        using (ApplicationDbContext ctx = new ApplicationDbContext())
                        {
                            var fzData = ctx.Fanzone.Include(x => x.PostsList).FirstOrDefault();
                            var post = fzData.PostsList.FirstOrDefault(y => y.Id.ToString() == postID);

                            if (post != null)
                            {
                                post.IsChecked = true;
                                post.IsApproved = true;
                                ctx.SaveChanges();

                                TempData["success"] = "Succesfully approved the given post.";
                                return RedirectToAction("FanZonePage", "FanZone");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error: Can't find the given post.");
                                TempData["ModelState"] = ModelState;
                                return RedirectToAction("FanZonePage", "FanZone");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("FanZonePage", "FanZone");
                    }
                }
            }
        }

        public async Task<ActionResult> DissmissPost(string postID)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Fanzone_Admin"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if (postID != null)
                    {
                        using (ApplicationDbContext ctx = new ApplicationDbContext())
                        {
                            var fzData = ctx.Fanzone.Include(x => x.PostsList).FirstOrDefault();
                            var post = fzData.PostsList.FirstOrDefault(y => y.Id.ToString() == postID);

                            if (post != null)
                            {
                                post.IsChecked = true;
                                post.IsApproved = false;
                                ctx.SaveChanges();

                                TempData["success"] = "Succesfully dissmissed the given post.";
                                return RedirectToAction("FanZonePage", "FanZone");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error: Can't find the given post.");
                                TempData["ModelState"] = ModelState;
                                return RedirectToAction("FanZonePage", "FanZone");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("FanZonePage", "FanZone");
                    }
                }
            }
        }

        public async Task<ActionResult> DeletePost(string postID)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            else
            {
                if (!User.IsInRole("Regular_User"))
                    return RedirectToAction("Index", "Home");
                else
                {
                    if (postID != null)
                    {
                        using (ApplicationDbContext ctx = new ApplicationDbContext())
                        {
                            var fzData = ctx.Fanzone.Include(x => x.PostsList).FirstOrDefault();
                            var post = fzData.PostsList.FirstOrDefault(y => y.Id.ToString() == postID);

                            if (post != null)
                            {
                                fzData.PostsList.Remove(post);
                                ctx.Posts.Remove(post);
                                ctx.SaveChanges();

                                TempData["success"] = "Succesfully deleted the given post.";
                                return RedirectToAction("FanZonePage", "FanZone");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error: Can't find the given post.");
                                TempData["ModelState"] = ModelState;
                                return RedirectToAction("FanZonePage", "FanZone");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Some attributes are null.");
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("FanZonePage", "FanZone");
                    }
                }
            }
        }

        public async Task<ActionResult> AddPostOffer(AddMyPostOfferViewModel ampoVM)
        {
            if(ampoVM.PostId != null)
            {
                using (ApplicationDbContext ctx = new ApplicationDbContext())
                {
                    string fzID = ctx.Fanzone.FirstOrDefault().Id.ToString();
                    var fzData = ctx.Fanzone.Include(x => x.PostsList.Select(y => y.LicitationsList)).FirstOrDefault();
                    var resPost = fzData.PostsList.FirstOrDefault(p => p.Id.ToString() == ampoVM.PostId);

                    if(resPost != null)
                    {
                        string uID = User.Identity.GetUserId().ToString();
                        var chData = resPost.LicitationsList.FirstOrDefault(l => l.ParentUserId == uID);
                        if(chData == null)
                        {
                            Licitation lic = new Licitation
                            {
                                OfferedPrice = ampoVM.OfferValue,
                                ParentPostId = ampoVM.PostId.ToString(),
                                ParentUserId = uID,
                                IsAccepted = false                                
                            };

                            resPost.LicitationsList.Add(lic);
                            ctx.SaveChanges();

                            TempData["success"] = "Succesfully added my offer for the given post.";
                            return RedirectToAction("PostOffers", "FanZone", new { postID = ampoVM.PostId });
                        }
                        else
                        {
                            ModelState.AddModelError("", "You already posted an offer for this post.");
                            TempData["ModelState"] = ModelState;
                            return RedirectToAction("PostOffers", "FanZone", new { postID = ampoVM.PostId });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Can't find the given post.");
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("PostOffers", "FanZone", new { postID = ampoVM.PostId });
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Error: Some attributes are null.");
                TempData["ModelState"] = ModelState;
                return RedirectToAction("PostOffers", "FanZone", new { postID = ampoVM.PostId });
            }
        }

        public async Task<ActionResult> EditPostOffer(EditMyPostOfferViewModel empoVM)
        {
            if (empoVM.LicitationId != null)
            {
                using (ApplicationDbContext ctx = new ApplicationDbContext())
                {
                    var licData = ctx.Licitations.FirstOrDefault(l => l.Id.ToString() == empoVM.LicitationId);

                    if(licData != null)
                    {
                        licData.OfferedPrice = empoVM.OfferValue;
                        ctx.SaveChanges();

                        TempData["success"] = "Succesfully edited my offer for the given post.";
                        return RedirectToAction("PostOffers", "FanZone", new { postID = empoVM.PostId });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Can't find the given licitation.");
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("EditMyPostOffer", "FanZone", new { licitationID = empoVM.LicitationId });
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Error: Some attributes are null.");
                TempData["ModelState"] = ModelState;
                return RedirectToAction("EditMyPostOffer", "FanZone", new { licitationID = empoVM.LicitationId });
            }
        }

        public async Task<ActionResult> AcceptPostOffer(string licitationID)
        {
            if (licitationID != null)
            {
                using (ApplicationDbContext ctx = new ApplicationDbContext())
                {
                    var licData = ctx.Licitations.FirstOrDefault(x => x.Id.ToString() == licitationID);

                    if(licData != null)
                    {
                        string fzID = ctx.Fanzone.FirstOrDefault().Id.ToString();
                        var fzData = ctx.Fanzone.Include(x => x.PostsList.Select(y => y.LicitationsList)).FirstOrDefault();
                        var resPost = fzData.PostsList.FirstOrDefault(p => p.Id.ToString() == licData.ParentPostId);

                        if(resPost != null)
                        {
                            resPost.IsGraded = true;
                            licData.IsAccepted = true;
                            ctx.SaveChanges();

                            //email to all users
                            foreach (var l in resPost.LicitationsList)
                            {
                                var fromAddress = new MailAddress("isaNS2017@gmail.com", "ISA NS");

                                string userEmail = "";
                                string userName = "";
                                using (var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
                                {
                                    var user = um.FindById(l.ParentUserId);

                                    if (user != null)
                                    {
                                        userEmail = user.Email;
                                        userName = user.UserName;
                                    }                                        
                                    else
                                        break;
                                }
                                    
                                var toAddress = new MailAddress(userEmail, userName);
                                string fromPassword = "isa2017_123";

                                string subject = "Offer review on post: " + resPost.Name;
                                string body = "";
                                if (l.Id != licData.Id)
                                {
                                    body = "Hello " + userName + System.Environment.NewLine + "Your offer has been refused!";
                                }
                                else
                                {
                                    body = "Hello " + userName + System.Environment.NewLine + "Your offer has been accepted!";
                                }

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
                            

                            TempData["success"] = "Succesfully accepted the given offer for the given post.";
                            return RedirectToAction("PostOffers", "FanZone", new { postID = licData.ParentPostId });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Can't find the given post data.");
                            TempData["ModelState"] = ModelState;
                            return RedirectToAction("PostOffers", "FanZone", new { postID = licData.ParentPostId });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Can't find the given offer data.");
                        TempData["ModelState"] = ModelState;
                        return RedirectToAction("PostOffers", "FanZone", new { postID = licData.ParentPostId });
                    }
                } 
            }
            else
            {
                ModelState.AddModelError("", "Error: Some attributes are null.");
                TempData["ModelState"] = ModelState;
                return RedirectToAction("FanZonePage", "FanZone");
            }
        }

    }
}