using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class HallsViewModel
    {
    }
    public class HallViewModel
    {
        [Required]
        [Display(Name = "Hall name")]
        public String Name { get; set; }
        [Required]
        [Display(Name ="Number of rows for seats")]
        public int Rows { get; set; }
        [Required]
        [Display(Name = "Number of seats in every row")]
        public int Columns { get; set; }

    }

    public class FastReserveTicket
    {
        public HallTimeProjection hallTimeProj { get; set; }

        [Required]
        [Display(Name = "Ticket discount in % :(between 0 and 100)")]
        public double Krmaca { get; set; }
    }
}