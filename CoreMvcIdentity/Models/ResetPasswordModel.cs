using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Şifre zorunlu alandır.")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
