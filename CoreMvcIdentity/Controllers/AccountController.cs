using CoreMvcIdentity.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcIdentity.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult UserList()
        {
            var userList = _userManager.Users.ToList();
            return View(userList);
        }
    }
}
