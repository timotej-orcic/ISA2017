using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class ChangePointsScaleViewModels
    {
        [Required(ErrorMessage = "The bronze points count is required")]
        [RegularExpression("[1-9][0-9]+", ErrorMessage = "Input must be a number greater then 0")]
        public int BronzeCount { get; set; }

        [Required(ErrorMessage = "The bronze points count is required")]
        [RegularExpression("[1-9][0-9]+", ErrorMessage = "Input must be a number greater then 0")]
        public int SilverCount { get; set; }

        [Required(ErrorMessage = "The bronze points count is required")]
        [RegularExpression("[1-9][0-9]+", ErrorMessage = "Input must be a number greater then 0")]
        public int GoldCount { get; set; }
    }
}