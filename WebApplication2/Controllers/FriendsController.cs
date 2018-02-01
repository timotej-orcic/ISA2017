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

        public List<ApplicationUser>  usersToShow ;
        
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
            //List<ApplicationUser> friends = new List<ApplicationUser>();
            
           
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
            List<ApplicationUser> pendinglist = new List<ApplicationUser>();
            foreach(ApplicationUser appuser in usersToShow)
            {
                var userreq = await ctx.Users.Include(x => x.RequestsList).FirstOrDefaultAsync(x => x.Id == appuser.Id);
                foreach(Request req in userreq.RequestsList)
                {
                    if (req.Sender.Equals(user))
                    {
                        pendinglist.Add(appuser);
                        break;
                    }
                }
            }
            //user.FriendList.Add(usersToShow[0]);
            ctx.SaveChanges();
            ViewBag.pendinglist = pendinglist;
            ViewBag.usersToShow = usersToShow;
            ViewBag.FriendList = user.FriendList;
            return View("ShowFriends",model);
        }
        public async Task<ActionResult> AddFriend(String username, String name, String lastname)
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            String id = User.Identity.GetUserId();
           // var user = await ctx.Users.Include(x => x.FriendList).FirstOrDefaultAsync(x => x.Id == id);
            Console.WriteLine(username);
            ApplicationUser us = ctx.Users.Find(User.Identity.GetUserId());
            var users = ctx.Users.ToList();
            ApplicationUser toAdd = new ApplicationUser();
            var loggeduser = await ctx.Users.Include(x => x.FriendList).FirstOrDefaultAsync(x => x.Id == id);
            foreach (ApplicationUser u in users)
            {
                if (u.UserName.Equals(username))
                {
                    
                    var user = await ctx.Users.Include(x => x.RequestsList).FirstOrDefaultAsync(x => x.UserName == username);
                    Request friendRequest = new Models.Request()
                    {
                        Id = Guid.NewGuid(),
                        Receiver = user,
                        Sender = us
                    };
                    user.RequestsList.Add(friendRequest);
                }
            }
            ctx.SaveChanges();
            usersToShow = new List<ApplicationUser>();
            FriendsViewModel model = new FriendsViewModel
            {
                FirstName = name,
                LastName = lastname
            };
            
            foreach (ApplicationUser regUser in users)
            {
                if (name != null && regUser.Name.ToLower().Equals(name.ToLower()) && lastname != null && regUser.LastName.ToLower().Equals(lastname.ToLower()))
                {

                    usersToShow.Add(regUser);
                }
                else if (name != null && regUser.Name.ToLower().Equals(name.ToLower()))
                {

                    usersToShow.Add(regUser);
                }
                else if (lastname != null && regUser.LastName.ToLower().Equals(lastname.ToLower()))
                {

                    usersToShow.Add(regUser);
                }
                else if (name == null && lastname == null)
                {
                    ModelState.AddModelError("", "You didn't type nothing.");
                    return View("ShowFriends");
                }
            }
            List<ApplicationUser> pendinglist = new List<ApplicationUser>();
            foreach (ApplicationUser appuser in usersToShow)
            {
                var userreq = await ctx.Users.Include(x => x.RequestsList).FirstOrDefaultAsync(x => x.Id == appuser.Id);
                foreach (Request req in userreq.RequestsList)
                {
                    if (req.Sender.Equals(loggeduser))
                    {
                        pendinglist.Add(appuser);
                        break;
                    }
                }
            }
            ViewBag.pendinglist = pendinglist;
            ViewBag.usersToShow = usersToShow;
            ViewBag.FriendList = us.FriendList;
            return View("ShowFriends", model);
        }

        public async Task<ActionResult> RemoveFriend(String username, String name, String lastname)
        {
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            String id = User.Identity.GetUserId();
            var user = await ctx.Users.Include(x => x.FriendList).FirstOrDefaultAsync(x => x.Id == id);
            Console.WriteLine(username);
            var users = ctx.Users.ToList();
            ApplicationUser forRemove = new ApplicationUser();
            foreach(ApplicationUser friend in user.FriendList)
            {
                if (friend.UserName.Equals(username))
                {
                    forRemove = friend;
                   
                }
            }
            usersToShow = new List<ApplicationUser>();
            user.FriendList.Remove(forRemove);
            ctx.SaveChanges();
            foreach (ApplicationUser regUser in users)
            {
                if (name != null && regUser.Name.ToLower().Equals(name.ToLower()) && lastname != null && regUser.LastName.ToLower().Equals(lastname.ToLower()))
                {

                    usersToShow.Add(regUser);
                }
                else if (name != null && regUser.Name.ToLower().Equals(name.ToLower()))
                {

                    usersToShow.Add(regUser);
                }
                else if (lastname != null && regUser.LastName.ToLower().Equals(lastname.ToLower()))
                {

                    usersToShow.Add(regUser);
                }
                else if (name == null && lastname == null)
                {
                    ModelState.AddModelError("", "You didn't type nothing.");
                    return View("ShowFriends");
                }
            }
            FriendsViewModel model = new FriendsViewModel
            {
                FirstName = name,
                LastName = lastname
            };
            List<ApplicationUser> pendinglist = new List<ApplicationUser>();
            foreach (ApplicationUser appuser in usersToShow)
            {
                var userreq = await ctx.Users.Include(x => x.RequestsList).FirstOrDefaultAsync(x => x.Id == appuser.Id);
                foreach (Request req in userreq.RequestsList)
                {
                    if (req.Sender.Equals(user))
                    {
                        pendinglist.Add(appuser);
                        break;
                    }
                }
            }
            ViewBag.pendinglist = pendinglist;
            ViewBag.usersToShow = usersToShow;
            ViewBag.FriendList = user.FriendList;
            return View("ShowFriends",model);
        }
    }
}