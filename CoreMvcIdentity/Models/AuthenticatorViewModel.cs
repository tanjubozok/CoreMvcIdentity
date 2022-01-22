using CoreMvcIdentity.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreMvcIdentity.Models
{
    public class AuthenticatorViewModel
    {
        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }

        [Display(Name = "Doğrulama Kodunuz")]
        [Required(ErrorMessage = "Doğrulama kodu gereklidir")]
        public string VerificationCode { get; set; }

        [Display(Name = "İki Adımla Doğrulama Seçeneği")]
        public TwoFactor TwoFactorType { get; set; }

        public string TwoFactorTypeUpdateMessage { get; set; }

        public string VerifyTwoFactorTokenUpdateMessage { get; set; }
        public IEnumerable<string> RecoveryCodes { get; set; }
    }
}
