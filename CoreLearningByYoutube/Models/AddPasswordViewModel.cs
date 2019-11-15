using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreLearningByYoutube.Models
{
    public class AddPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "必須與新密碼符合")]
        [Display(Name = "確認密碼")]
        public string ConfirmPassword { get; set; }
    }
}
