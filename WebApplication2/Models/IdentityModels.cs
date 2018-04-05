using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Linq;
using System.Collections.Generic;
using WebApplication2.Models;

namespace Isa2017Cinema.Models
{

    public enum Type { GOLD, SILVER, BRONZE, DEFAULT }
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public double Points { get; set; }
        public Type UserType { get; set; }
        public List<ApplicationUser> FriendList { get; set; }
        public List<ApplicationUser> RequestsUserNames { get; set; }
        public List<Ticket> ReservationsList { get; set; }
        public List<Recension> RecensionList { get; set; }
        public List<ThemeRequisit> ReservedRequisitsList { get; set; }
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
            ApplicationDbContext ctx = new ApplicationDbContext();
            return ctx;
        }

        public DbSet<Hall> Halls { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Points> DiscountPoints { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ThemeRequisit> ThemeRequisits { get; set; }
        public DbSet<FanZone> Fanzone { get; set; }
        protected override void OnModelCreating(DbModelBuilder mb)
        {
            base.OnModelCreating(mb);
        }

        public async void setRoles()
        {
            List<string> Roles = new List<string>
            {
                "Regular_User",
                "System_Admin",
                "Fanzone_Admin",
                "Location_Admin"
            };

            using (var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext())))
            {
                foreach (var role in Roles)
                {
                    if (!rm.RoleExists(role))
                    {
                        var roleResult = rm.Create(new IdentityRole(role));
                        if (!roleResult.Succeeded)
                            throw new ApplicationException("Creating role " + role + "failed with error(s): " + roleResult.Errors);
                    }
                }
            }

            using (var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
            {
                IdentityResult admin;
                if (um.Users.FirstOrDefault(usr => usr.Email == "main.admin@isa.com") == null)
                {
                    SystemAdmin mainAdmin = new SystemAdmin
                    {
                        Admin_Type = AdminType.SYSTEM_ADMIN,
                        Name = "Admin",
                        LastName = "Main",
                        Email = "main.admin@isa.com",
                        UserName = "AdminMain",
                        IsMainAdmin = true
                    };
                    admin = await um.CreateAsync(mainAdmin, "MAdmin123!");
                    if (admin.Succeeded)
                    {
                        if (!um.IsInRole(mainAdmin.Id, "System_Admin"))
                        {
                            var userResult = um.AddToRole(mainAdmin.Id, "System_Admin");
                            if (!userResult.Succeeded)
                                throw new ApplicationException("Adding user '" + mainAdmin.Id + "' to '" + "System_Admin" + "' role failed with error(s): " + userResult.Errors);
                        }
                    }
                }
            }
        }
    }
}