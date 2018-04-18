using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Models;

namespace Isa2017Cinema.Models
{
    public enum SeatType { OPENED_FREE, OPENED_TAKEN, CLOSED}
    public class Hall
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Location ParentLocation { get; set; }
        public String Name { get; set; }
        public int RowsCount { get; set; }
        public int ColsCount { get; set; }
        public List<List<SeatType>> SeatsList { get; set; }
        public List<Row> Seats { get; set; }
    }
}