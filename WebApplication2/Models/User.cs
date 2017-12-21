using System;
using Isa2017Cinema.Models;

namespace Isa2017Cinema.Models
{
    public abstract class User : ApplicationUser
    {
        public String Name { get; set; }
        public String LastName { get; set; }
        public String Password { get; set; }
        public String City { get; set; }
        public String PhoneNum { get; set; }
    }
}