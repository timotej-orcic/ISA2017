using Isa2017Cinema.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Isa2017Cinema.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            
            bool enableFanzoneOptions = true;
            if (User.IsInRole("Fanzone_Admin"))
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var resUsr = ctx.Database.SqlQuery<FanZoneAdmin>("select * from AspNetUsers where id = '" + User.Identity.GetUserId() + "'").FirstOrDefault();
                    if (resUsr != null)
                    {
                        if (!resUsr.HasSetPassword)
                        {
                            enableFanzoneOptions = false;
                        }
                    }
                    else
                    {
                        enableFanzoneOptions = false;
                    }
                }
            }

            ViewBag.enableFanzoneOptions = enableFanzoneOptions;

            bool enableLocationOptions = true;
            if (User.IsInRole("Location_Admin"))
            {
                using (var ctx = new ApplicationDbContext())
                {
                    var resUsr = ctx.Database.SqlQuery<LocationAdmin>("select * from AspNetUsers where id = '" + User.Identity.GetUserId() + "'").FirstOrDefault();
                    if (resUsr != null)
                    {
                        if (!resUsr.HasSetPassword)
                        {
                            enableLocationOptions = false;
                        }
                    }
                    else
                    {
                        enableLocationOptions = false;
                    }
                }
            }

            ViewBag.enableLocationOptions = enableLocationOptions;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}