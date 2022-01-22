using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Models
{
    public class ForgatPasswordViewModel
    {
        [Required(ErrorMessage = "E-posta alanı alandır")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi")]
        [Display(Name = "E-posta Adresi")]
        public string Email { get; set; }
    }
}
