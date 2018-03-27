using Isa2017Cinema.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace Isa2017Cinema
{
    public class Initializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        public Initializer()
        {
        }

        protected override void Seed(ApplicationDbContext context)
        {
            FanZone fz1 = new FanZone
            {
                Id = Guid.NewGuid(),
                PostsList = new List<Post>(),
                RequisitsList = new List<ThemeRequisit>()
            };
            FanZone fz2 = new FanZone
            {
                Id = Guid.NewGuid(),
                PostsList = new List<Post>(),
                RequisitsList = new List<ThemeRequisit>()
            };
            var locations = new List<Location>
            {
                new Location { LocType = LocationType.CINEMA, Name = "Arena Cineplex", Address = "Novosadskog sajma" , Description="Nema opis" , DiscountedTicketsList = new List<Ticket>()
                ,ProjectionsList = new List<Projection>(), HallsList = new List<Hall>(), RecensionsList = new List<Recension>(), LocationFanZone = fz1
                   },
                 new Location { LocType = LocationType.CINEMA, Name = "Big Cinestar", Address = "Bulevar" , Description="Nema opis" , DiscountedTicketsList = new List<Ticket>()
                 ,ProjectionsList = new List<Projection>(), HallsList = new List<Hall>(), RecensionsList = new List<Recension>(), LocationFanZone = fz2
                 }

            };

            locations.ForEach(location => context.Locations.Add(location));

            Points bronzePoints = new Points
            {
                Points_Type = PointsType.BRONZE,
                PointsCount = 100
            };
            context.DiscountPoints.Add(bronzePoints);

            Points silverPoints = new Points
            {
                Points_Type = PointsType.SILVER,
                PointsCount = 400
            };
            context.DiscountPoints.Add(silverPoints);

            Points goldPoints = new Points
            {
                Points_Type = PointsType.GOLD,
                PointsCount = 1000
            };
            context.DiscountPoints.Add(goldPoints);

            context.SaveChanges();
        }
    }
}