using Isa2017Cinema.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class RegisterNewLocationViewModel
    {
        [Required(ErrorMessage = "The location type is required")]
        public LocationType Location_Type { get; set; }

        [Required(ErrorMessage = "The location name is required")]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required(ErrorMessage = "The location address is required")]
        [MaxLength(60)]
        public string Address { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }
    }
}