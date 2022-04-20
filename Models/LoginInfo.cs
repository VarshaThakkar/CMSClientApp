using System.ComponentModel.DataAnnotations;

namespace CMSClientApp.Models
{
    public class LoginInfo
    {
       
        [Required(ErrorMessage = "This field is required!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
