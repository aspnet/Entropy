using System.ComponentModel.DataAnnotations;

namespace Mvc.FileUpload.Models
{
    public class User
    {
        [MinLength(5, ErrorMessage = "Name of the user must be alteast 5 characters long.")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Range(18, 45, ErrorMessage = "Age must be between 18 and 45 (inclusive)")]
        [Required(ErrorMessage = "Age is required")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Zipcode is required")]
        public int Zipcode { get; set; }
    }
}
