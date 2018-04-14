using Foolproof;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class AddNewAdminViewModel
    {
        [Required(ErrorMessage = "The admin type is required")]
        public AdminType Admin_Type { get; set; }

        [Required(ErrorMessage = "The name is required")]
        [MaxLength(20)]
        [RegularExpression("[A-Z][a-z]{2,19}", ErrorMessage = "Invalid name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The lastname is required")]
        [MaxLength(20)]
        [RegularExpression("[A-Z][a-z]{2,19}", ErrorMessage = "Invalid last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The user name is required")]
        [RegularExpression("[A-Za-z0-9]{4,20}", ErrorMessage = "Invalid username")]
        public string UserName { get; set; }

        [RequiredIf("Admin_Type", 1, ErrorMessage = "The Location is required")]
        public string MyLocationId { get; set; }
    }
}