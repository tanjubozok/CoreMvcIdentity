using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Kullanıcı adı alanı alandır")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "E-posta alanı alandır")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi")]
        [Display(Name = "E-posta Adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı alandır")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Telefon NUmarası")]
        public string Phone { get; set; }
    }
}
