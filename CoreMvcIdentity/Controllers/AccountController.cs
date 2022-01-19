using CoreMvcIdentity.Identity;
using CoreMvcIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CoreMvcIdentity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        ModelState.AddModelError("", "Çok fazla yanlış giriş denemesi yaptığınız için hesabınız bir süreliğine kitlenmiştir.");
                        return View(model);
                    }
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Email adresiniz onaylanmamıştır. Lütfen eposta adresini kontrol ediniz.");
                        return View(model);
                    }

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        //giriş sayısını sıfırla
                        await _userManager.ResetAccessFailedCountAsync(user);

                        return RedirectToAction("Profil", "Member");
                        //return TempData["returnUrl"] != null ? Redirect(TempData["returnUrl"].ToString()) : RedirectToAction("Profil", "Member");
                    }
                    else
                    {
                        //giriş sayısını +1
                        await _userManager.AccessFailedAsync(user);

                        //kac kes yanlış girdi.
                        int failedCount = await _userManager.GetAccessFailedCountAsync(user);
                        ModelState.AddModelError("", $"{failedCount} kez başarsız giriş denemesi yaptınız.");
                        if (failedCount > 3)
                        {
                            await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(60)));
                            ModelState.AddModelError("", "Hesabınız 4 kez başarısız giriş denemesi yaptığınız için 60 dakika süre ile kilitlenmiştir.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Kullanıcı Adı veyay Şifre hatalıdır.");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı Adı veya Şifre hatalıdır.");
                }
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    UserName = model.UserName,
                    PhoneNumber = model.Phone != "" ? model.Phone : "",
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string urlLink = Url.Action("ConfirmationEmail", "Account", new
                    {
                        userId = user.Id,
                        token = emailConfirmationToken
                    }, protocol: HttpContext.Request.Scheme);

                    string bodyContent = $"<a href='{urlLink}'>Kayıt Onay Link</a>";

                    Helpers.Mail.Send(user.Email, "Kayıt Onay", bodyContent);

                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(model);
        }

        public IActionResult ForgatPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgatPassword(ForgatPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var generatePasswordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var generatePasswordResetLink = Url.Action("ResetPassword", "Account", new
                {
                    userId = user.Id,
                    token = generatePasswordResetToken
                }, HttpContext.Request.Scheme);

                string bodyContent = $"<a href='{generatePasswordResetLink}'>Reset Password Link</a>";
                try
                {
                    Helpers.Mail.Send(model.Email, "Şifre Sıfırlama", bodyContent);
                    ModelState.AddModelError("", "Şifre sıfırlama linki kayıtlı e-posta adresine gönderildi.");
                    return View(new ForgatPasswordModel());
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "E-posta gönderilemedi.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Kayıtlı e-posta adresi bulunamadı.");
            }
            return View(model);
        }

        public IActionResult ResetPassword([Bind("UserId", "Token")] ResetPasswordModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model, string mod)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);
                        ModelState.AddModelError("", "Şifre değiştirme işleminiz başarılı bir şekilde tamamlandı.");
                        return View(new ResetPasswordModel());
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            if (item.Code == "InvalidToken")
                            {
                                ModelState.AddModelError("", "Bu link ile daha önce şifre değiştirilmiştir.");
                            }
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmationEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    var model = new ConfirmationEmail
                    {
                        Message = "Doğrulama işlemi başarılı oldu.",
                        Status = "success",
                        EmailAddress = user.Email
                    };
                    return View(model);
                }
                else
                {
                    var model = new ConfirmationEmail
                    {
                        Message = "Doğrulama işlemi başarısız oldu.",
                        Status = "warning",
                        EmailAddress = user.Email
                    };
                    return View(model);
                }
            }
            else
            {
                var model = new ConfirmationEmail
                {
                    Message = "Kullanıcı bulunamadı.",
                    Status = "danger",
                    EmailAddress = user.Email == "" ? "" : user.Email
                }; return View(model);
            }
        }

        public IActionResult ReplaceConfirmationEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReplaceConfirmationEmail(ConfirmationEmail model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.EmailAddress);
                if (user != null)
                {
                    string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    string urlLink = Url.Action("ConfirmationEmail", "Account", new
                    {
                        userId = user.Id,
                        token = emailConfirmationToken
                    }, protocol: HttpContext.Request.Scheme);

                    string bodyContent = $"<a href='{urlLink}'>Kayıt Onay Link</a>";
                    Helpers.Mail.Send(user.Email, "Kayıt Onay", bodyContent);

                    ModelState.AddModelError("", "E-Posta adresinize onaylama linki başarılı bir şekilde gönderildi.");
                }
                else
                {
                    ModelState.AddModelError("", "Kayıtlı bir e-posta adresi bulunamadı.");
                }
            }
            else
            {
                ModelState.AddModelError(model.EmailAddress, "Email alanı zorunlu alandır.");
            }
            return View(model);
        }
    }
}
