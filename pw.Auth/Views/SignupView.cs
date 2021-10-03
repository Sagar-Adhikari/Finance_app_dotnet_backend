using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace pw.Auth.Views
{
    public class SignupView
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int ShakhaId { get; set; }
        public List<string> Roles { get; set; }
        [Required]
        public string Password { get; set; }
        public string Post { get; set; }
    }
}
