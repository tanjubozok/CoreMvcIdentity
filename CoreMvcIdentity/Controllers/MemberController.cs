using CoreMvcIdentity.Identity;
using CoreMvcIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreMvcIdentity.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Profil()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                var model = new ProfilModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Phone = user.PhoneNumber
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult EditPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditPassword(EditPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                bool checkPassword = await _userManager.CheckPasswordAsync(user, model.Password);
                if (checkPassword)
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);

                        await _signInManager.SignOutAsync();
                        await _signInManager.PasswordSignInAsync(user, model.NewPassword, false, false);

                        ModelState.AddModelError("", "Şifre başarılı bir şekilde değiştirildi.");
                        return View();
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Kullanmış olduğunuz şifre hatalıdır.");
                }
            }
            return View(model);
        }
    }
}