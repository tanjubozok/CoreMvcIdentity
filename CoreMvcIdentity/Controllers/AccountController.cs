using CoreMvcIdentity.Identity;
using CoreMvcIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcIdentity.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult UserList()
        {
            var userList = _userManager.Users.ToList();
            return View(userList);
        }

        public IActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View(new LoginModel());
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

                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        //giriş sayısını sıfırla
                        await _userManager.ResetAccessFailedCountAsync(user);

                        return TempData["returnUrl"] != null
                            ? Redirect(TempData["returnUrl"].ToString())
                            : RedirectToAction("Index", "Home");
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
            return View(new RegisterModel());
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

        public IActionResult Logout()
        {
            return View();
        }
    }
}
