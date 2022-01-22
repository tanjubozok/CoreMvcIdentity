using CoreMvcIdentity.Enums;
using CoreMvcIdentity.Identity;
using CoreMvcIdentity.Models;
using CoreMvcIdentity.TwoFactorServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcIdentity.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TwoFactorService _twoFactorService;

        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TwoFactorService twoFactorService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _twoFactorService = twoFactorService;
        }

        public async Task<IActionResult> Profil()
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    var model = new ProfilViewModel
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        Phone = user.PhoneNumber,
                        BirthDay = user.BirthDay,
                        City = user.City,
                        Picture = user.Picture,
                        Gender = (Gender)user.Gender
                    };
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult EditPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditPassword(EditPasswordViewModel model)
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

        public async Task<IActionResult> EditMember()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var model = new EditMemberViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.PhoneNumber,
                UserName = user.UserName,
                BirthDay = user.BirthDay,
                City = user.City,
                Picture = user.Picture,
                Gender = (Gender)user.Gender
            };
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember(EditMemberViewModel model, IFormFile userPicture)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                string userPhone = await _userManager.GetPhoneNumberAsync(user);
                if (userPhone != model.Phone && _userManager.Users.Any(u => u.PhoneNumber == model.Phone))
                {
                    ModelState.AddModelError("", "Bu telefon numarası başka üye tarafından kullanılmaktadır.");
                    return View(model);
                }

                if (userPicture?.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/user", fileName);

                    using var stream = new FileStream(path, FileMode.Create);
                    await userPicture.CopyToAsync(stream);
                    user.Picture = "/images/user/" + fileName;
                }

                user.UserName = model.UserName;
                user.PhoneNumber = model.Phone;
                user.Email = model.Email;
                user.City = model.City;
                user.BirthDay = model.BirthDay;
                user.Gender = (int)model.Gender;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(user, false);

                    ModelState.AddModelError("", "Güncelleme işlemi başarılı bir şekilde tamamlandı.");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Profil");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> TwoFactorAuth(AuthenticatorViewModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (string.IsNullOrEmpty(model.VerifyTwoFactorTokenUpdateMessage))
            {
                var baseModel = new AuthenticatorViewModel
                {
                    TwoFactorType = (TwoFactor)user.TwoFactor,
                    TwoFactorTypeUpdateMessage = null
                };
                return View(baseModel);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorAuth(AuthenticatorViewModel model, bool empty)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            switch (model.TwoFactorType)
            {
                case TwoFactor.None:
                    user.TwoFactorEnabled = false;
                    user.TwoFactor = (sbyte)TwoFactor.None;
                    model.TwoFactorTypeUpdateMessage = "İki adımlı kimlik doğruluma tipiniz  <b>'Hiçbiri'</b> olarak seçilmiştir.";
                    break;
                case TwoFactor.Phone:
                    break;
                case TwoFactor.Email:
                    break;
                case TwoFactor.MicrosoftGoogle:
                    return RedirectToAction("TwoFactorWithAuthenticator");
                default:
                    break;
            }
            await _userManager.UpdateAsync(user);
            return View(model);
        }

        public async Task<IActionResult> TwoFactorWithAuthenticator()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            string getAuthenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(getAuthenticatorKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                getAuthenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            AuthenticatorViewModel model = new()
            {
                SharedKey = getAuthenticatorKey,
                AuthenticatorUri = _twoFactorService.GenerateQrCodeUri(user.Email, getAuthenticatorKey)
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorWithAuthenticator(AuthenticatorViewModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var verificationCode = model.VerificationCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var verifyTwoFactorToken = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (verifyTwoFactorToken)
            {
                user.TwoFactorEnabled = true;
                user.TwoFactor = (sbyte)TwoFactor.MicrosoftGoogle;

                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 5);

                model.VerifyTwoFactorTokenUpdateMessage = "İki adımlı kimlik doğrulama tipiniz Microsoft/Google Authenticator olarak belirlenmiştir.";
                model.RecoveryCodes = recoveryCodes;
                model.TwoFactorType = (TwoFactor)user.TwoFactor;

                return RedirectToAction("TwoFactorAuth", model);
            }
            else
            {
                ModelState.AddModelError("", "Girdiğiniz doğrulama kodu yanlıştır");
                return View(model);
            }
        }
    }
}