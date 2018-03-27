using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public enum PointsType { BRONZE, SILVER, GOLD }

    public class Points
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public PointsType Points_Type { get; set; }
        public int PointsCount { get; set; }
    }
}