using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreLearningByYoutube.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "信箱")]
        [Required(ErrorMessage = "信箱必填寫")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "密碼")]
        [Required(ErrorMessage = "密碼必填寫")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "記住我")]        
        public bool RememberMe { get; set; }
    }
}
