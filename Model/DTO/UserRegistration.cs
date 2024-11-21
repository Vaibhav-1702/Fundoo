using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class UserRegistration
    {
        [Required]
        public string name { get; set; }

        [Required]
        [EmailAddress]
        public string emailAddress { get; set; }

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Please enter a valid 10-digit phone number starting with 6, 7, 8, or 9.")]
        public string phoneNumber { get; set; }


        [Required]
        public string password { get; set; }
    }
}
