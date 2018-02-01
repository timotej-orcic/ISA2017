using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using WebApplication2.Models;

namespace Isa2017Cinema.Models
{

    public enum Type { GOLD, SILVER, BRONZE, DEFAULT }
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    
    public class ApplicationUser : IdentityUser
    {
        public String Name { get; set; }
        public String LastName { get; set; }
        public String Password { get; set; }
        public String City { get; set; }
        public Double Points { get; set; }
        public Type UserType { get; set; }
        public List<ApplicationUser> FriendList { get; set; }
        public List<ApplicationUser> RequestsUserNames { get; set; }
        public List<Ticket> ReservationsList { get; set; }
        public List<Recension> RecensionList { get; set; }
        public List<Post> PostsList { get; set; }
        public List<Request> RequestsList { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DatabaseContext", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {

            return new ApplicationDbContext();
        }
      
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Location> Locations { get; set; }
        protected override void OnModelCreating(DbModelBuilder mb)
        {
            base.OnModelCreating(mb);
        }

    }
}