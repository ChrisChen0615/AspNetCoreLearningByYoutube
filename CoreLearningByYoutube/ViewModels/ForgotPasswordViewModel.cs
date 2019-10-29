using System.ComponentModel.DataAnnotations;

namespace CoreLearningByYoutube.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
