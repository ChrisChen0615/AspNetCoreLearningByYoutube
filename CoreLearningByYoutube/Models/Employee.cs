using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CoreLearningByYoutube.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Display(Name = "名稱")]
        [Required(ErrorMessage = "姓名必填")]
        [MaxLength(10, ErrorMessage = "最多10個字")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email必填")]
        [RegularExpression(@"[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "無效mail位址")]
        public string Email { get; set; }

        [Display(Name = "部門")]
        [Required(ErrorMessage = "部門必選")]
        public Dept? Department { get; set; }

        public string PhotoPath { get; set; }
    }
}
