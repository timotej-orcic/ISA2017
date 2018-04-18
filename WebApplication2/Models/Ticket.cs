using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Models;

namespace Isa2017Cinema.Models
{
    public class Ticket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public HallTimeProjection Projection { get; set; }
        public int SeatColumn { get; set; }
        public int SeatRow { get; set; }
        public Double Price { get; set; }
        public Double DiscountMultiplier { get; set; }

        public static implicit operator List<object>(Ticket v)
        {
            throw new NotImplementedException();
        }
    }
}