using CoreLearningByYoutube.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreLearningByYoutube.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name ="信箱")]
        [Required(ErrorMessage ="信箱必填寫")]
        [EmailAddress]
        [Remote(action: "IsEmailInUse",controller:"Account")]//此屬性在輸入當下可直接呼叫action做驗證，不用透過ajax
        [ValidEmailDomain(allowedDomain: "sanrong.com",ErrorMessage = "網域名稱必須為'sanrong.com'")]//submit後才檢查
        public string Email { get; set; }

        [Display(Name = "密碼")]
        [Required(ErrorMessage = "密碼必填寫")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "密碼確認")]
        [DataType(DataType.Password)]        
        [Compare("Password",ErrorMessage = "與密碼不符合")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "城市")]
        public string City { get; set; }
    }
}
