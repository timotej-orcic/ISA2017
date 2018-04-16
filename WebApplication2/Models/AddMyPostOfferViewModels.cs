using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class AddMyPostOfferViewModel
    {
        [Required(ErrorMessage = "The offer value is required")]
        [RegularExpression(@"((0|([1-9]\d*)){1,10})(\.\d{1,4})?", ErrorMessage = "Invalid offer value")]
        public double OfferValue { get; set; }

        public string PostId { get; set; }
    }
}