using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Models
{
    public class ProfilModel
    {
        [Required(ErrorMessage = "Kullanıcı adı alanı alandır")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "E-posta alanı alandır")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi")]
        [Display(Name = "E-posta Adresi")]
        public string Email { get; set; }

        [Display(Name = "Telefon Numarası")]
        public string Phone { get; set; }
    }
}
