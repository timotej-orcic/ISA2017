using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class ProjectionViewModels
    {
    }

    public class ProjectionViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public String Name { get; set; }

        [Required]
        [Display(Name = "Actors list")]
        public String ActorsList { get; set; }

        [Required]
        [Display(Name = "Genre")]
        public String Genre { get; set; }

        [Required]
        [Display(Name = "Director name")]
        public String DirectorName { get; set; }

        [Required]
        [Display(Name = "Duration time")]
        public int DurationTime { get; set; }

        [Required]
        [Display(Name = "Poster url")]
        public String PosterUrl { get; set; }

        [Required]
        [Display(Name = "Average rating")]
        public Double AvgRating { get; set; }


        [Required]
        [Display(Name = "Description")]
        public String Description { get; set; }


        [Required]
        [Display(Name = "Ticket price")]
        public Double TicketPrice { get; set; }
        
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ImageUpload { get; set; }
    }
    public class CallFriendsViewModel
    {
        public ApplicationUser user;

        public HallTimeProjection projectionHallTime;

        public int brKarata;
    }
}