using Isa2017Cinema.Models;
using System;
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

           

            var projections = new List<Projection>
            {
                new Projection {  Name = "Pirati sa Kariba" , ActorsList = "" ,
                Genre = "mjuzikl" , DirectorName = "Hasan" , DurationTime = 123 , PosterUrl = "../images/placeholder4.png" ,
                AvgRating = 10 , Description = "Borba pirata protiv kostura" , ProjHallsTimeList = new List<HallTimeProjection>(),
                 },
                new Projection {  Name = "Indijana Dzouns" , ActorsList = "" ,
                Genre = "drama" , DirectorName = "Tarantino" , DurationTime = 119 , PosterUrl = "../images/placeholder4.png" ,
                AvgRating = 10 , Description = "veoma lijep susret" , ProjHallsTimeList = new List<HallTimeProjection>(),
                  },
                new Projection {  Name = "Pirati sa Eureke" , ActorsList = "" ,
                Genre = "horor" , DirectorName = "Spilberg" , DurationTime = 98 , PosterUrl = "../images/placeholder4.png" ,
                AvgRating = 10 , Description = "nekada davno" , ProjHallsTimeList = new List<HallTimeProjection>(),
                 }
            };

            projections.ForEach(projection => context.Projections.Add(projection));

            var locations = new List<Location>
            {
                new Location { LocType = LocationType.CINEMA, Name = "Arena Cineplex", Address = "Novosadskog sajma" , Description="Nema opis" , DiscountedTicketsList = new List<Ticket>()
                ,ProjectionsList = projections, HallsList = new List<Hall>(), RecensionsList = new List<Recension>(), MyAdminId = null, City="Novi Sad"
                   },
                 new Location { LocType = LocationType.CINEMA, Name = "Big Cinestar", Address = "Bulevar" , Description="Nema opis" , DiscountedTicketsList = new List<Ticket>()
                 ,ProjectionsList = new List<Projection>(), HallsList = new List<Hall>(), RecensionsList = new List<Recension>(), MyAdminId = null, City="Beograd"
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