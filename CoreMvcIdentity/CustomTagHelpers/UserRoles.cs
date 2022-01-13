using CoreMvcIdentity.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvcIdentity.CustomTagHelpers
{
    [HtmlTargetElement("td", Attributes = "user-roles")]
    public class UserRoles : TagHelper
    {
        private UserManager<AppUser> _userManager;

        public UserRoles(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HtmlAttributeName("user-roles")]
        public string UserId { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            var roles = await _userManager.GetRolesAsync(user);

            string html = string.Empty;
            roles.ToList().ForEach(x =>
            {
                html += $"<span class='badge bg-info text-dark'>{x}</span>";
                html += "</br>";
            });

            output.Content.SetHtmlContent(html);
        }
    }
}
