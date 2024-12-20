﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string emailAddress { get; set; }

        [Required]
        public string password { get; set; }
    }
}
