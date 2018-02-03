using Isa2017Cinema.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace WebApplication2.Models
{
    public enum AdminType { FANZONE_ADMIN, LOCATION_ADMIN, SYSTEM_ADMIN }

    public class Admin : ApplicationUser
    {
        public AdminType Admin_Type { get; set; }
    }
}