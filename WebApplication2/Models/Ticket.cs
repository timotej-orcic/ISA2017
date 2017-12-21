using System;

namespace Isa2017Cinema.Models
{
    public class Ticket
    {
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