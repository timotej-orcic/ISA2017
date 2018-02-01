using Isa2017Cinema.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class FriendsController : Controller
    {
        public List<ApplicationUser> usersToShow;
        // GET: Friends
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowFriends()
        {
            ViewBag.usersToShow = new List<ApplicationUser>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Search(FriendsViewModel model)
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            // var user = (RegUser)ctx.Users.Find(User.Identity.GetUserId());
            String id = User.Identity.GetUserId();
            List<ApplicationUser> friends = new List<ApplicationUser>();
            
           
            //RegUser user = (RegUser)ctx.Users.FirstOrDefault(x => x.Id.Equals(id));
            var users = ctx.Users.ToList();
            
            usersToShow = new List<ApplicationUser>();
            var user = await ctx.Users.Include(x => x.FriendList).FirstOrDefaultAsync(x => x.Id == id);
        
            foreach (ApplicationUser regUser in users)
            {
                if (model.FirstName != null && regUser.Name.ToLower().Equals(model.FirstName.ToLower()) && model.LastName != null && regUser.LastName.ToLower().Equals(model.LastName.ToLower()))
                {
                    usersToShow.Add(regUser);
                }
                else if (model.FirstName != null && regUser.Name.ToLower().Equals(model.FirstName.ToLower()))
                {
                    usersToShow.Add(regUser);
                }
                else if (model.LastName != null && regUser.LastName.ToLower().Equals(model.LastName.ToLower()))
                {
                    usersToShow.Add(regUser);
                }
                else if (model.FirstName == null && model.LastName == null)
                {
                    ModelState.AddModelError("", "You didn't type nothing.");
                    return View("ShowFriends");
                }
            }
           // user.FriendList.Add(usersToShow[0]);
            ctx.SaveChanges();
            ViewBag.usersToShow = usersToShow;
            ViewBag.FriendList = user.FriendList;
            return View("ShowFriends");
        }
    }
}