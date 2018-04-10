﻿using Isa2017Cinema.Models;
using System.Collections.Generic;
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
            FanZone fanzone = new FanZone
            {
                PostsList = new List<Post>(),
                RequisitsList = new List<ThemeRequisit>()
            };

            context.Fanzone.Add(fanzone);

            var locations = new List<Location>
            {
                new Location { LocType = LocationType.CINEMA, Name = "Arena Cineplex", Address = "Novosadskog sajma" , Description="Nema opis" , DiscountedTicketsList = new List<Ticket>()
                ,ProjectionsList = new List<Projection>(), HallsList = new List<Hall>(), RecensionsList = new List<Recension>()
                   },
                 new Location { LocType = LocationType.CINEMA, Name = "Big Cinestar", Address = "Bulevar" , Description="Nema opis" , DiscountedTicketsList = new List<Ticket>()
                 ,ProjectionsList = new List<Projection>(), HallsList = new List<Hall>(), RecensionsList = new List<Recension>()
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

            ThemeRequisit thReq1 = new ThemeRequisit
            {
                Name = "Batman hat",
                AvailableCount = 5,
                Description = "Cool hat",
                ImageUrl = "~/images/placeholder4.png",
                Price = 1000
            };
            context.ThemeRequisits.Add(thReq1);

            context.SaveChanges();
        }
    }
}