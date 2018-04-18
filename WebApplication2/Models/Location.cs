using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Isa2017Cinema.Models
{
    public enum LocationType { CINEMA, THEATRE }
    public class Location
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public LocationType LocType { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string MyAdminId { get; set; }
        public string City { get; set; }
        public List<Ticket> DiscountedTicketsList { get; set; }
        public List<Projection> ProjectionsList { get; set; }
        public List<Hall> HallsList { get; set; }
        public List<Recension> RecensionsList { get; set; }
        public Double AvgRating { get; set; }
    }

    
}