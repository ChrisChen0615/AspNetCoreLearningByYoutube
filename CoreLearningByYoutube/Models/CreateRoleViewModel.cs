using System.ComponentModel.DataAnnotations;

namespace CoreLearningByYoutube.Models
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "角色名稱必填")]
        [Display(Name = "角色名稱")]
        public string RoleName { get; set; }
    }
}
