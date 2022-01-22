using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Models
{
    public class EditPasswordViewModel
    {
        [Required(ErrorMessage = "Kullanığın şifre zorunlu alandır")]
        [Display(Name = "Kullanığın Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Yeni şifre zorunlu alandır")]
        [Display(Name = "Yeni Şifre")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Tekrar yeni şifre zorunlu alandır")]
        [Display(Name = "Yeni Şifre Tekrar")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Yeni şifre ile Tekrar yeni şifre aynı değildir.")]
        public string ReNewPassword { get; set; }
    }
}
