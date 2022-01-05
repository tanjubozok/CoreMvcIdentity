using CoreMvcIdentity.Identity;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreMvcIdentity.CustomValidations
{
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            List<IdentityError> errors = new();

            if (password.ToLower().Contains(user.UserName))
            {
                errors.Add(new IdentityError() { Code = "PasswordContainsUserName", Description = $"Şifreniz '{user.UserName}' içeremez." });
            }

            return errors.Count > 0 ? Task.FromResult(IdentityResult.Failed(errors.ToArray())) : Task.FromResult(IdentityResult.Success);
        }
    }
}
