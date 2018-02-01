using Isa2017Cinema.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace WebApplication2.Models
{
    public enum AdminType { SYSTEM_ADMIN, FANZONE_ADMIN, LOCATION_ADMIN}

    public class Admin : ApplicationUser
    {
        public AdminType Admin_Type { get; set; }
        //public string Name { get; set; }
        //public string Lastname { get; set; }
        //public string Password { get; set; }
    }
}