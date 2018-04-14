using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Isa2017Cinema.Models
{
    public class Licitation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string ParentPostId { get; set; }
        public double OfferedPrice { get; set; }
        public string ParentUserId { get; set; }
        public bool IsAccepted { get; set; }
    }
}