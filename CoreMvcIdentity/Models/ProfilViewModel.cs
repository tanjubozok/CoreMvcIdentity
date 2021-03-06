using CoreMvcIdentity.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Models
{
    public class ProfilViewModel
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

        [Display(Name = "Şehir")]
        public string City { get; set; }

        [Display(Name = "Profil Resimi")]
        public string Picture { get; set; }

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime? BirthDay { get; set; }

        [Display(Name = "Cinsiyet")]
        public Gender Gender { get; set; }
    }
}
