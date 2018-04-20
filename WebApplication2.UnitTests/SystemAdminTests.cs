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
using System.Threading.Tasks;

namespace WebApplication2.UnitTests
{
    [TestFixture]
    public class SystemAdminTests
    {
        [Test]
        public void AddAdmin_RequestNotAuthenticated_RedirectToLoginPage()
        {
            //Arrange
            System_AdminController sac = new System_AdminController();

            var requestMock = new Mock<HttpRequestBase>();
            requestMock.SetupGet(x => x.IsAuthenticated).Returns(false);

            var contextMock = new Mock<HttpContextBase>();
            contextMock.SetupGet(x => x.Request).Returns(requestMock.Object);

            sac.ControllerContext = new ControllerContext(contextMock.Object, new RouteData(), sac);
            var inputVM = new AddNewAdminViewModel();

            //Act
            var result = sac.AddAdmin(inputVM).Result;
            RedirectToRouteResult routeResult = result as RedirectToRouteResult;

            //Assers
            Assert.That(routeResult, Is.Not.Null);
            Assert.AreEqual(routeResult.RouteValues["action"], "Login");
            Assert.That(sac.TempData.ContainsKey("success"), Is.False);
        }

        [Test]
        public void AddAdmin_UserNotInGoodRole_RedirectToHomePage()
        {
            //Arrange
            System_AdminController sac = new System_AdminController();

            var userMock = new Mock<IPrincipal>();
            userMock.Setup(p => p.IsInRole("Regular_User")).Returns(true);

            var requestMock = new Mock<HttpRequestBase>();
            requestMock.SetupGet(x => x.IsAuthenticated).Returns(true);

            var contextMock = new Mock<HttpContextBase>();
            contextMock.SetupGet(ctx => ctx.User).Returns(userMock.Object);
            contextMock.SetupGet(ctx => ctx.Request).Returns(requestMock.Object);

            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.SetupGet(con => con.HttpContext).Returns(contextMock.Object);

            sac.ControllerContext = controllerContextMock.Object;
            var inputVM = new AddNewAdminViewModel();

            //Act
            var result = sac.AddAdmin(inputVM).Result;
            RedirectToRouteResult routeResult = result as RedirectToRouteResult;

            //Assert
            Assert.That(routeResult, Is.Not.Null);
            Assert.AreEqual(routeResult.RouteValues["action"], "Index");
            Assert.That(sac.TempData.ContainsKey("success"), Is.False);
        }

        [Test]
        public void AddAdmin_AddLocationAdminAndSomeVMAttributesAreNull_RedirectToAddNewAdminPage()
        {
            //Arrange
            System_AdminController sac = new System_AdminController();

            var userMock = new Mock<IPrincipal>();
            userMock.Setup(p => p.IsInRole("System_Admin")).Returns(true);

            var requestMock = new Mock<HttpRequestBase>();
            requestMock.SetupGet(x => x.IsAuthenticated).Returns(true);

            var contextMock = new Mock<HttpContextBase>();
            contextMock.SetupGet(ctx => ctx.User).Returns(userMock.Object);
            contextMock.SetupGet(ctx => ctx.Request).Returns(requestMock.Object);

            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.SetupGet(con => con.HttpContext).Returns(contextMock.Object);

            sac.ControllerContext = controllerContextMock.Object;
            var inputVM = new AddNewAdminViewModel()
            {
                Admin_Type = AdminType.LOCATION_ADMIN,
                Email = "",
                Name = null,
                LastName = "",
                UserName = null,
                MyLocationId = ""
            };

            //Act
            var result = sac.AddAdmin(inputVM).Result;
            RedirectToRouteResult routeResult = result as RedirectToRouteResult;

            //Assert
            Assert.That(routeResult, Is.Not.Null);
            Assert.AreEqual(routeResult.RouteValues["action"], "AddNewAdmin");
            Assert.AreEqual(sac.ModelState.Values.Count, 1);
            Assert.AreEqual(sac.ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, "Error while trying to add new admin (some fields are null)");
        }

        [Test]
        public void AddAdmin_AddLocationAdminAndLocIdIsNull_RedirectToAddNewAdminPage()
        {
            //Arrange
            System_AdminController sac = new System_AdminController();

            var userMock = new Mock<IPrincipal>();
            userMock.Setup(p => p.IsInRole("System_Admin")).Returns(true);

            var requestMock = new Mock<HttpRequestBase>();
            requestMock.SetupGet(x => x.IsAuthenticated).Returns(true);

            var contextMock = new Mock<HttpContextBase>();
            contextMock.SetupGet(ctx => ctx.User).Returns(userMock.Object);
            contextMock.SetupGet(ctx => ctx.Request).Returns(requestMock.Object);

            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.SetupGet(con => con.HttpContext).Returns(contextMock.Object);

            sac.ControllerContext = controllerContextMock.Object;
            var inputVM = new AddNewAdminViewModel()
            {
                Admin_Type = AdminType.LOCATION_ADMIN,
                Email = "",
                Name = "",
                LastName = "",
                UserName = "",
                MyLocationId = null
            };

            //Act
            var result = sac.AddAdmin(inputVM).Result;
            RedirectToRouteResult routeResult = result as RedirectToRouteResult;

            //Assert
            Assert.That(routeResult, Is.Not.Null);
            Assert.AreEqual(routeResult.RouteValues["action"], "AddNewAdmin");
            Assert.AreEqual(sac.ModelState.Values.Count, 1);
            Assert.AreEqual(sac.ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage, "Error: Admin location is null.");
        }


        //ovaj vec mora biti integration zbog pristupa bazi
        /*[Test]
        public async Task AddAdmin_AddSystemAdminAlreadyExist_RedirectToAddNewAdminPage()
        {
            //Arrange
            System_AdminController sac = new System_AdminController();

            var userMock = new Mock<IPrincipal>();
            userMock.Setup(p => p.IsInRole("System_Admin")).Returns(true);

            var requestMock = new Mock<HttpRequestBase>();
            requestMock.SetupGet(x => x.IsAuthenticated).Returns(true);

            var contextMock = new Mock<HttpContextBase>();
            contextMock.SetupGet(ctx => ctx.User).Returns(userMock.Object);
            contextMock.SetupGet(ctx => ctx.Request).Returns(requestMock.Object);

            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.SetupGet(con => con.HttpContext).Returns(contextMock.Object);

            sac.ControllerContext = controllerContextMock.Object;
            var inputVM = new AddNewAdminViewModel()
            {
                Admin_Type = AdminType.LOCATION_ADMIN,
                Email = "main.admin@isa.com",
                Name = "",
                LastName = "",
                UserName = "",
                MyLocationId = ""
            };

            //Act
            var result = await sac.AddAdmin(inputVM);
            RedirectToRouteResult routeResult = result as RedirectToRouteResult;

            //Assert
            Assert.That(routeResult, Is.Not.Null);
            Assert.AreEqual(routeResult.RouteValues["action"], "AddNewAdmin");
            Assert.AreEqual(sac.ModelState.Values.Count, 1);
            string errMsg = sac.ModelState.Values.ElementAt(0).Errors.ElementAt(0).ErrorMessage;
            Assert.AreEqual(errMsg, "User with this email adress already exists");
        }*/

    }
}
