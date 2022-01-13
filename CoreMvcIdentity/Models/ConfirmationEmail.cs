using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Models
{
    public class ConfirmationEmail
    {
        [Required(ErrorMessage = "E-posta zorunlu alandır")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi")]
        [Display(Name = "E-posta Adresi")]
        public string EmailAddress { get; set; }

        public string Message { get; set; }
        public string Status { get; set; }
    }
}
