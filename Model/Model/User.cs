using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Model
{
    [Table("Users")] 
    public class User
    {
        [Key]
        public int UserId { get; set; } 

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Please enter a valid 10-digit phone number starting with 6, 7, 8, or 9.")]
        public string PhoneNumber { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public ICollection<Note> Notes { get; set; } = new List<Note>();

        public ICollection<Collaborator> Collaborators { get; set; } = new List<Collaborator>();

    }
}
