using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Isa2017Cinema
{
    public class Initializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        public Initializer()
        {
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var rooms = new List<Hall>
            {
                new Hall { Id=Guid.NewGuid(), ColsCount=2,RowsCount=3,ParentLocation=new Location(),SeatsList = new List<List<SeatType>>()}
              
            };
            rooms.ForEach(room => context.Halls.Add(room));
            context.SaveChanges();
        }
    }
}