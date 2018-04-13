using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Isa2017Cinema.Models
{
    public class Ticket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Projection ParentProjection { get; set; }
        public DateTime ProjectionTime { get; set; }
        public Hall ProjectionHall { get; set; }
        public int SeatColumn { get; set; }
        public int SeatRow { get; set; }
        public Double Price { get; set; }
        public Double DiscountMultiplier { get; set; }
    }
}