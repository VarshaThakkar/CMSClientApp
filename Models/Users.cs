using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CMSClientApp.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "This field is required!")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "ConfirmPassword and Password values must be the same")]
        public string ConfirmPassword { get; set; }
        public bool Status { get; set; }
    }
}
