using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Isa2017Cinema.Models
{
    public class Post
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string ParentUserId { get; set; }
        public string Description { get; set; }
        public DateTime OfferExpireDate { get; set; }
        public string ImageUrl { get; set; }
        public bool IsTakenByAdmin { get; set; }
        public string ParentAdminId { get; set; }
        public bool IsChecked { get; set; }
        public bool IsApproved { get; set; }
        public bool IsGraded { get; set; }
        public List<Licitation> LicitationsList { get; set; }
    }
}