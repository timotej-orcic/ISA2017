using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Linq;
using System.Collections.Generic;
using WebApplication2.Models;
using System.Security.Cryptography;
using System.Text;

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
        public List<ApplicationUser> RequestsList { get; set; }
        public List<Ticket> ReservationsList { get; set; }
        public List<Recension> RecensionList { get; set; }
        public List<Post> PostsList { get; set; }
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
            setRoles(ctx);
            return ctx;
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public DbSet<Hall> Halls { get; set; }
        public DbSet<Location> Locations { get; set; }
        protected override void OnModelCreating(DbModelBuilder mb)
        {
            base.OnModelCreating(mb);
        }

        public static async void setRoles(ApplicationDbContext ctx)
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
                        //Password = "MAdmin123!",
                        //PasswordHash = CreateMD5("MAdmin123!"),
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