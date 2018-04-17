using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class HallTimeProjection
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Hall Hall { get; set; }
        public DateTime Time { get; set; }
        public List<Row> Seats { get; set; }
        public Projection Projection { get; set; }
        public Double TicketPrice { get; set; }
    }
}