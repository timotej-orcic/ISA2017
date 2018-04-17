using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Models;

namespace Isa2017Cinema.Models
{
    public class Projection
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String ActorsList { get; set; }
        public String Genre { get; set; }
        public String DirectorName { get; set; }
        public int DurationTime { get; set; }
        public String PosterUrl { get; set; }
        public Double AvgRating { get; set; }
        public String Description { get; set; }
        public List<HallTimeProjection> ProjHallsTimeList { get; set; }
         
    }
}