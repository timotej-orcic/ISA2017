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
        public String Name { get; set; }
        public String Address { get; set; }
        public String Description { get; set; }
        public List<Ticket> DiscountedTicketsList { get; set; }
        public List<Projection> ProjectionsList { get; set; }
        public List<Hall> HallsList { get; set; }
        public List<Recension> RecensionsList { get; set; }
        public FanZone LocationFanZone { get; set; }
    }
}