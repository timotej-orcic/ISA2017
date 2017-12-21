using System;
using System.Collections.Generic;

namespace Isa2017Cinema.Models
{
    public class Projection
    {
        public Guid Id { get; set; }
        public LocationType ProjLocation { get; set; }
        public String Name { get; set; }
        public List<String> ActorsList { get; set; }
        public String Genre { get; set; }
        public String DirectorName { get; set; }
        public int DurationTime { get; set; }
        public byte[] PosterImage { get; set; }
        public Double AvgRating { get; set; }
        public String Description { get; set; }
        public List<Hall> ProjHallsList { get; set; }
        public List<DateTime> ProjTimeList { get; set; }
        public Double TicketPrice { get; set; }
    }
}