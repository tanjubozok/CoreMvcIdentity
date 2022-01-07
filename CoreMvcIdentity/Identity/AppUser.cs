using Microsoft.AspNetCore.Identity;
using System;

namespace CoreMvcIdentity.Identity
{
    public class AppUser : IdentityUser
    {
        public string City { get; set; }
        public string Picture { get; set; }
        public DateTime? BirthDay { get; set; }
        public int Gender { get; set; }
    }
}
