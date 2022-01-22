using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Enums
{
    public enum TwoFactor
    {
        [Display(Name = "Hiçbiri")]
        None = 0,

        [Display(Name = "Telefon ile Kimlik Doğrulama")]
        Phone = 1,

        [Display(Name = "Email ile Kimlik Doğrulama")]
        Email = 2,

        [Display(Name = "Microsoft/Google Authenticator ile Kimlik Doğrulama")]
        MicrosoftGoogle = 3
    }
}
