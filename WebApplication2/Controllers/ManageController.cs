using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Isa2017Cinema.Models;

namespace Isa2017Cinema.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : message == ManageMessageId.ChangeNameSuccess ? "Your name has been changed."
                : message == ManageMessageId.ChangeLastNameSuccess ? "Your lastname has been changed."
                : message == ManageMessageId.ChangeCitySuccess ? "Your city has been changed."
                : message == ManageMessageId.ChangePhoneSuccess ? "Your phone has been changed."
                : message == ManageMessageId.ChangeEmailSuccess ? "Your email has been changed."
                : message == ManageMessageId.ChangeUserNameSuccess ? "Your username has been changed."
                : "";

            var userId = User.Identity.GetUserId();
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(userId);
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                FirstName = user.Name,
                LastName = user.LastName,
                City = user.City,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                UserName = user.UserName
                /*TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)*/
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        public ActionResult ChangeEmail()
        {

            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(User.Identity.GetUserId());
            ChangeEmailViewModel model = new ChangeEmailViewModel
            {
                Email = user.Email
            };
            return View(model);
        }



        //
        // POST: /Manage/ChangeName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {

                var userE = UserManager.FindByEmail(model.Email);
                if (userE != null)
                {
                    ModelState.AddModelError("", "User with that email is already registered.");
                    return View(model);
                }
                else
                {
                    user.Email = model.Email;
                    ctx.SaveChanges();
                    return RedirectToAction("Index", new { Message = ManageMessageId.ChangeEmailSuccess });
                }
            }else
            {
             //   ModelState.AddModelError("", "Email is not good.");
                return View(model);
            }
        }

        public ActionResult ChangeUserName()
        {

            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(User.Identity.GetUserId());
            ChangeFieldViewModel model = new ChangeFieldViewModel
            {
                Field = user.UserName
            };
            return View(model);
        }



        //
        // POST: /Manage/ChangeName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeUserName(ChangeFieldViewModel model)
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(User.Identity.GetUserId());
            if (model.Field == null)
            {
                ModelState.AddModelError("", "User name can not be empty.");
                return View(model);
            }
            bool exists = false;
            foreach(ApplicationUser u in ctx.Users)
            {
                if (u.UserName.Equals(model.Field))
                {
                    exists = true;
                    break;
                }
            }
            if (exists)
            {
                ModelState.AddModelError("", "User with that username is already registered.");
                return View(model);
            }
            else
            {
                user.UserName = model.Field;
                ctx.SaveChanges();
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangeUserNameSuccess });
            }
        }
        //
        // GET: /Manage/ChangeName
        public ActionResult ChangeName()
        {
         
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(User.Identity.GetUserId());
            ChangeFieldViewModel model = new ChangeFieldViewModel
            {
                Field = user.Name
            };
            return View(model);
        }

        

        //
        // POST: /Manage/ChangeName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeName(ChangeFieldViewModel model)
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(User.Identity.GetUserId());
            if (model.Field == null)
            {
                ModelState.AddModelError("", "Name can not be empty.");
                return View(model);
            }
            user.Name = model.Field;
            ctx.SaveChanges();
            return RedirectToAction("Index", new { Message = ManageMessageId.ChangeNameSuccess });
        }

        //GET; /Manage/ChangeLastName
        public ActionResult ChangeLastName()
        {

            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(User.Identity.GetUserId());
            ChangeFieldViewModel model = new ChangeFieldViewModel
            {
                Field = user.LastName
            };
            return View(model);
        }

        //POST: /Manage/ChangeLastName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeLastName(ChangeFieldViewModel model)
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(User.Identity.GetUserId());
            if (model.Field == null)
            {
                ModelState.AddModelError("", "Lastname can not be empty.");
                return View(model);
            }
            user.LastName = model.Field;
            ctx.SaveChanges();
            return RedirectToAction("Index", new { Message = ManageMessageId.ChangeLastNameSuccess });
        }

        //GET; /Manage/ChangeCity
        public ActionResult ChangeCity()
        {

            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(User.Identity.GetUserId());
            ChangeFieldViewModel model = new ChangeFieldViewModel
            {
                Field = user.City
            };
            return View(model);
        }

        //POST: /Manage/ChangeCity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeCity(ChangeFieldViewModel model)
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(User.Identity.GetUserId());
            if (model.Field == null)
            {
                ModelState.AddModelError("", "Name of the city can not be empty.");
                return View(model);
            }
            user.City = model.Field;
            ctx.SaveChanges();
            return RedirectToAction("Index", new { Message = ManageMessageId.ChangeCitySuccess });
        }

        //GET; /Manage/ChangeNumber
        public ActionResult ChangeNumber()
        {

            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(User.Identity.GetUserId());
            ChangePhoneViewModel model = new ChangePhoneViewModel
            {
                PhoneNumber = user.PhoneNumber,
                
            };
            return View(model);
        }

        //POST: /Manage/ChangePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeNumber(ChangePhoneViewModel model)
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            var user = ctx.Users.Find(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {

                user.PhoneNumber = model.PhoneNumber;
                ctx.SaveChanges();
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePhoneSuccess });

            }
        
            else
            {
                //   ModelState.AddModelError("", "Email is not good.");
                return View(model);
            }
          
         }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationDbContext ctx = ApplicationDbContext.Create();
           var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
               
                
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                user.Password = model.NewPassword;
                
                ctx.SaveChanges();
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            ChangeNameSuccess,
            ChangeLastNameSuccess,
            ChangeCitySuccess,
            ChangePhoneSuccess,
            ChangeEmailSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            ChangeUserNameSuccess,

            Error
        }

#endregion
    }
}