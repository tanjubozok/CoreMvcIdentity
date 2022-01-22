using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Models
{
    public class ResetPasswordByAdminResetViewModel
    {
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }
    }
}
