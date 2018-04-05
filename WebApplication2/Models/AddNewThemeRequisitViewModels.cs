using System.ComponentModel.DataAnnotations;
using System.Web;

namespace WebApplication2.Models
{
    public class AddNewThemeRequisitViewModel
    {
        [Required(ErrorMessage = "The name is required")]
        [MaxLength(20)]
        [RegularExpression("[A-Z][A-Za-z0-9' ']{2,19}", ErrorMessage = "Invalid name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The price is required")]
        [RegularExpression(@"((0|([1-9]\d*)){1,10})(\.\d{1,4})?", ErrorMessage = "Invalid price")]
        public double Price { get; set; }

        [Required(ErrorMessage = "The available count is required")]
        public int AvailableCount { get; set; }

        [Required(ErrorMessage = "The requisit image is required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase ImageUpload { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }
    }
}