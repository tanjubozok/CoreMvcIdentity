using CoreMvcIdentity.Identity;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreMvcIdentity.CustomValidations
{
    public class CustomUserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            List<IdentityError> errors = new();

            string[] digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            foreach (var item in digits)
            {
                if (user.UserName[0].ToString() == item)
                {
                    errors.Add(new IdentityError() { Code = "UserNameFirstDigitContains", Description = "Kullanıcı Adının ilk karakteri rakam içeremez." });
                }
            }

            return errors.Count > 0 ? Task.FromResult(IdentityResult.Failed(errors.ToArray())) : Task.FromResult(IdentityResult.Success);
        }
    }
}
