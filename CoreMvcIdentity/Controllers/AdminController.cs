using CoreMvcIdentity.Identity;
using CoreMvcIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreMvcIdentity.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            var userList = await _userManager.Users.ToListAsync();
            return View(userList);
        }

        public async Task<IActionResult> UserResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var model = new ResetPasswordByAdminResetViewModel
            {
                UserId = user.Id
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserResetPassword(ResetPasswordByAdminResetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    return RedirectToAction("UserList");
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

        public async Task<IActionResult> Roles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppRole role = new()
                {
                    Name = model.Name
                };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
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
                ModelState.AddModelError("", "Zorunlu alanları doldurunuz.");
            }
            return View(model);
        }

        public async Task<IActionResult> RoleEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError("", "Günceleme işlemi başarısız oldu");
            }
            else
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    var model = new RoleViewModel
                    {
                        Id = role.Id,
                        Name = role.Name
                    };
                    return View(model);
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.Id);
                if (role != null)
                {
                    role.Name = model.Name;
                    var result = await _roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Roles");
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
                    ModelState.AddModelError("", "Role güncellemede hata oluştu.");
                    return RedirectToAction("Roles");
                }
            }
            else
            {
                ModelState.AddModelError("", "Zorunlu alanları doldurunuz.");
            }
            return View(model);
        }

        public async Task<IActionResult> RoleDelete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError("", "Hata oluştu.");
            }
            else
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    var result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Roles");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
            }
            return View();
        }

        public async Task<IActionResult> RoleAssign(string id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "hata oluştu.");
                return View();
            }
            else
            {
                var user = await _userManager.FindByIdAsync(id);
                var roles = _roleManager.Roles;
                var userRole = await _userManager.GetRolesAsync(user);

                List<RoleAssignViewModel> listModel = new();
                foreach (var item in roles)
                {
                    RoleAssignViewModel model = new()
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        RoleId = item.Id,
                        RoleName = item.Name,
                        Exist = userRole.Contains(item.Name)
                    };
                    listModel.Add(model);
                }
                return View(listModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignViewModel> listModel)
        {
            try
            {
                foreach (var item in listModel)
                {
                    var user = await _userManager.FindByIdAsync(item.UserId);
                    if (item.Exist)
                    {
                        await _userManager.AddToRoleAsync(user, item.RoleName);
                    }
                    else
                    {
                        await _userManager.RemoveFromRoleAsync(user, item.RoleName);
                    }
                }
                return RedirectToAction("UserList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(listModel);
            }
        }
    }
}
