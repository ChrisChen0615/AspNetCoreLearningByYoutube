using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreLearningByYoutube.Models
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        public string Id { get; set; }
                
        [Required(ErrorMessage ="角色名稱必填")]
        [Display(Name = "角色名稱")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
