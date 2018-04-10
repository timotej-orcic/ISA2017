using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace WebApplication2.Models
{
    public class AddNewPostViewModel
    {
        [Required(ErrorMessage = "The name is required")]
        [MaxLength(20)]
        [RegularExpression("[A-Z][A-Za-z0-9' ']{2,19}", ErrorMessage = "Invalid name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The description is required")]
        [MaxLength(300)]
        public string Description { get; set; }

        [Required(ErrorMessage = "The open bidding date is required")]
        public DateTime BiddingDate { get; set; }

        [DataType(DataType.Upload)]
        public HttpPostedFileBase ImageUpload { get; set; }

    }
}