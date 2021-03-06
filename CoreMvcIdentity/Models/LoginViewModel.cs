using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta zorunlu alandır")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi")]
        [Display(Name = "E-posta Adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunlu alandır")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
