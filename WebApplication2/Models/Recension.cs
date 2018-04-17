using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Isa2017Cinema.Models
{
    public class Recension
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Location location { get; set; }
        public Projection projection { get; set; }
        public ApplicationUser RecensionUser { get; set; }
        public Double RatingProjection { get; set; }
        public Double RatingLocation { get; set; }
    }
}