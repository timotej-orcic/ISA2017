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

        public ActionResult ShowFriendRequests()
        {
            List<Request> requests = new List<Request>();
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            String id = User.Identity.GetUserId();
            requests = ctx.Requests.ToList();
            var users = ctx.Users.ToList();
            usersToShow = new List<ApplicationUser>();

            foreach (Request req in requests)
            {
                if (req.Receiver_id.Equals(Guid.Parse(id)))
                {
                    foreach (ApplicationUser appuser in users)
                    {
                        if (req.Sender_id.Equals(Guid.Parse(appuser.Id)))
                        {
                            usersToShow.Add(appuser);
                        }
                    }
                }
            }


            ViewBag.usersRequest = usersToShow;
            return View();
        }

        public async Task<ActionResult> AcceptRequest(String username)
        {
            List<Request> requests = new List<Request>();
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            String id = User.Identity.GetUserId();
            requests = ctx.Requests.ToList();
            var users = ctx.Users.ToList();
            usersToShow = new List<ApplicationUser>();
            ApplicationUser user1 = new ApplicationUser();
            ApplicationUser user2 = new ApplicationUser();

            foreach(ApplicationUser appuser in users)
            {
                if (appuser.UserName.Equals(username))
                {
                    user1 = appuser;
                }
                if (appuser.Id.Equals(id))
                {
                    user2 = appuser;
                }
            }
           
            var user = await ctx.Users.Include(x => x.FriendList).FirstOrDefaultAsync(x => x.UserName == user1.UserName);
            user.FriendList.Add(user2);
            var us = await ctx.Users.Include(x => x.FriendList).FirstOrDefaultAsync(x => x.UserName == user2.UserName);
            us.FriendList.Add(user1);

            foreach (Request req in requests)
            {
                if (username.Equals(req.Sender_username) && req.Receiver_id.Equals(Guid.Parse(id)))
                {
                    ctx.Requests.Remove(req);
                    requests.Remove(req);
                    break;
                }
            }
            ctx.SaveChanges();
            foreach (Request req in requests)
            {
                if (req.Receiver_id.Equals(Guid.Parse(id)))
                {
                    foreach (ApplicationUser appuser in users)
                    {
                        if (req.Sender_id.Equals(Guid.Parse(appuser.Id)))
                        {
                            usersToShow.Add(appuser);
                        }
                    }
                }
            }


            ViewBag.usersRequest = usersToShow;
            return View("ShowFriendRequests");
        }
        public async Task<ActionResult> DeclineRequest(String username)
        {
            List<Request> requests = new List<Request>();
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            String id = User.Identity.GetUserId();
            requests = ctx.Requests.ToList();
            var users = ctx.Users.ToList();
            usersToShow = new List<ApplicationUser>();

            foreach(Request req in requests)
            {
                if(username.Equals(req.Sender_username) && req.Receiver_id.Equals(Guid.Parse(id)))
                {
                    ctx.Requests.Remove(req);
                    requests.Remove(req);
                    break;
                }
            }
            ctx.SaveChanges();
            foreach (Request req in requests)
            {
                if (req.Receiver_id.Equals(Guid.Parse(id)))
                {
                    foreach (ApplicationUser appuser in users)
                    {
                        if (req.Sender_id.Equals(Guid.Parse(appuser.Id)))
                        {
                            usersToShow.Add(appuser);
                        }
                    }
                }
            }


            ViewBag.usersRequest = usersToShow;
            return View("ShowFriendRequests");
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
                if (!regUser.Id.Equals(id))
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
                        usersToShow.Add(regUser);
                    }
                }
            }
            List<ApplicationUser> pendinglist = new List<ApplicationUser>();
            foreach(ApplicationUser appuser in usersToShow)
            {
                var userreq = await ctx.Users.Include(x => x.RequestsList).FirstOrDefaultAsync(x => x.Id == appuser.Id);
                foreach(Request req in userreq.RequestsList)
                {
                    if (req.Sender_username.Equals(user.UserName))
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
                        Receiver_id = Guid.Parse(user.Id),
                        Receiver_username = user.UserName,
                        Sender_id = Guid.Parse(us.Id),
                        Sender_username = us.UserName
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
                if (!regUser.Id.Equals(id))
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
                        usersToShow.Add(regUser);
                    }
                }
            }
            List<ApplicationUser> pendinglist = new List<ApplicationUser>();
            foreach (ApplicationUser appuser in usersToShow)
            {
                var userreq = await ctx.Users.Include(x => x.RequestsList).FirstOrDefaultAsync(x => x.Id == appuser.Id);
                foreach (Request req in userreq.RequestsList)
                {
                    if (req.Sender_username.Equals(loggeduser.UserName))
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
            ApplicationUser forRemove2 = new ApplicationUser();
            foreach (ApplicationUser friend in user.FriendList)
            {
                if (friend.UserName.Equals(username))
                {
                    forRemove = friend;
                   
                }
                
            }

            foreach(ApplicationUser appuser in users)
            {
                if (appuser.Id.Equals(id))
                {
                    forRemove2 = appuser;
                }
            }

            usersToShow = new List<ApplicationUser>();
            user.FriendList.Remove(forRemove);
            var user2 = await ctx.Users.Include(x => x.FriendList).FirstOrDefaultAsync(x => x.Id == forRemove.Id);
            user2.FriendList.Remove(forRemove2);
            ctx.SaveChanges();
            foreach (ApplicationUser regUser in users)
            {
                if (!regUser.Id.Equals(id))
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
                        usersToShow.Add(regUser);
                    }
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
                    if (req.Sender_username.Equals(user.UserName))
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

        public async Task<ActionResult> CancelFriend(String username, String name, String lastname)
        {
            List<Request> requests = new List<Request>();
            ApplicationDbContext ctx = ApplicationDbContext.Create();
            String id = User.Identity.GetUserId();
            requests = ctx.Requests.ToList();
            foreach(Request req in requests)
            {
                if(req.Sender_id.Equals(Guid.Parse(id)) && req.Receiver_username.Equals(username))
                {
                    ctx.Requests.Remove(req);
                    break;
                }
            }
            var users = ctx.Users.ToList();
          
            var user = await ctx.Users.Include(x => x.FriendList).FirstOrDefaultAsync(x => x.Id == id);

            
            usersToShow = new List<ApplicationUser>();
            
            ctx.SaveChanges();
            foreach (ApplicationUser regUser in users)
            {
                if (!regUser.Id.Equals(id))
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
                        usersToShow.Add(regUser);
                    }
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
                    if (req.Sender_username.Equals(user.UserName))
                    {
                        pendinglist.Add(appuser);
                        break;
                    }
                }
            }
            ViewBag.pendinglist = pendinglist;
            ViewBag.usersToShow = usersToShow;
            ViewBag.FriendList = user.FriendList;
            return View("ShowFriends", model);
        }
    }
}