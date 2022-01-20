using CoreMvcIdentity.Enums;
using CoreMvcIdentity.Identity;
using CoreMvcIdentity.Models;
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

        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Profil()
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    var model = new ProfilModel
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

        public async Task<IActionResult> EditMember()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var model = new EditMemberModel()
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
        public async Task<IActionResult> EditMember(EditMemberModel model, IFormFile userPicture)
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
    }
}