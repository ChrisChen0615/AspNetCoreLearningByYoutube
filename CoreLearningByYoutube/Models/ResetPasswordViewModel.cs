using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CoreLearningByYoutube.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }
                
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="必須與密碼符合")]
        [Display(Name = "確認密碼")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
