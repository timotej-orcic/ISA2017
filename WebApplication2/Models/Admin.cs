using Isa2017Cinema.Models;

namespace WebApplication2.Models
{
    public enum AdminType { FANZONE_ADMIN, LOCATION_ADMIN, SYSTEM_ADMIN }

    public class Admin : ApplicationUser
    {
        public bool HasSetPassword { get; set; }
        public AdminType Admin_Type { get; set; }
    }
}