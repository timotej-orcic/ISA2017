using Isa2017Cinema.Models;
using Microsoft.AspNet.Identity;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebApplication2.Controllers;
using WebApplication2.Models;

namespace WebApplication2.UnitTests
{
    [TestFixture]
    public class ReservationControllerTests
    {

        [Test]
        public void CheckIfReservationIsInPast_ItIsInPast_True()
        {
            ReservationController sac = new ReservationController();
            var input = new ProjectionWithFlagViewModel()
            {
                isCancelable = false,
                Karta = new Ticket()
                {
                    Projection = new HallTimeProjection()
                    {
                        Time = new DateTime(),
                    }
                }
            };

            var contextMock = new Mock<HttpContextBase>();
            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.Setup(con => con.HttpContext).Returns(contextMock.Object);

            sac.ControllerContext = controllerContextMock.Object;
            var result = sac.checkIfReservationIsInPast(input);
            Assert.That(result, Is.True);

        }
        [Test]
        public void CheckIfReservationIsInPast_ItIsNotPast_False()
        {
            ReservationController sac = new ReservationController();
            string time = "00:00 PM";
            string vreme1 = "4/20/2018" + " " + time;
            var input = new ProjectionWithFlagViewModel()
            {
                isCancelable = false,
                Karta = new Ticket()
                {
                    Projection = new HallTimeProjection()
                    {
                        Time = DateTime.Parse(vreme1),
                    }
                }
            };

            var contextMock = new Mock<HttpContextBase>();
            var controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.Setup(con => con.HttpContext).Returns(contextMock.Object);

            sac.ControllerContext = controllerContextMock.Object;
            var result = sac.checkIfReservationIsInPast(input);
            Assert.That(result, Is.False);

        }
        /*[Test]
        public void ConfirmInvitation_RedirectToConfirmInvitation()
        {

            var mockDB = new Mock<Database>();
           
            var mockContext = new Mock<ApplicationDbContext>();
           
            Guid inviterId = new Guid();
            Guid invitedId = new Guid();
            Guid ticketId = new Guid();

            ReservationController rrc = new ReservationController();
            var result = rrc.ConfirmInvitation(inviterId, invitedId, ticketId).Result;
            RedirectToRouteResult routeResult = result as RedirectToRouteResult;

            Assert.That(routeResult, Is.Not.Null);
            Assert.AreEqual(routeResult.RouteValues["action"], "ConfirmInvitation");
        }*/
    }
}
