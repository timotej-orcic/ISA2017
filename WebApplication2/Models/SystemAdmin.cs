using Microsoft.AspNet.Identity.EntityFramework;
using WebApplication2.Models;

namespace Isa2017Cinema.Models
{
    public class SystemAdmin : Admin
    {
        public bool IsMainAdmin { get; set; }
    }
}