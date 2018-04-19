using Isa2017Cinema.Models;
using Moq;
using NUnit.Framework;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using WebApplication2.Controllers;
using WebApplication2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplication2.UnitTests
{
    [TestFixture]
    public class SystemAdminTests
    {
        [Test]
        public void AddAdmin_RequestNotAuthenticated_RedirectToLoginPage()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(false);
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            System_AdminController sac = new System_AdminController();
            sac.ControllerContext = new ControllerContext(context.Object, new RouteData(), sac);
            var inputVM = new AddNewAdminViewModel();

            var result = sac.AddAdmin(inputVM).Result;
            RedirectToRouteResult routeResult = result as RedirectToRouteResult;

            Assert.That(routeResult, Is.Not.Null);
            Assert.AreEqual(routeResult.RouteValues["action"], "Login");
            Assert.That(sac.TempData.ContainsKey("success"), Is.False);
        }

        [Test]
        public void AddAdmin_UserNotInGoodRole_RedirectToHomePage()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);
            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            var userManager = new Mock<UserManager<ApplicationUser>>(userStore.Object);
            var user = new ApplicationUser { Id = "1", UserName = "User" };
            userManager.Object.Create(user);
            userManager.Object.AddToRole(user.Id, "Regular_User");

            //InitUserRoles();
            context.Setup(ctx => ctx.User).Returns((IPrincipal)user);

            System_AdminController sac = new System_AdminController();
            sac.ControllerContext = new ControllerContext(context.Object, new RouteData(), sac);
            var inputVM = new AddNewAdminViewModel();

            var result = sac.AddAdmin(inputVM).Result;
            RedirectToRouteResult routeResult = result as RedirectToRouteResult;

            Assert.That(routeResult, Is.Not.Null);
            Assert.AreEqual(routeResult.RouteValues["action"], "Login");
            Assert.That(sac.TempData.ContainsKey("success"), Is.False);
        }

        public void InitUserRoles()
        {
            List<string> Roles = new List<string>
            {
                "Regular_User",
                "System_Admin",
                "Fanzone_Admin",
                "Location_Admin"
            };

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            var roleManager = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            foreach (var role in Roles)
            {
                if (!roleManager.Object.RoleExists(role))
                {
                    var roleResult = roleManager.Object.Create(new IdentityRole(role));
                }
            }
        }

    }
}
